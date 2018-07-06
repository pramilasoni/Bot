# Build 2018: Getting Started with Azure Bot Service and LUIS

This lab will show you how to build a bot using the new Microsoft Bot Framework and Azure. The lab scenario focuses on Contoso Ridesharing, a fictional car sharing company, who wants their customers to be able to book rides using an interactive bot. In order to accomplish this, customers will need to be able to query prices and fare rules using natural language.

## Setup your environment

### A) Setup your Azure subscription

This lab **requires** an Azure subscription.

### B) Download the lab materials

Before we start the lab, we need to prepare the lab VM. Follow the next steps **carefully** to ensure your directory structure is created correctly. Once complete, you should have some files and the `BotService` and `Resources` folders under `Downloads`.

1. [] Open **Microsoft Edge** and navigate to ++https://aka.ms/botframeworklab++.
1. [] When prompted, click **Open**.
1. [] Click **Extract** and then **Extract all**.
1. [] Click **Browse...** and then **Downloads**.
1. [] Click **Select Folder** and then **Extract**.

## Getting started with Azure Bot Service

### A) Create Azure Bot Service

The Azure Bot Service is an integrated offering for building and hosting bots. It pulls together the Microsoft Bot Framework for core bot functionality and Azure Web Apps for hosting. In this lab, we'll be using an Azure Web App to host your bot but Azure Functions is also supported.

1. [] Log into the Azure Portal (++portal.azure.com++).
1. [] In the **New** blade, search for **Web App Bot**.
1. [] **Select** the first result and then click the **Create** button.
1. [] Provide the required information:
    * Bot name: `build-bot-<your initials>`
    * Use a resource group of your preference.
    * Location: `West US`
    * Pricing tier: `F0 (10K Premium Messages)`
    * App name: `build-bot-<your initials>`
    * Bot template: `Basic C#`
    * Azure Storage: create a new one with the recommended name
    * Application Insights Location: `West US`
1. [] Click on **App service plan/Location**.
1. [] Click **Create New**.
1. [] Provide the required information:
    * App Service plan name: `build-bot-<your initials>`
    * Location: `West US`
1. [] Click **OK** to save the new App service plan.
1. [] Click **Create** to deploy the service. This step might take a few moments.
1. [] Once the deployment is completed you will see a **Deployment succeeded** notification.
1. [] Go to **All Resources** in the left pane and **search** for the new resource (`build-bot-<your initials>`).
1. [] Click on the **Web App Bot** to open it.
1. [] Click on the **Test in Web Chat** option in the menu to test the new bot.
1. [] Type **Hello** into the built-in chat control.

    > [!NOTE] You should see the response from the bot.

We recommend keeping this tab open. We'll be returning to the Azure Portal shortly.

### B) Run locally

Before we run our bot in Azure, we'll run it locally in Visual Studio 2017 using the Bot Emulator.

1. [] Open **Visual Studio 2017** from the Start Menu.
1. [] Click **Open Project/Solution**.
1. [] Select the **solution file** `Downloads\BotService\BotService.sln` and wait for it to load.
1. [] Expand the **Dependencies** node in the **Solution Explorer**.
1. [] Expand the **NuGet** node.

    > [!NOTE] We've pre-installed the latest preview version of the new Bot Framework SDK. We've included packages with support for Azure and LUIS.
1. [] Run the app by clicking on the **IIS Express** button in Visual Studio (with the green play icon).
1. [] Once the page has loaded in Microsoft Edge, **copy** the site's endpoint from the address bar to the clipboard (e.g. `http://localhost:XXXXX`).

### C) Debugging with Bot Framework Emulator

The bot emulator provides a convenient way to interact and debug your bot locally.

1. [] Open **CarsBot.cs** from the **Bots** folder.
1. [] Put a **breakpoint** on line 41.
1. [] Run the app by clicking on the **IIS Express** button in Visual Studio (with the green play icon).
1. [] Open the **botframework-emulator** from the Start Menu.
1. [] **Paste** the endpoint copied from the previous section into the address bar of the emulator.
1. [] **Append** the bot path to the URL: ++/api/messages++
1. [] **Type** ++Hello++ and press enter.
1. [] Return to **Visual Studio** and wait for the breakpoint to be hit.
1. [] **Mouse over** the `turnContext.Activity.Text` variable to see your input message.
1. [] Press **Continue** in the toolbar.
1. [] **Remove** the breakpoint.
1. [] **Stop** debugging by clicking the stop button in the toolbar.

