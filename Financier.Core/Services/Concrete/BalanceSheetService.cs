using Financier.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Financier.Services
{
    public class BalanceSheetService : IBalanceSheetService
    {
        ILogger<BalanceSheetService> m_logger;
        FinancierDbContext m_dbContext;

        public BalanceSheetService(ILogger<BalanceSheetService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public BalanceSheet Generate(DateTime at)
        {
            // TODO: this is implemented in terms of very low level (data layer) concepts.
            // This should be implemented in terms of other services.

            Currency primaryCurrency = m_dbContext.Currencies.Single(c => c.IsPrimary);

            // Find all logical account ids
            List<AccountRelationship> physicalToLogicalRelationships = 
                m_dbContext.AccountRelationships
                    .Where(ar => ar.Type == AccountRelationshipType.PhysicalToLogical)
                    .ToList();
            HashSet<int> logicalAccountIds = new HashSet<int>(physicalToLogicalRelationships.Select(ar => ar.DestinationAccountId));

            // Interesting accounts are non-logical accounts of type Asset, Cpaital and Liability
            List<Account> interestingAccounts = 
                m_dbContext.Accounts
                    .Where(
                        a => 
                        !logicalAccountIds.Contains(a.AccountId) && 
                        (
                         a.Type == AccountType.Asset || 
                         a.Type == AccountType.Liability)
                        )
                    .ToList();

            var assets = new List<BalanceSheetItem>();
            var liabilities = new List<BalanceSheetItem>();
            foreach (Account interestingAccount in interestingAccounts)
            {
                // TODO: we are hitting the database too much
                var item = new BalanceSheetItem(interestingAccount.Name, GetBalance(interestingAccount, at));

                if(interestingAccount.Type == AccountType.Asset)
                {
                    assets.Add(item);
                }
                else if (interestingAccount.Type == AccountType.Liability)
                {
                    liabilities.Add(item);
                }
            }

            return new BalanceSheet(primaryCurrency.Symbol, assets, liabilities);
        }

        // TODO: centralize this
        private decimal GetBalance(Account account, DateTime at)
        {
            IEnumerable<int> logicalAccountIds = m_dbContext.AccountRelationships
                .Where(r => r.SourceAccountId == account.AccountId && r.Type == AccountRelationshipType.PhysicalToLogical)
                .Select(r => r.DestinationAccountId);
            var relevantAccountIds = new HashSet<int>(logicalAccountIds);
            relevantAccountIds.Add(account.AccountId);

            IEnumerable<Transaction> creditTransactions = m_dbContext.Transactions
                .Where(t => 
                    relevantAccountIds.Contains(t.CreditAccountId) &&
                    t.At <= at);
            IEnumerable<Transaction> debitTransactions = m_dbContext.Transactions
                .Where(t => 
                    relevantAccountIds.Contains(t.DebitAccountId) &&
                    t.At <= at);

            decimal creditBalance = creditTransactions.Sum(t => t.Amount);
            decimal debitBalance = debitTransactions.Sum(t => t.Amount);

            if (IsCreditAccount(account))
            {
                return creditBalance - debitBalance;
            }

            return debitBalance - creditBalance;
        }

        // TODO: move this
        private static bool IsCreditAccount(Account account)
        {
            return (
                account.Type == AccountType.Capital || 
                account.Type == AccountType.Income || 
                account.Type == AccountType.Liability
            );
        }
    }
}
