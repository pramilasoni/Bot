using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Build.Labs.BotFramework.Models;
using Build.Labs.BotFramework.Services;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts;
using Microsoft.Bot.Builder.Prompts.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text;
using Newtonsoft.Json;
using TextPrompt = Microsoft.Bot.Builder.Dialogs.TextPrompt;
using ChoicePrompt = Microsoft.Bot.Builder.Dialogs.ChoicePrompt;

namespace Build.Labs.BotFramework.Bots
{
    public static class PromptStep
    {
        public const string GatherInfo = "gatherInfo";
        public const string LocationPrompt = "locationPrompt";
        public const string TimePrompt = "timePrompt";
        public const string CarTypePrompt = "carTypePrompt";
    }

    public class CarsBot : IBot
    {
        private readonly DialogSet dialogs;
        private readonly PriceEstimateService priceService = new PriceEstimateService();
        public CarsBot()
        {
            dialogs = new DialogSet();
           // dialogs.Add(PromptStep.CarTypePrompt, new TextPrompt());
            dialogs.Add(PromptStep.CarTypePrompt, new ChoicePrompt(Culture.English));

            dialogs.Add(PromptStep.TimePrompt, new TextPrompt());
            dialogs.Add(PromptStep.LocationPrompt, new TextPrompt(LocationValidator));
            dialogs.Add(PromptStep.GatherInfo, new WaterfallStep[] { CarTypeStep, TimeStep, LocationStep, FinalStep });
        }

        public async Task OnTurn(ITurnContext turnContext)
        {
            var state = turnContext.GetConversationState<ReservationData>();
            var dialogContext = dialogs.CreateContext(turnContext, state);
            await dialogContext.Continue();

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    //if (!turnContext.Responded)
                    //{
                    //    await turnContext.SendActivity("Welcome to Contoso Ridesharing!");
                    //    throw new Exception("Show me the error handling!");
                    //}
                    //break;
                    if (!turnContext.Responded)
                    {
                        var result = turnContext.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);
                        var topIntent = result?.GetTopScoringIntent();
                        // Your code goes here
                        switch (topIntent != null ? topIntent.Value.intent : null)
                        {
                            case LuisIntents.Greetings:
                                await turnContext.SendActivity("Hello! I am your ride sharing assistant.");
                                break;
                            case LuisIntents.GetPriceEstimate:
                                var location = (string)result.Entities[LuisEntities.Location]?.First;
                                GetPriceEstimateHandler(turnContext, location);
                                break;
                            case LuisIntents.ScheduleRide:
                                var carType = (string)result.Entities[LuisEntities.CarType]?.First;
                                location = (string)result.Entities[LuisEntities.Location]?.First;
                                var time = GetTimeValueFromResult(result);
                                ScheduleRideHandler(turnContext, carType, location, time);
                                break;

                            default:
                                await turnContext.SendActivity("Sorry, I didn't understand that.");
                                break;
                        }



                    }
                    break;