### D) Deploy to Azure from Visual Studio

For the purposes of our lab, we'll be deploying directly from Visual Studio.

1. [] Open **CarsBot.cs** in the **Bots** folder.
1. [] **Replace** the `Hello World!` message on line 41 with: ++Welcome to Contoso Ridesharing!++
1. [] Click on `Bot Demo` in the top right corner of **Visual Studio**.
1. [] Click on **Account Settings...**.
1. [] Click on the **Sign out** button.
1. [] Click on the **Sign in** button.
1. [] **Login** with the same credentials as you used for **Azure**.

    > [!NOTE] This will connect Visual Studio to your Azure subscription.
1. [] Click **Close**.
1. [] **Right-click** the `BotService` project.
1. [] Click **Publish**.
1. [] Mark the option `Select Existing`.
1. [] Click **Publish**.
1. [] Select the **Web App** under the only **resource group**.
1. [] Click **OK**.
1. [] **Wait** for the deployment to complete. This operation might take a few minutes.
1. [] Return to your **Bot Service Web App** in the **Azure Portal**.
1. [] Go to **Test in Web Chat** page.
1. [] **Type** ++Hello++ and press enter.

    > [!NOTE] The bot should reply with the new message.

## Add middleware to the bot pipeline

### A) Add error handling

The new SDK provides a new pipeline-based model for building bots. One of the most useful during development is `CatchExceptionMiddleware`. It provides a single location for catching and managing errors. It also provides extensions to send friendly errors to the end users while tracing the full exception details for troubleshooting purposes.

1. [] In **Visual Studio**, open **Startup.cs**.
1. [] **Review** the existing middleware.

    > [!NOTE] The `ConversationState` middleware is configured to use in-memory storage for maintaining conversation data. We'll improve this later on in the lab.
1. [] **Add** the following import:

    ```cs
    using Microsoft.Bot.Builder.TraceExtensions;
    ```
1. [] **Add** the following code to the `ConfigureServices` method after the conversation state middleware:

    ```cs
    options.Middleware.Add(new CatchExceptionMiddleware<Exception>(async (context, exception) =>
    {
        await context.TraceActivity("CarsBot exception", exception);
        await context.SendActivity("Oops! It looks like something went wrong!");
    }));
    ```
1. [] Put a **breakpoint** on the first line of the exception middleware (i.e. on the call to `TraceActivity`).
1. [] Open **CarsBot.cs** from the **Bots** folder.
1. [] **Add** the following code just **after** the `Contoso` greeting.

    ```cs
    throw new Exception("Show me the error handling!");
    ```
1. [] Run the app by clicking on the **IIS Express** button in Visual Studio (with the green play icon).

### B) Test error handling

Let's see the error handling middleware in action.

1. [] Return to the **botframework-emulator**.
1. [] Click the **refresh** button to start a new conversation.
1. [] Type ++Hello++ and press **enter**.

    > [!NOTE] You should receive the greetings response from your local bot.
1. [] Return to **Visual Studio** and wait for the breakpoint to be hit.
1. [] **Mouse over** the `context` and `exception` to inspect their contents.
1. [] Press **Continue** in the toolbar.
1. [] **Remove** the breakpoint.
1. [] Return to the **Bot Emulator**.

    > [!NOTE] You should now see an additional message from the exception middleware. You won't see the actual exception message here.
1. [] **Stop** debugging by clicking the stop button in the toolbar.
1. [] Return to **CarsBot.cs**.
1. [] **Remove** the exception previously added.

## Adding functionality to your bot

### A) Create a LUIS subscription

Language Understanding (LUIS) allows your application to understand what a person wants in their own words. LUIS uses machine learning to allow developers to build applications that can receive user input in natural language and extract meaning from it.

