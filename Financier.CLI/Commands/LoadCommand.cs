using Financier.Services;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier.CLI.Commands
{
    public class LoadCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Populate the database from a file";

            CommandOption databaseOption = command.Option(
                "-d|--database",
                "The name of the database to use",
                CommandOptionType.SingleValue);

            CommandOption pathOption = command.Option(
                "-p|--path",
                "The path to the file to load from",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                string databaseName = databaseOption.Value();
                var serviceCollection = ServiceCollectionSetup.SetupCoreServices(databaseName);

                // Application services
                serviceCollection.AddTransient<IDatabaseSerializationService, DatabaseSerializationXmlService>();

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                string path = pathOption.Value();
                IDatabaseSerializationService databaseCreationService = serviceProvider.GetRequiredService<IDatabaseSerializationService>();
                databaseCreationService.Load(path);

                return 0;
            });
        }
    }
}
