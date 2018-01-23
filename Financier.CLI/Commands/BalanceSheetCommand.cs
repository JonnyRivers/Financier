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

            CommandOption databaseOption = command.Option(
                "-d|--database",
                "The name of the database to use",
                CommandOptionType.SingleValue);

            CommandOption atOption = command.Option(
                "-a|--at",
                "The time to prepare a balance sheet for",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                string databaseName = databaseOption.Value();
                var serviceCollection = ServiceCollectionSetup.SetupCoreServices(databaseName);

                // Application services
                serviceCollection.AddSingleton<IAccountService, AccountService>();
                serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
                serviceCollection.AddSingleton<IBalanceSheetService, BalanceSheetService>();

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                IBalanceSheetService balanceSheetService = serviceProvider.GetRequiredService<IBalanceSheetService>();
                DateTime at = DateTime.Parse(atOption.Value());
                BalanceSheet balanceSheet = balanceSheetService.Generate(at);
                DisplayBalanceSheet(balanceSheet, at);

                return 0;
            });
        }

        // TODO: Create a BalanceSheetConsoleWriterService to refactor Financier.CLI.Commands.BalanceSheetCommand
        // https://github.com/JonnyRivers/Financier/issues/14
        private static void DisplayBalanceSheet(BalanceSheet balanceSheet, DateTime at)
        {
            Console.WriteLine($"Balance sheet at {at}");
            Console.WriteLine();

            Console.WriteLine("Assets");
            foreach (BalanceSheetItem item in balanceSheet.Assets)
            {
                Console.WriteLine("    {0,-50} {1} {2,10}", item.Name, balanceSheet.CurrencySymbol, item.Balance);
            }
            Console.WriteLine("                                                       ============");
            Console.WriteLine("                                                       {0} {1,10}", balanceSheet.CurrencySymbol, balanceSheet.TotalAssets);
            Console.WriteLine("                                                       ============");
            Console.WriteLine();

            Console.WriteLine("Liabilities");
            foreach (BalanceSheetItem item in balanceSheet.Liabilities)
            {
                Console.WriteLine("    {0,-50} {1} {2,10}", item.Name, balanceSheet.CurrencySymbol, item.Balance);
            }
            Console.WriteLine("                                                       ============");
            Console.WriteLine("                                                       {0} {1,10}", balanceSheet.CurrencySymbol, balanceSheet.TotalLiabilities);
            Console.WriteLine("                                                       ============");

            Console.WriteLine();
            Console.WriteLine("                                                       ============");
            Console.WriteLine("Net Worth                                              {0} {1,10}", balanceSheet.CurrencySymbol, balanceSheet.NetWorth);
            Console.WriteLine("                                                       ============");
        }
    }
}
