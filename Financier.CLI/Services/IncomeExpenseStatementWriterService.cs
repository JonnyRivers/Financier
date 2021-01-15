using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.IO;
using System.Text;

namespace Financier.CLI.Services
{
    public class IncomeExpenseStatementWriterService : IIncomeExpenseStatementWriterService
    {
        private ILogger<IncomeExpenseStatementWriterService> m_logger;

        public IncomeExpenseStatementWriterService(ILoggerFactory loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<IncomeExpenseStatementWriterService>();
        }

        public void Write(IncomeExpenseStatement incomeExpenseStatement, string outputPath)
        {
            Console.WriteLine($"Income Expense Statement from {incomeExpenseStatement.StartAt} to {incomeExpenseStatement.EndAt}");
            Console.WriteLine();
            
            using(StreamWriter writer = new StreamWriter(outputPath))
            {
                StringBuilder header = new StringBuilder();
                header.Append("AccountName");
                foreach (DateTimeRange dateTimeRange in incomeExpenseStatement.DateTimeRanges)
                {
                    if (incomeExpenseStatement.Period == IncomeExpensePeriod.Quarterly)
                    {
                        int quarterIndex = (dateTimeRange.Start.Month - 1) / 3;
                        header.Append($",Q{quarterIndex + 1} {dateTimeRange.Start.Year}");
                    }
                }
                writer.WriteLine(header.ToString());

                foreach (IncomeExpenseAccount account in incomeExpenseStatement.IncomeAccounts.OrderBy(a => a.Name))
                {
                    if (account.Periods.All(p => p.Total == 0))
                        continue;

                    StringBuilder row = new StringBuilder();
                    row.Append(account.Name);
                    foreach (IncomeExpenseAccountPeriod period in account.Periods.OrderBy(i => i.Range.Start))
                    {
                        row.Append($",{period.Total}");
                    }
                    writer.WriteLine(row.ToString());
                }

                writer.WriteLine();
                writer.WriteLine(header.ToString());
                foreach (IncomeExpenseAccount account in incomeExpenseStatement.ExpenseAccounts.OrderBy(a => a.Name))
                {
                    if (account.Periods.All(p => p.Total == 0))
                        continue;

                    StringBuilder row = new StringBuilder();
                    row.Append(account.Name);
                    foreach (IncomeExpenseAccountPeriod period in account.Periods.OrderBy(i => i.Range.Start))
                    {
                        row.Append($",{period.Total}");
                    }
                    writer.WriteLine(row.ToString());
                }
            }
        }
    }
}
