﻿using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Financier.CLI.Services
{
    public class CashflowStatementConsoleWriterService : ICashflowStatementWriterService
    {
        private ILogger<CashflowStatementConsoleWriterService> m_logger;
        private IEnvironmentService m_environmentService;

        public CashflowStatementConsoleWriterService(
            ILoggerFactory loggerFactory,
            IEnvironmentService environmentService)
        {
            m_logger = loggerFactory.CreateLogger<CashflowStatementConsoleWriterService>();
            m_environmentService = environmentService;
        }

        public void Write(CashflowStatement cashflowStatement)
        {
            Console.WriteLine($"Cashflow Statement from {cashflowStatement.StartAt} to {cashflowStatement.EndAt}");
            Console.WriteLine();
            foreach(CashflowAccount account in cashflowStatement.Accounts.OrderBy(a => a.Name))
            {
                if (account.Inflow == 0 && account.Outflow == 0)
                    continue;

                Console.WriteLine($"{account.Name}");
                Console.WriteLine();
                Console.WriteLine("Period           Inflow     Outflow    Cashflow");
                Console.WriteLine("---------------- ---------- ---------- ----------");
                foreach (CashflowAccountPeriod period in account.Periods.OrderBy(i => i.Range.Start))
                {
                    Console.WriteLine("{0,-16} {1,10} {2,10} {3,10}",
                        period.Range.Start.ToShortDateString(), period.Inflow, period.Outflow, period.Cashflow);
                }

                Console.WriteLine("---------------- ---------- ---------- ==========");
                Console.WriteLine("Total            {0,10} {1,10} {2,10}",
                    account.Inflow, account.Outflow, account.Cashflow);
                Console.WriteLine("---------------- ---------- ---------- ==========");
                Console.WriteLine();
                Console.WriteLine();
            }

            // TODO: statement totals
        }
    }
}
