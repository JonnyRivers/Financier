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

            CommandOption atOption = command.Option(
                "-a|--at",
                "The time to prepare a balance sheet for",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                var serviceCollection = ServiceCollectionSetup.SetupCoreServices();

                // Application services
                serviceCollection.AddSingleton<IAccountService, AccountService>();
                serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
                serviceCollection.AddSingleton<IBalanceSheetService, BalanceSheetService>();
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
