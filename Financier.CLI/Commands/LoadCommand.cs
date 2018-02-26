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

            CommandOption pathOption = command.Option(
                "-p|--path",
                "The path to the file to load from",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                var serviceCollection = ServiceCollectionSetup.SetupCoreServices();

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
