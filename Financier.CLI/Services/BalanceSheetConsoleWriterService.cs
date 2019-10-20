using System;
using System.Linq;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.CLI.Services
{
    public class BalanceSheetConsoleWriterService : IBalanceSheetWriterService
    {
        private ILogger<BalanceSheetConsoleWriterService> m_logger;
        private IEnvironmentService m_environmentService;

        public BalanceSheetConsoleWriterService(
            ILoggerFactory loggerFactory,
            IEnvironmentService environmentService)
        {
            m_logger = loggerFactory.CreateLogger<BalanceSheetConsoleWriterService>();
            m_environmentService = environmentService;
        }

        public void Write(BalanceSheet balanceSheet, DateTime at)
        {
            Console.WriteLine($"Balance sheet at {at}");
            Console.WriteLine();

            Console.WriteLine("Assets");
            foreach (BalanceSheetItem item in balanceSheet.Assets.OrderByDescending(a => a.Balance))
            {
                Console.WriteLine("    {0,-50} {1} {2,10}", item.Name, balanceSheet.CurrencySymbol, item.Balance);
            }
            Console.WriteLine("                                                       ============");
            Console.WriteLine("                                                       {0} {1,10}", balanceSheet.CurrencySymbol, balanceSheet.TotalAssets);
            Console.WriteLine("                                                       ============");
            Console.WriteLine();

            Console.WriteLine("Liabilities");
            foreach (BalanceSheetItem item in balanceSheet.Liabilities.OrderBy(a => a.Balance))
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
