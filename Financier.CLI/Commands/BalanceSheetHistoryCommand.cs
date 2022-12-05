using Financier.CLI.Services;
using Financier.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Financier.CLI.Commands
{
    public class BalanceSheetHistoryCommand
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
                serviceCollection.AddSingleton<IBalanceSheetHistoryWriterService, BalanceSheetHistoryConsoleWriterService>();

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                var balanceSheets = new Dictionary<DateTime, BalanceSheet>();
                BalanceSheet balanceSheet = null;
                IBalanceSheetService balanceSheetService = serviceProvider.GetRequiredService<IBalanceSheetService>();
                DateTime cursor = DateTime.Now;
                do
                {
                    balanceSheet = balanceSheetService.Generate(cursor);
                    balanceSheets.Add(cursor, balanceSheet);
                    cursor = cursor.Subtract(TimeSpan.FromDays(14));
                } while (balanceSheet != null && balanceSheet.NetWorth != 0);

                IBalanceSheetHistoryWriterService balanceSheetHistoryWriterService = serviceProvider.GetRequiredService<IBalanceSheetHistoryWriterService>();
                balanceSheetHistoryWriterService.Write(balanceSheets);

                return 0;
            });
        }
    }
}