While LUIS has a standalone portal for building the model, it uses Azure for subscription management.

Set up your own LUIS environment:

1. [] Return to the Azure Portal (++portal.azure.com++).
1. [] In the **New** blade, search for **Language Understanding**.
1. [] **Select** the first result and then click the **Create** button.
1. [] Provide the required information:
    * App name: `build-bot-luis-<your_initials>`.
    * Location: `West US`.
    * Pricing tier: `F0 (5 Calls per second, 10K Calls per month)`.
    * Use existing resource group: `@lab.CloudResourceGroup(329).Name`
    * **Confirm** that you read and understood the terms by **checking** the box.
1. [] Click **Create**. This step might take a few seconds.
1. [] Once the deployment is complete, you will see a **Deployment succeeded** notification.
1. [] Go to **All Resources** in the left pane and **search** for the new resource (`build-bot-luis-<your initials>`).
1. [] **Click** on the resource.
1. [] Go to the **Keys** page.
1. [] Copy the **Key 1** value into **Notepad**.

    > [!NOTE] We'll need this key later on.

### B) Import and extend the LUIS model

Before calling LUIS, we need to train it with the kinds of phrases we expect our users to use.

1. [] Login to the **LUIS portal** (++www.luis.ai++).

    > [!NOTE] Use the same credentials as you used for logging into Azure.
1. [] **Scroll down** to the bottom of the welcome page.
1. [] Click **Create an app**.
1. [] Select **United States** from the country list.
1. [] Check the **I agree** checkbox.
1. [] Click the **Continue** button.
1. [] From `My Apps`, click **Import new app**. 
1. [] **Select** the base model from `Downloads\botframeworklab\Resources\build-bot.json`.
1. [] Click on the **Done** button.
1. [] **Wait** for the import to complete.
1. [] Click on the **Train** button and wait for it to finish.
1. [] Click the **Test** button to open the test panel.
1. [] **Type** ++Hello++ and press enter.

    > [!NOTE] It should return the `Greetings` intent.
1. [] Click the **Test** button in the top right to close the test panel.
1. [] Add a **new intent**:
    * Click on **Intents**.
    * Click on **Create new intent**.
    * Type the new intent name: ++GetPriceEstimate++
    * Click **Done**.
1. [] Add a new **utterance**:
    * Type: ++how much does it cost to get to the airport?++
    * Press **Enter**.
1. [] Add another new **utterance**:
    * Type: ++could you give me a price estimate for a ride to the airport?++
    * Press **Enter**.
1. [] **Test** your new intent:
    * Click on the **Train** button and wait for it to finish.
    * Click on **Test** button.
    * Type the following test utterance: ++how much does it cost for a cab to the airport?++
    * Press **Enter**.

        > [!NOTE] The test should return the `GetPriceEstimate` intent.
1. [] Publish your application:
    * Go to the **Publish** tab.
    * Click **Add key**. You'll need to scroll down to find the button.
    * Select the only **tenant**.
    * Select the only **subscription name**.
    * Select the **key** that you created before.
    * Click on **Add Key**.
    * Click on the **Publish** button next to the *Production* slot.
    * Wait for the process to finish.
1. [] Go to the **Settings** tab.
1. [] **Copy** the LUIS *Application ID* to Notepad.

    > [!NOTE] We'll need this app ID later on.

### C) Add LUIS middleware to your bot

Like all of the Cognitive Services, LUIS is accessible via a RESTful endpoint. However, the Bot Builder SDK has an inbuilt middleware component we can use to simplify this integration. This transparently calls LUIS before invoking our code, allowing our code to focus on processing the user's intent rather than natural language.

1. [] In **Visual Studio**, open **Startup.cs**
1. [] **Add** the LUIS middleware to the `ConfigureServices` method *after* the exception handling code:

    ```cs
    options.Middleware.Add(
        new LuisRecognizerMiddleware(
            new LuisModel(
                "<your_luis_app_id>",
                "<your_luis_subscription_key>",
                new Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/")),
            null, new LuisOptions
            {
                TimezoneOffset = -7
            }
        ));
    ```

    > [!ALERT] Make sure you replace `<your_luis_app_id>` and `<your_luis_subscription_key>` with the values you captured in Notepad earlier in the lab.
