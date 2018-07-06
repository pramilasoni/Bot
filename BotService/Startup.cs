using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Core.Extensions;
using System;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Build.Labs.BotFramework.Bots;
using Build.Labs.BotFramework.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.TraceExtensions;

namespace Build.Labs.BotFramework
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => Configuration);
            services.AddBot<CarsBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);
                options.Middleware.Add(new ConversationState<ReservationData>(new MemoryStorage()));
              
                options.Middleware.Add(
                    new LuisRecognizerMiddleware(
                        new LuisModel(
                            "3b28f6ac-8efd-4b31-9038-ea329e542138",
                            "cd68e16ff8f24654835333a347ace810",
                            new Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/")),
                        null, new LuisOptions
                        {
                            TimezoneOffset = -7
                        }
                    ));
                options.Middleware.Add(new CatchExceptionMiddleware<Exception>(async (context, exception) =>
                {
                    await context.TraceActivity("CarsBot exception", exception);
                    await context.SendActivity("Oops! It looks like something went wrong!");
                }));

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseBotFramework();
        }
    }
}