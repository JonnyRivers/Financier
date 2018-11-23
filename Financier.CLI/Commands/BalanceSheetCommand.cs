using Financier.CLI.Services;
using Financier.Services;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier.CLI.Commands
{
    public class BalanceSheetCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Generate a balance sheet from the database at a given time";

            CommandOption databaseConnectionOption = command.Option(
                "-d|--database",
                "The database connection to connect to",
                CommandOptionType.SingleValue);

            CommandOption passwordOption = command.Option(
                "-p|--password",
                "The password to connect with",
                CommandOptionType.SingleValue);

            CommandOption atOption = command.Option(
                "-a|--at",
                "The time to prepare a balance sheet for",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                string databaseConnectionName = databaseConnectionOption.Value();
                string password = passwordOption.HasValue() ? passwordOption.Value() : String.Empty;
                ServiceCollection serviceCollection = ServiceCollectionSetup.SetupCoreServices(
                    databaseConnectionName, 
                    password
                );

                // Financier.Core services
                serviceCollection.AddSingleton<IAccountService, AccountService>();
                serviceCollection.AddSingleton<IBalanceSheetService, BalanceSheetService>();
                serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
                serviceCollection.AddSingleton<IEnvironmentService, EnvironmentService>();

                // Application services
                serviceCollection.AddSingleton<IBalanceSheetWriterService, BalanceSheetConsoleWriterService>();

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                IBalanceSheetService balanceSheetService = serviceProvider.GetRequiredService<IBalanceSheetService>();
                DateTime at = DateTime.Parse(atOption.Value());
                BalanceSheet balanceSheet = balanceSheetService.Generate(at);

                IBalanceSheetWriterService balanceSheetWriterService = serviceProvider.GetRequiredService<IBalanceSheetWriterService>();
                balanceSheetWriterService.Write(balanceSheet, at);

                return 0;
            });
        }
    }
}