1. [] Open **CarsBot.cs** in the **Bots** folder.
1. [] **Replace** the contents of the `case ActivityTypes.Message:` case with the following code:

    ```cs
    if (!turnContext.Responded)
    {
        var result = turnContext.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);
        var topIntent = result?.GetTopScoringIntent();
        
        // Your code goes here
    }
    break;
    ```

    > [!NOTE] The first step is to extract the LUIS *intent* from the context. This is populated by the middleware.
1. [] **Add** the following code snippet where indicated:

    ```cs
    switch (topIntent != null ? topIntent.Value.intent : null)
    {
        case LuisIntents.Greetings:
            await turnContext.SendActivity("Hello! I am your ride sharing assistant.");
            break;
        case LuisIntents.GetPriceEstimate:
            var location = (string)result.Entities[LuisEntities.Location]?.First;
            GetPriceEstimateHandler(turnContext, location);
            break;
        default:
            await turnContext.SendActivity("Sorry, I didn't understand that.");
            break;
    }
    ```
]
    > [!NOTE] This switch will send the user's message to the right handler based on the LUIS intent.
1. [] Put a **breakpoint** in the `LuisIntents.GetPriceEstimate` switch case.

### D) Test LUIS configuration

Let's run the bot to see LUIS in action.

1. [] Run the app by clicking on the **IIS Express** button in Visual Studio (with the green play icon).
1. [] Return to the **botframework-emulator**.
1. [] Click the **refresh** button to start a new conversation.
1. [] **Type** ++how much does it cost to get to the airport?++ and press enter.
1. [] Return to **Visual Studio** and wait for the breakpoint to be hit.
1. [] **Mouse over** the `location` variable to inspect its content and see the data provided by LUIS.
1. [] Press **Continue** in the toolbar.
1. [] **Remove** the breakpoint.
1. [] Return to the **Bot Emulator** to see the response from the bot.
1. [] **Stop** debugging by clicking the stop button in the toolbar.

## Using the Bot Framework SDK

### A) Add a pre-built entity

In this section, we'll configure the conversation flow that will allow us to create a reservation. One of the most important parts of this flow will be to specify the pick up time. LUIS includes a set of prebuilt entities that make processing complex concepts like dates and times easier. When a prebuilt entity is included in your application, its predictions are included in your published application.

1. [] **Return** to the **LUIS portal** (++www.luis.ai++).
1. [] Click on **My Apps**.
1. [] **Select** your recently created app.
1. [] Click on the **Build** tab.
1. [] Add a prebuilt entity to handle date times:
    * Click **Entities** in the left menu.
    * Click on **Manage prebuilt entities**.
    * **Type** ++datetimeV2++ in the search box.
    * Mark the **checkbox** next to the result.
    * Click **Done**.
1. [] Click **Train**.
1. [] Modify intent to detect dates:
    * Click **Intents** in the left menu.
    * Click on **ScheduleRide**.
    * **Type** a new utterance: ++I need a car for May 5th at 3:00pm++
    * Press **Enter**.

        > [!NOTE] In the list, `May 5th at 3:00pm` will be automatically recognized as `datetimeV2`.
    * Type a new utterance: ++I need a ride for tomorrow at 7 am++

        > [!NOTE] In the list, `tomorrow at 7 am` will be automatically recognized as `datetimeV2`.
1. [] Click on the **Train** button.
1. [] **Wait** for it to finish.
1. [] Go to the **Publish** tab.
1. [] Click on the **Publish** button next to the *Production* slot.

### B) Setup the conversation flow

Now that our LUIS model is able to process dates and times, we'll finish the implementation by using the SDK to query the vehicle type, confirm surge pricing if applicable, and ask any clarifying questions. And we'll skip questions if the user has already provided the information as part of their initial utterance.

