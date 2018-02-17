using Financier.CLI.Services;
using Financier.Services;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier.CLI.Commands
{
    public class CashflowCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Generate a balance sheet from the database at a given time";

            CommandOption databaseOption = command.Option(
                "-d|--database",
                "The name of the database to use",
                CommandOptionType.SingleValue);

            CommandOption startAtOption = command.Option(
                "-s|--startAt",
                "The start of the cashflow period",
                CommandOptionType.SingleValue);

            CommandOption endAtOption = command.Option(
                "-e|--endAt",
                "The end of the cashflow period",
                CommandOptionType.SingleValue);

            command.OnExecute(() =>
            {
                string databaseName = databaseOption.Value();
                var serviceCollection = ServiceCollectionSetup.SetupCoreServices(databaseName);

                // Application services
                serviceCollection.AddSingleton<IAccountService, AccountService>();
                serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
                serviceCollection.AddSingleton<ICashflowService, CashflowService>();
                serviceCollection.AddSingleton<ICashflowStatementWriterService, CashflowStatementConsoleWriterService>();

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                ICashflowService cashflowService = serviceProvider.GetRequiredService<ICashflowService>();
                DateTime startAt = DateTime.Parse(startAtOption.Value());
                DateTime endAt = DateTime.Parse(endAtOption.Value());
                CashflowStatement cashflowStatement = cashflowService.Generate(startAt, endAt);

                ICashflowStatementWriterService cashflowStatementWriterService = 
                    serviceProvider.GetRequiredService<ICashflowStatementWriterService>();
                cashflowStatementWriterService.Write(cashflowStatement);

                return 0;
            });
        }
    }
}
