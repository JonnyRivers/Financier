﻿using Financier.CLI.Services;
using Financier.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier.CLI.Commands
{
    public class CashflowCommand
    {
        public static void Configure(CommandLineApplication command)
        {
            command.Description = "Generate a cashflow statement from the database for a given period";

            CommandOption databaseConnectionOption = command.Option(
                "-d|--database",
                "The database connection to connect to",
                CommandOptionType.SingleValue);

            CommandOption passwordOption = command.Option(
                "-p|--password",
                "The password to connect with",
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
                string databaseConnectionName = databaseConnectionOption.Value();
                string password = passwordOption.HasValue() ? passwordOption.Value() : String.Empty;
                ServiceCollection serviceCollection = ServiceCollectionSetup.SetupCoreServices(
                    databaseConnectionName,
                    password
                );

                // Financier.Core services
                serviceCollection.AddSingleton<IAccountRelationshipService, AccountRelationshipService>();
                serviceCollection.AddSingleton<IAccountService, AccountService>();
                serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
                serviceCollection.AddSingleton<ICashflowService, CashflowService>();
                serviceCollection.AddSingleton<IEnvironmentService, EnvironmentService>();
                serviceCollection.AddSingleton<ITransactionService, TransactionService>();

                // Application services
                serviceCollection.AddSingleton<ICashflowStatementWriterService, CashflowStatementConsoleWriterService>();

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                ICashflowService cashflowService = serviceProvider.GetRequiredService<ICashflowService>();
                DateTime startAt = DateTime.Parse(startAtOption.Value());
                DateTime endAt = DateTime.Parse(endAtOption.Value());
                CashflowStatement cashflowStatement = cashflowService.Generate(CashflowPeriod.Fortnightly, startAt, endAt);

                ICashflowStatementWriterService cashflowStatementWriterService = 
                    serviceProvider.GetRequiredService<ICashflowStatementWriterService>();
                cashflowStatementWriterService.Write(cashflowStatement);

                return 0;
            });
        }
    }
}