1. [] Open **CarsBot.cs** in the **Bots** folder.
1. [] **Modify** the `OnTurn` method by adding the following lines to the beginning of the method:

    ```cs
    var state = turnContext.GetConversationState<ReservationData>();
    var dialogContext = dialogs.CreateContext(turnContext, state);
    await dialogContext.Continue();
    ```

    Also add the following case to the switch statement, before the **default** case:

    ```cs
    case LuisIntents.ScheduleRide:
        var carType = (string)result.Entities[LuisEntities.CarType]?.First;
        location = (string)result.Entities[LuisEntities.Location]?.First;
        var time = GetTimeValueFromResult(result);
        ScheduleRideHandler(turnContext, carType, location, time);
        break;
    ```

1. [] **Modify** the `GetTimeValueFromResult` method by adding the following lines before the return statement:

    ```cs
    var timex = (string)result.Entities["builtin_datetime"]?.First["timex"].First;
    if (timex != null)
    {
        timex = timex.Contains(":") ? timex : $"{timex}:00";
        return DateTime.Parse(timex).ToString("MMMM dd HH:mm tt");
    }
    ```

    > [!NOTE] The time returned by Luis contains the datetime as a string in a property called 'timex'.

1. [] **Add** the `ScheduleRideHandler` method to start the conversation flow for the schedule ride intent:

    ```cs
    private async void ScheduleRideHandler(ITurnContext turnContext, string carType, string location, string time)
    {
        var state = turnContext.GetConversationState<ReservationData>();
        var dialogContext = dialogs.CreateContext(turnContext, state);
        state.CarType = carType;
        state.Location = location;
        state.Time = time;
        await dialogContext.Begin(PromptStep.GatherInfo);
    }
    ```

    > [!NOTE] The method invokes the `WaterfallStep` dialog created in the constructor by referencing it by name (`GatherInfo`).
1. [] **Add** the following constructor:

    ```cs
    public CarsBot()
    {
        dialogs = new DialogSet();
        dialogs.Add(PromptStep.CarTypePrompt, new TextPrompt());
        dialogs.Add(PromptStep.TimePrompt, new TextPrompt());
        dialogs.Add(PromptStep.LocationPrompt, new TextPrompt(LocationValidator));
        dialogs.Add(PromptStep.GatherInfo, new WaterfallStep[] { CarTypeStep, TimeStep, LocationStep, FinalStep });
    }
    ```

    > [!NOTE] This will setup the conversation flow passing the Luis results between the steps.

1. Put a breakpoint on the `await dialogContext.Begin(PromptStep.GatherInfo);` line in the `ScheduleRideHandler`.

### C) Test the conversation flow

Let's run the bot to see how LUIS processes the new conversation flow.

1. [] Run the app by clicking on the **IIS Express** button in Visual Studio (with the green play icon).
1. [] Return to the **botframework-emulator**.
1. [] Click the **refresh** button to start a new conversation.
1. [] Type ++I need a ride to the airport tomorrow at 9 am++ and press **enter**.
1. [] Return to **Visual Studio** and wait for the breakpoint to be hit.
1. [] **Mouse over** the `location`, `carType` and `time` variables to inspect their content.

    > [!NOTE] Only the location will have a value, as this is the only piece of information provided in our initial utterance.
1. [] Press **Continue** in the toolbar.
1. [] Return to the **botframework-emulator**.
1. [] The bot will display a list with the different car types available for your ride.
1. [] Type ++Sedan++ and press **enter**.
1. [] **Stop** debugging by clicking the stop button in the toolbar.
1. [] **Remove** the breakpoints previously added.

> [!NOTE] At this point the conversation flow will be finished.

### D) Add a visual response to your bot: Carousel

Bots are capable of interacting with users through more than just text-based chat. The imported LUIS app comes with a *CarTypeStep* intent that allows customers to choose between the different types of vehicles available for hire. Currently, this method is returning the options as a simple text. Let's modify it and return a carousel.

1. [] Open **CarsBot.cs** in the **Bots** folder.
1. [] Modify the `CarsBot` constructor method, replace this line
    ```cs
    dialogs.Add(PromptStep.CarTypePrompt, new TextPrompt());
    ```

    with
    ```cs
    dialogs.Add(PromptStep.CarTypePrompt, new ChoicePrompt(Culture.English));
    ```
