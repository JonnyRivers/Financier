using Financier.Services;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier.CLI.Commands
{
    public class CreateCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Ensure the database is created";

            command.OnExecute(() =>
            {
                var serviceCollection = ServiceCollectionSetup.SetupCoreServices();                

                // Application services
                serviceCollection.AddTransient<IDatabaseCreationService, DatabaseCreationService>();

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                IDatabaseCreationService databaseCreationService = serviceProvider.GetRequiredService<IDatabaseCreationService>();
                databaseCreationService.Create();

                return 0;
            });
        }
    }
}
