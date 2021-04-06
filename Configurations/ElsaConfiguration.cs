using Elsa.Activities.Email.Extensions;
using Elsa.Activities.Http.Extensions;
using Elsa.Activities.Timers.Extensions;
using Elsa.Dashboard.Extensions;
using Elsa.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelusHeathPack.Extensions;
using TelusHeathPack.Handlers;

namespace TelusHeathPack.Configurations
{
    public class ElsaConfiguration
    {
        public static void ConfigureElsa(IServiceCollection services, IConfiguration configuration)
        {
            services.AddServerSideBlazor();

            services
                // Add Elsa services
                .AddElsa(
                    elsa =>
                    {
                        // Configure Elsa to use MongoDB provider
                        // elsa.AddMongoDbStores(configuration, databaseName: "UserRegistration", connectionStringName: "MongoDB");
                    })
                
                // Add Elsa dashboard services
                .AddElsaDashboard()
                
                // Add the activities that will be used
                .AddEmailActivities(options => options.Bind(configuration.GetSection("Elsa:Smtp")))
                .AddHttpActivities(options => options.Bind(configuration.GetSection("Elsa:Http")))
                .AddTimerActivities(options => options.Bind(configuration.GetSection("Elsa:Timers")))
                .AddUserActivities()
                // Add a MongoDB collection
               // .AddMongoDbCollection<User>("Users")
                
                // Add liquid handler
                .AddNotificationHandlers(typeof(LiquidConfigurationHandler));
        }
    }
}