1. [] **Add** the following code to the `CarTypeStep` method where indicated to prepare the three options:

    ```cs
    var actions = new[]
    {
        new CardAction(type: ActionTypes.ImBack, title: "Sedan", value: "Sedan"),
        new CardAction(type: ActionTypes.ImBack, title: "SUV", value: "SUV"),
        new CardAction(type: ActionTypes.ImBack, title: "Sports car", value: "Sports car")
    };
    ```
1. [] **Add** the final piece of code to create the hero card and send it:

    ```cs
    var heroCard = new HeroCard(buttons: actions);
    var activity = (Activity)MessageFactory.Carousel(new[] {heroCard.ToAttachment()}, "Please select a car type.");
    var choices = actions.Select(x => new Choice { Action = x, Value = (string)x.Value }).ToList();
    await dialogContext.Prompt(PromptStep.CarTypePrompt, activity, new ChoicePromptOptions { Choices = choices });
    ```
1. [] **Remove** the previous text response from the method:

    ```cs
    await dialogContext.Context.SendActivity("What kind of vehicle would you like?");
    await dialogContext.Prompt(PromptStep.CarTypePrompt, $"Available options are: {string.Join(", ", BotConstants.CarTypes)}");
    ```
1. [] Modify the `TimeStep` method, replace this line
    ```cs
    state.CarType = (result as TextResult).Value;
    ```

    with
    ```cs
    state.CarType = ((ChoiceResult)result).Value.Value;
    ```

### E) [Optional] Add another visual response to your bot: Receipt

LUIS provides advanced card types to provide rich interactivity. Here, we'll use a specialized card type to display a receipt to the user after the ride is confirmed. The `GetReceiptCard` method creates a receipt as an attachment, it only has an empty receipt with a title. Let's modify it and complete our receipt details.

1. [] Open **CarsBot.cs** in the **Bots** folder.
1. [] **Modify** the `GetReceiptCard` method to add some general information right after the receipt `Title`:

    ```cs
    Facts = new List<Fact>
    {
        new Fact("Booking Number", "000345"),
        new Fact("Customer", "Lance Olson"),
        new Fact("Payment Method", "VISA 5555-****")
    },
    ```

    > [!NOTE] The facts allow adding general information to the receipt in a form of key/value pairs.

1. [] **Add** the items that will be charged to your receipt, including the total amount and taxes:

    ```cs
    Items = new List<ReceiptItem>
    {
        new ReceiptItem(title: reservation.CarType, subtitle: pickupTime, price: price, quantity: "1", image: new CardImage(BotConstants.CarImageUrl))
    },
    Tax = "$ 5.50",
    Total = price,
    ```
1. [] **Add** a button to provide access to more information about pricing:

    ```cs
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
    ```
1. [] **Update** the `FinalStep` method to attach the receipt to the response by adding this snippet where indicated:

    ```cs
    var receipt = await GetReceiptCard(state);
    var message = MessageFactory.Attachment(receipt);
    await dialogContext.Context.SendActivity(message);
    ```

### F) Test the advanced visual responses

Let's run the bot and try a new utterance to see the conversation flow in action along the new visual responses that we included.

1. [] Run the app by clicking on the **IIS Express** button in Visual Studio (with the green play icon).
1. [] Return to the **botframework-emulator**.
1. [] Click the **refresh** button to start a new conversation.
1. [] **Type** ++I need a ride to the airport tomorrow at 9:30 am++ and press enter.
1. [] Return to **Visual Studio** and wait for the breakpoint to be hit.
1. [] Debugger will stop in the first step `CarTypeStep`, where we added the new visual response to display the options.
1. [] Press **Continue** in the toolbar.
1. [] Return to the **botframework-emulator**.
1. [] The bot will display a list with the different car types available for your ride, this time the options are displayed in a nice list with buttons. Click `Sedan`.
1. [] Return to **Visual Studio** and wait for the breakpoint to be hit.
1. [] The Debugger will stop in the `FinalStep` where we added the new receipt response.

    > [!NOTE] Notice that the debugger didn't stop in the `LocationStep` and `TimeStep` as they were provided in the first utterance.
