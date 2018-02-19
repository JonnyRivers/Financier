using Financier.Services;
using System;
using System.Linq;

namespace Financier.CLI.Services
{
    public class CashflowStatementConsoleWriterService : ICashflowStatementWriterService
    {
        public void Write(CashflowStatement cashflowStatement)
        {
            Console.WriteLine($"Cashflow Statement from {cashflowStatement.StartAt} to {cashflowStatement.EndAt}");
            Console.WriteLine();
            Console.WriteLine("Name                                     Inflow     Outflow    Cashflow");
            Console.WriteLine("---------------------------------------- ---------- ---------- ----------");
            foreach (CashflowStatementItem item in cashflowStatement.Items.OrderBy(i => i.Name))
            {
                Console.WriteLine("{0,-40} {1,10} {2,10} {3,10}", 
                    item.Name, item.Inflow, item.Outflow, item.Cashflow);
            }

            Console.WriteLine("---------------------------------------- ---------- ---------- ==========");
            Console.WriteLine("Total                                    {0,10} {1,10} {2,10}",
                cashflowStatement.TotalInflow, cashflowStatement.TotalOutflow, cashflowStatement.NetCashflow);
            Console.WriteLine("---------------------------------------- ---------- ---------- ==========");
        }
    }
}
