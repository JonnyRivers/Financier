using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    public class BalanceSheetService : IBalanceSheetService
    {
        private ILogger<BalanceSheetService> m_logger;
        private Entities.FinancierDbContext m_dbContext;

        public BalanceSheetService(
            ILoggerFactory loggerFactory,
            Entities.FinancierDbContext dbContext)
        {
            m_logger = loggerFactory.CreateLogger<BalanceSheetService>();
            m_dbContext = dbContext;
        }

        public BalanceSheet Generate(DateTime at)
        {
            string primaryCurrencySymbol = m_dbContext.Currencies.Single(c => c.IsPrimary).Symbol;

            var logicalAccountIds = new HashSet<int>(
                m_dbContext.AccountRelationships
                    .Where(ar => ar.Type == AccountRelationshipType.PhysicalToLogical)
                    .Select(ar => ar.DestinationAccountId)
            );

            List<Entities.Account> allPhysicalAccountEntities = m_dbContext.Accounts
                .Include(a => a.Currency)
                .Where(a => a.Type == AccountType.Asset || a.Type == AccountType.Liability)
                .Where(a => !logicalAccountIds.ToList().Contains(a.AccountId))
                .ToList();

            List<int> allPhysicalAccountIds = allPhysicalAccountEntities.Select(a => a.AccountId).ToList();

            List<Entities.AccountRelationship> allRelatedPhysicalLogicalRelationships = m_dbContext.AccountRelationships
                    .Where(r => allPhysicalAccountIds.Contains(r.SourceAccountId) &&
                                r.Type == AccountRelationshipType.PhysicalToLogical)
                    .ToList();
            List<int> allRelatedLogicalAccountIds = allRelatedPhysicalLogicalRelationships.Select(r => r.DestinationAccountId).ToList();

            List<int> allAccountIds = new HashSet<int>(allPhysicalAccountIds.Concat(allRelatedLogicalAccountIds)).ToList();

            List<Entities.Transaction> allRelatedCreditTransactions =
                m_dbContext.Transactions
                    .Where(t => allAccountIds.Contains(t.CreditAccountId) &&
                                t.At <= at)
                    .ToList();
            List<Entities.Transaction> allRelatedDebitTransactions =
                m_dbContext.Transactions
                    .Where(t => allAccountIds.Contains(t.DebitAccountId) &&
                                t.At <= at)
                    .ToList();

            var assets = new List<BalanceSheetItem>();
            var liabilities = new List<BalanceSheetItem>();
            foreach (Entities.Account physicalAccountEntity in allPhysicalAccountEntities)
            {
                IEnumerable<int> relatedLogicalAccountIds = allRelatedPhysicalLogicalRelationships
                    .Where(r => r.SourceAccountId == physicalAccountEntity.AccountId)
                    .Select(r => r.DestinationAccountId);

                var accountIds = new List<int>();
                accountIds.Add(physicalAccountEntity.AccountId);
                accountIds.AddRange(relatedLogicalAccountIds);

                IEnumerable<Entities.Transaction> creditTransactions = allRelatedCreditTransactions
                    .Where(t => accountIds.Contains(t.CreditAccountId));
                IEnumerable<Entities.Transaction> debitTransactions = allRelatedDebitTransactions
                    .Where(t => accountIds.Contains(t.DebitAccountId));

                // get most recent transaction
                IEnumerable<Entities.Transaction> allTransactions = creditTransactions.Concat(debitTransactions).OrderByDescending(t => t.At);
                if (!allTransactions.Any())
                    continue;

                // calculate balance
                decimal creditBalance = creditTransactions.Sum(t => t.Amount);
                decimal debitBalance = debitTransactions.Sum(t => t.Amount);
                decimal balance = debitBalance - creditBalance;

                var item = new BalanceSheetItem(
                    physicalAccountEntity.Name,
                    balance,
                    allTransactions.Select(t => t.At).First()
                );

                if(physicalAccountEntity.Type == AccountType.Asset)
                {
                    assets.Add(item);
                }
                else if (physicalAccountEntity.Type == AccountType.Liability)
                {
                    liabilities.Add(item);
                }
            }

            return new BalanceSheet(primaryCurrencySymbol, assets, liabilities);
        }
    }
}
