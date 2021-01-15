using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    public class IncomeExpenseService : IIncomeExpenseService
    {
        ILogger<IncomeExpenseService> m_logger;
        IAccountService m_accountService;
        ICurrencyService m_currencyService;
        ITransactionService m_transactionService;

        public IncomeExpenseService(
            ILoggerFactory loggerFactory,
            IAccountRelationshipService accountRelationshipService,
            IAccountService accountService,
            ICurrencyService currencyService,
            ITransactionService transactionService)
        {
            m_logger = loggerFactory.CreateLogger<IncomeExpenseService>();
            m_accountService = accountService;
            m_currencyService = currencyService;
            m_transactionService = transactionService;
        }

        private static DateTimeRange DateTimeAndPeriodToDateTimeRange(DateTime startAt, IncomeExpensePeriod period)
        {
            if(period == IncomeExpensePeriod.Quarterly)
            {
                int quarterIndex = (startAt.Month - 1) / 3;
                int monthIndex = (quarterIndex * 3) + 1;
                DateTime periodStart = new DateTime(startAt.Year, monthIndex, 1);
                DateTime periodEnd = periodStart.AddMonths(3);

                return new DateTimeRange(periodStart, periodEnd);
            }

            throw new ArgumentException($"Unrecognized IncomeExpensePeriod {period}", nameof(period));
        }

        private static DateTimeRange NextDateTimeRange(DateTimeRange dateTimeRange, IncomeExpensePeriod period)
        {
            if (period == IncomeExpensePeriod.Quarterly)
            {
                return new DateTimeRange(dateTimeRange.Start.AddMonths(3), dateTimeRange.End.AddMonths(3));
            }

            throw new ArgumentException($"Unrecognized IncomeExpensePeriod {period}", nameof(period));
        }

        private static IEnumerable<DateTimeRange> GenerateDateTimeRanges(IncomeExpensePeriod period, DateTime startAt, DateTime endAt)
        {
            var ranges = new List<DateTimeRange>();
            DateTimeRange startRange = DateTimeAndPeriodToDateTimeRange(startAt, period);
            ranges.Add(startRange);
            DateTimeRange cursor = startRange;
            while(endAt > cursor.End)
            {
                cursor = NextDateTimeRange(cursor, period);
                ranges.Add(cursor);
            }

            return ranges.OrderBy(r => r.Start);
        }

        public IncomeExpenseStatement Generate(IncomeExpensePeriod period, DateTime startAt, DateTime endAt)
        {
            if (endAt < startAt)
                throw new ArgumentException("End cannnot be earlier than start", nameof(endAt));

            IEnumerable<DateTimeRange> dateTimeRanges = GenerateDateTimeRanges(period, startAt, endAt);

            Currency primaryCurrency = m_currencyService.GetPrimary();

            List<Account> incomeAccounts = m_accountService.GetAll().Where(a => a.Type == AccountType.Income).ToList();
            List<Account> expenseAccounts = m_accountService.GetAll().Where(a => a.Type == AccountType.Expense).ToList();
            List<Account> relevantAccounts = incomeAccounts.Concat(expenseAccounts).ToList();

            List<Transaction> relevantTransactions = m_transactionService.GetAll(
                relevantAccounts.Select(a => a.AccountId),
                startAt,
                endAt).ToList();

            var statementIncomeAccounts = new List<IncomeExpenseAccount>();
            foreach (Account account in incomeAccounts)
            {
                var accountPeriods = new List<IncomeExpenseAccountPeriod>();

                foreach (DateTimeRange range in dateTimeRanges.OrderBy(r => r.Start))
                {
                    IEnumerable<Transaction> periodTransactions = relevantTransactions
                        .Where(t => t.At >= range.Start && t.At < range.End);

                    IEnumerable<Transaction> inflowTransactions =
                        periodTransactions.Where(t =>
                            t.CreditAccount.AccountId == account.AccountId);
                    IEnumerable<Transaction> outflowTransactions =
                        periodTransactions.Where(t =>
                            t.DebitAccount.AccountId == account.AccountId);

                    decimal inflow = inflowTransactions.Sum(t => t.Amount);
                    decimal outflow = outflowTransactions.Sum(t => t.Amount);
                    decimal total = inflow - outflow;

                    var accountPeriod = new IncomeExpenseAccountPeriod(range, total);

                    accountPeriods.Add(accountPeriod);
                }

                var statementIncomeAccount = new IncomeExpenseAccount(
                    account.Name,
                    primaryCurrency.Symbol,
                    accountPeriods
                );

                statementIncomeAccounts.Add(statementIncomeAccount);
            }

            var statementExpenseAccounts = new List<IncomeExpenseAccount>();
            foreach (Account account in expenseAccounts)
            {
                var accountPeriods = new List<IncomeExpenseAccountPeriod>();

                foreach (DateTimeRange range in dateTimeRanges.OrderBy(r => r.Start))
                {
                    IEnumerable<Transaction> periodTransactions = relevantTransactions
                        .Where(t => t.At >= range.Start && t.At < range.End);

                    IEnumerable<Transaction> inflowTransactions =
                        periodTransactions.Where(t =>
                            t.DebitAccount.AccountId == account.AccountId);
                    IEnumerable<Transaction> outflowTransactions =
                        periodTransactions.Where(t =>
                            t.CreditAccount.AccountId == account.AccountId);

                    decimal inflow = inflowTransactions.Sum(t => t.Amount);
                    decimal outflow = outflowTransactions.Sum(t => t.Amount);
                    decimal total = inflow - outflow;

                    var accountPeriod = new IncomeExpenseAccountPeriod(range, total);

                    accountPeriods.Add(accountPeriod);
                }

                var statementExpenseAccount = new IncomeExpenseAccount(
                    account.Name,
                    primaryCurrency.Symbol,
                    accountPeriods
                );

                statementExpenseAccounts.Add(statementExpenseAccount);
            }

            return new IncomeExpenseStatement(
                dateTimeRanges,
                period,
                startAt, 
                endAt, 
                primaryCurrency.Symbol,
                statementIncomeAccounts,
                statementExpenseAccounts
            );
        }
    }
}
