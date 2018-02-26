using Financier.Services;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier.CLI.Commands
{
    public class SaveCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Dump database contents to a file";

            CommandOption pathOption = command.Option(
                "-p|--path",
                "The path to the file to save to",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                var serviceCollection = ServiceCollectionSetup.SetupCoreServices();

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