                case ActivityTypes.ConversationUpdate:
                    foreach (var newMember in turnContext.Activity.MembersAdded)
                    {
                        if (newMember.Id != turnContext.Activity.Recipient.Id)
                        {
                            await turnContext.SendActivity("Hello and welcome to the Build Bot Framework Lab!");
                        }
                    }
                    break;
            }
        }

        private async Task CarTypeStep(DialogContext dialogContext, object result, SkipStepFunction next)
        {
            var state = dialogContext.Context.GetConversationState<ReservationData>();
            if (state.CarType == null)
            {
                // Your code goes here
                var actions = new[]
                {
                    new CardAction(type: ActionTypes.ImBack, title: "Sedan", value: "Sedan"),
                    new CardAction(type: ActionTypes.ImBack, title: "SUV", value: "SUV"),
                    new CardAction(type: ActionTypes.ImBack, title: "Sports car", value: "Sports car")
                };

                var heroCard = new HeroCard(buttons: actions);
                var activity = (Activity)MessageFactory.Carousel(new[] { heroCard.ToAttachment() }, "Please select a car type.");
                var choices = actions.Select(x => new Choice { Action = x, Value = (string)x.Value }).ToList();
                await dialogContext.Prompt(PromptStep.CarTypePrompt, activity, new ChoicePromptOptions { Choices = choices });

            //    await dialogContext.Context.SendActivity("What kind of vehicle would you like?");
            //    await dialogContext.Prompt(PromptStep.CarTypePrompt, $"Available options are: {string.Join(", ", BotConstants.CarTypes)}");
            }
            else
            {
                await next();
            }
        }

        private async Task TimeStep(DialogContext dialogContext, object result, SkipStepFunction next)
        {
            var state = dialogContext.Context.GetConversationState<ReservationData>();
            if (result != null)
            {
                // state.CarType = (result as TextResult).Value;
                state.CarType = ((ChoiceResult)result).Value.Value;

            }

            if (string.IsNullOrEmpty(state.Time))
            {
                await dialogContext.Prompt(PromptStep.TimePrompt, "When do you need the ride?");
            }
            else
            {
                await next();
            }
        }

        private async Task LocationStep(DialogContext dialogContext, object result, SkipStepFunction next)
        {
            var state = dialogContext.Context.GetConversationState<ReservationData>();
            if (result != null)
            {
                var time = (result as TextResult).Value;
                state.Time = time;
            }

            if (state.Location == null)
            {
                await dialogContext.Prompt(PromptStep.LocationPrompt, "Where are you heading to?");
            }
            else
            {
                await next();
            }
        }

        private async Task FinalStep(DialogContext dialogContext, object result, SkipStepFunction next)
        {
            var state = dialogContext.Context.GetConversationState<ReservationData>();
            if (result != null)
            {
                state.Location = (result as TextResult).Value;
            }
            //steps
            var receipt = await GetReceiptCard(state);
            var message = MessageFactory.Attachment(receipt);
            await dialogContext.Context.SendActivity(message);

            // Your code goes here

            await dialogContext.Context.SendActivity("Thanks for your reservation!");
            await dialogContext.End(state);
        }

        private async Task<Attachment> GetReceiptCard(ReservationData reservation)
        {

            var price = await priceService.GetRidePriceEstimate(reservation.Location);
            var pickupTime = reservation.Time;
            var receiptCard = new ReceiptCard()
            {
                Title = "Ride Sharing Receipt",
                // steps
                Facts = new List<Fact>
                {
                    new Fact("Booking Number", "000345"),
                    new Fact("Customer", "Lance Olson"),
                    new Fact("Payment Method", "VISA 5555-****")
                },
                Items = new List<ReceiptItem>
                {
                    new ReceiptItem(title: reservation.CarType, subtitle: pickupTime, price: price, quantity: "1", image: new CardImage(BotConstants.CarImageUrl))
                },
                Tax = "$ 5.50",
                Total = price,
                Buttons = new List<CardAction>
                {
                    new CardAction()
                    {
                        Type = ActionTypes.OpenUrl,
                        Title = "More information",
                        Image = BotConstants.CompanyLogoUrl,
                        Value = "https://azure.microsoft.com/en-us/pricing/"
                    }
                }

            };
            return receiptCard.ToAttachment();
        }

        private async void ScheduleRideHandler(ITurnContext turnContext, string carType, string location, string time)
        {
            var state = turnContext.GetConversationState<ReservationData>();
            var dialogContext = dialogs.CreateContext(turnContext, state);
            state.CarType = carType;
            state.Location = location;
            state.Time = time;
            await dialogContext.Begin(PromptStep.GatherInfo);
        }

        private string GetTimeValueFromResult(RecognizerResult result)
        {
            var timex = (string)result.Entities["builtin_datetime"]?.First["timex"].First;
            if (timex != null)
            {
                timex = timex.Contains(":") ? timex : $"{timex}:00";
                return DateTime.Parse(timex).ToString("MMMM dd HH:mm tt");
            }

            return null;
        }

        private async void GetPriceEstimateHandler(ITurnContext context, string location)
        {
            if (location != null)
            {
                var priceEstimate = await priceService.GetRidePriceEstimate(location);
                await context.SendActivity($"I estimate that it will cost {priceEstimate} to get to {location}.");
            }
            else
            {
                await context.SendActivity("What's the location?");
            }
        }

        private async Task LocationValidator(ITurnContext context, TextResult result)
        {
            if (result.Value.Length <= 3)
            {
                result.Status = PromptStatus.NotRecognized;
                await context.SendActivity("The location should be at least 3 characters long.");
            }
        }
    }
}