using Financier.Services;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier.CLI.Commands
{
    public class PopulateCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Populate the database";

            CommandOption databaseOption = command.Option(
                "-d|--database",
                "The name of the database to use",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                string databaseName = databaseOption.Value();
                var serviceCollection = ServiceCollectionSetup.SetupCoreServices(databaseName);

                // Application services
                serviceCollection.AddTransient<IDatabaseCreationService, DatabaseCreationService>();

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                IDatabaseCreationService databaseCreationService = serviceProvider.GetRequiredService<IDatabaseCreationService>();
                databaseCreationService.Populate();

                return 0;
            });
        }
    }
}
