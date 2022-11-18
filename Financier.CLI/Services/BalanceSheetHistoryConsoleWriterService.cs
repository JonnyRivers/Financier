using System;
using System.Collections.Generic;
using System.Linq;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.CLI.Services
{
    public class BalanceSheetHistoryConsoleWriterService : IBalanceSheetHistoryWriterService
    {
        private ILogger<BalanceSheetHistoryConsoleWriterService> m_logger;
        private IEnvironmentService m_environmentService;

        public BalanceSheetHistoryConsoleWriterService(
            ILoggerFactory loggerFactory,
            IEnvironmentService environmentService)
        {
            m_logger = loggerFactory.CreateLogger<BalanceSheetHistoryConsoleWriterService>();
            m_environmentService = environmentService;
        }

        public void Write(IDictionary<DateTime, BalanceSheet> balanceSheetsByDate)
        {
            Console.WriteLine("Date,NetWorth");

            foreach (KeyValuePair<DateTime, BalanceSheet> DateBalanceSheetPair in balanceSheetsByDate.OrderBy(kvp => kvp.Key))
            { 
                Console.WriteLine($"{DateBalanceSheetPair.Key}, {DateBalanceSheetPair.Value.NetWorth}");
            }
        }
    }
}
