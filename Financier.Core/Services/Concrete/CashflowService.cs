using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    public class CashflowService : ICashflowService
    {
        ILogger<CashflowService> m_logger;
        IAccountRelationshipService m_accountRelationshipService;
        IAccountService m_accountService;
        ICurrencyService m_currencyService;
        ITransactionService m_transactionService;

        public CashflowService(
            ILoggerFactory loggerFactory,
            IAccountRelationshipService accountRelationshipService,
            IAccountService accountService,
            ICurrencyService currencyService,
            ITransactionService transactionService)
        {
            m_logger = loggerFactory.CreateLogger<CashflowService>();
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;
            m_currencyService = currencyService;
            m_transactionService = transactionService;
        }

        private static TimeSpan PeriodToTimeSpan(CashflowPeriod period)
        {
            if(period == CashflowPeriod.Fortnightly)
                return TimeSpan.FromDays(14);

            throw new ArgumentException($"Unrecognized CashflowPeriod {period}", nameof(period));
        }

        private static IEnumerable<DateTimeRange> CreateDateTimeRanges(CashflowPeriod period, DateTime startAt, DateTime endAt)
        {
            if (endAt <= startAt)
                throw new ArgumentException("Start must be earlier than end", nameof(endAt));

            TimeSpan periodSpan = PeriodToTimeSpan(period);

            var ranges = new List<DateTimeRange>();
            DateTime cursor = startAt;
            while(cursor < endAt)
            {
                ranges.Add(new DateTimeRange(cursor, cursor + periodSpan));

                cursor += periodSpan;
            }

            return ranges;
        }

        public CashflowStatement Generate(CashflowPeriod period, DateTime startAt, DateTime endAt)
        {
            if (endAt < startAt)
                throw new ArgumentException("End cannnot be earlier than start", nameof(endAt));

            IEnumerable<DateTimeRange> ranges = CreateDateTimeRanges(CashflowPeriod.Fortnightly, startAt, endAt);

            Currency primaryCurrency = m_currencyService.GetPrimary();

            List<AccountRelationship> prepaymentToExpenseRelationships = 
                m_accountRelationshipService
                    .GetAll()
                    .Where(ar => ar.Type == AccountRelationshipType.PrepaymentToExpense)
                    .ToList();

            var relevantAccountIds = new HashSet<int>(
                prepaymentToExpenseRelationships.SelectMany(
                    r => new int[2] {
                        r.SourceAccount.AccountId,
                        r.DestinationAccount.AccountId
                    }
                )
            );

            List<Transaction> relevantTransactions = m_transactionService.GetAll(
                relevantAccountIds,
                startAt,
                endAt).ToList();

            var cashflowAccounts = new List<CashflowAccount>();
            foreach (AccountRelationship prepaymentToExpenseRelationship in prepaymentToExpenseRelationships)
            {
                string name =
                    prepaymentToExpenseRelationship.DestinationAccount.Name
                        .Replace("Expense", "")
                        .Trim();

                var accountPeriods = new List<CashflowAccountPeriod>();
                foreach(DateTimeRange range in ranges.OrderBy(r => r.Start))
                {
                    IEnumerable<Transaction> periodTransactions = relevantTransactions
                        .Where(t => t.At >= range.Start && t.At < range.End);

                    IEnumerable<Transaction> inflowTransactions =
                        periodTransactions.Where(t =>
                            t.DebitAccount.AccountId == prepaymentToExpenseRelationship.SourceAccount.AccountId);
                    IEnumerable<Transaction> outflowTransactions =
                        periodTransactions.Where(t =>
                            t.DebitAccount.AccountId == prepaymentToExpenseRelationship.DestinationAccount.AccountId);

                    var accountPeriod = new CashflowAccountPeriod(
                        range,
                        inflowTransactions.Sum(t => t.Amount),
                        outflowTransactions.Sum(t => t.Amount));

                    accountPeriods.Add(accountPeriod);
                }

                var account = new CashflowAccount(
                    name,
                    primaryCurrency.Symbol,
                    accountPeriods
                );

                cashflowAccounts.Add(account);
            }
            
            return new CashflowStatement(
                period,
                startAt, 
                endAt, 
                primaryCurrency.Symbol,
                cashflowAccounts
            );
        }
    }
}