1. [] Press **Continue** in the toolbar.
1. [] Return to the **botframework-emulator**.
1. [] The bot will display the receipt card.
1. [] **Remove** the breakpoints previously added.
1. [] **Stop** debugging by clicking the stop button in the toolbar.

## [Optional] Use Cosmos DB to store the bot state

While in-memory storage is great for development, it's less useful when bot conversations are long running or your bot is hosted on multiple web front-ends. To resolve this issue, the Bot Builder SDK provides support for a number of different state storage services including Cosmos DB, Table Storage, and Blob Storage.

1. [] Open **Startup.cs**.
1. [] **Add** the following import:
    ```cs
    using Microsoft.Bot.Builder.Azure;
    ```
1. [] **Modify** the `ConfigureServices` method and replace:

    ```cs
    options.Middleware.Add(new ConversationState<ReservationData>(new MemoryStorage()));
    ```

    with:
    
    ```cs
    var dataStore = new CosmosDbStorage(
        new Uri("https://<your_cosmos_instance_name>.documents.azure.com:443/"),
        "<your_cosmos_instance_key>",
        "botdb",
        "botcollection");
    options.Middleware.Add(new ConversationState<ReservationData>(dataStore));
    ```

    > [!NOTE] In the interests of time, we've pre-provisioned a Cosmos DB instance.

## [Optional] Making your bot multi-channel

### A) Publish back to Azure

Now that our development efforts are complete, it's time to publish the bot to Azure.

1. [] Right Click  the `BotService` project.
1. [] Click **Publish**.
1. [] Click **Publish** again.
1. [] **Wait** for the deployment to complete. This operation might take a few minutes.
1. [] Return to your **Bot Service Web App** in the **Azure Portal**.
1. [] Go to **Test in Web Chat**.
1. [] **Type** ++I need a ride to the airport++ and press enter.

    > [!NOTE] The bot should reply with the choice prompt previously added.

### B) Registering our bot with Telegram

One of the great strengths of the Bot Framework is its ability to make supporting different bot channels easier. In this section, you'll add a Telegram channel to your bot.

> [!ALERT] You'll need a personal Telegram account for this section of the lab.

1. [] Open **Telegram** from the **Start menu**.

    > [!NOTE] If this is your first time using Telegram you will have to create an account.
1. [] Click **Contacts**.
1. [] In the **Search** box, **type**: ++@BotFather++
1. [] Click **Start**.
1. [] **Type** ++/newbot++ and press **Enter** to send the message.
1. [] Provide a **name** for your bot (e.g. **BuildLabs<your_initials>Bot**).
1. [] Press **Enter**.
1. [] Provide a **username** for your bot (e.g. **BuildLabs<your_initials>Bot**).
1. [] Press **Enter**.

    > [!NOTE] BotFather will display a success message once your new bot is created along a token to access the HTTP API.
1. [] Copy the **token** as you will need it later.
1. [] **Return** to your **Web App Bot** in the **Azure Portal** (++portal.azure.com++).
1. [] Click **Channels**.
1. [] Select **Telegram**.
1. [] **Paste** the token obtained from Telegram.
1. [] Click **Save**.

### C) See our bot in Telegram

Let's re-deploy our bot and see this new functionality in action in Telegram.

1. [] **Repeat** the steps in the earlier *Publish back to Azure* section.
1. [] **Return** to the Telegram app.
1. [] Click on the **link** provided for your new bot.
1. [] Click **Start** to start a conversation.
1. [] **Type** ++how much does it cost for a ride to the airport?++.
1. [] **Continue** the flow to see the sticker displayed by the bot (after confirming the ride).

## Conclusion

Today you've built a bot using the new Bot Builder V4 SDK along with some of the most useful bot-related Azure services. You've see how to run the bot locally, deploy it to the cloud, and handle hard problems like natural language utterances, advanced visualization, and production hosting. Make sure to stay tuned to the [Bot Framework Blog](https://blog.botframework.com/) (https://blog.botframework.com/) to learn more as the SDK evolves towards GA!
