using Financier.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier.CLI.Commands
{
    public class SaveCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Dump database contents to a file";

            CommandOption databaseConnectionOption = command.Option(
                "-d|--database",
                "The database connection to connect to",
                CommandOptionType.SingleValue);

            CommandOption passwordOption = command.Option(
                "-p|--password",
                "The password to connect with",
                CommandOptionType.SingleValue);

            CommandOption pathOption = command.Option(
                "-p|--path",
                "The path to the file to save to",
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
                serviceCollection.AddTransient<IDatabaseSerializationService, DatabaseSerializationXmlService>();

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                string path = pathOption.Value();
                IDatabaseSerializationService databaseCreationService = serviceProvider.GetRequiredService<IDatabaseSerializationService>();
                databaseCreationService.Save(path);

                return 0;
            });
        }
    }
}
