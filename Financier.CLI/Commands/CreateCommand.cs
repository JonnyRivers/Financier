using Financier.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier.CLI.Commands
{
    public class CreateCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Ensure the database is created";

            CommandOption databaseConnectionOption = command.Option(
                "-d|--database",
                "The database connection to connect to",
                CommandOptionType.SingleValue);

            CommandOption passwordOption = command.Option(
                "-p|--password",
                "The password to connect with",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                string databaseConnectionName = databaseConnectionOption.Value();
                string password = passwordOption.HasValue() ? passwordOption.Value() : String.Empty;
                ServiceCollection serviceCollection = ServiceCollectionSetup.SetupCoreServices(
                    databaseConnectionName,
                    password
                );

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
