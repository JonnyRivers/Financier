using Financier.Services;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier.CLI.Commands
{
    public class ObliterateCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Ensure the database is obliterated";

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
                databaseCreationService.Obliterate();

                return 0;
            });
        }
    }
}
