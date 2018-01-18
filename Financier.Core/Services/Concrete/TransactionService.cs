using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Financier.Data;
using Microsoft.Extensions.Logging;

namespace Financier.Services
{
    public class TransactionService : ITransactionService
    {
        private ILogger<TransactionService> m_logger;
        private FinancierDbContext m_dbContext;

        public TransactionService(ILogger<TransactionService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Create(Transaction transaction)
        {
            m_dbContext.Transactions.Add(transaction);
            m_dbContext.SaveChanges();
        }

        public void Delete(int transactionId)
        {
            Transaction transaction = Get(transactionId);
            m_dbContext.Transactions.Remove(transaction);
            m_dbContext.SaveChanges();
        }

        public Transaction Get(int transactionId)
        {
            return m_dbContext.Transactions.Single(t => t.TransactionId == transactionId);
        }

        public IEnumerable<Transaction> GetAll()
        {
            return m_dbContext.Transactions;
        }

        public IEnumerable<Transaction> GetAll(int accountId, bool includeLogicalAccounts)
        {
            var relevantAccountIds = new HashSet<int>();
            relevantAccountIds.Add(accountId);
            if (includeLogicalAccounts)
            {
                IEnumerable<int> logicalAccountIds = m_dbContext.AccountRelationships
                    .Where(r => r.SourceAccountId == accountId &&
                                r.Type == AccountRelationshipType.PhysicalToLogical)
                    .Select(r => r.DestinationAccountId);
                foreach (int logicalAccountId in logicalAccountIds)
                    relevantAccountIds.Add(logicalAccountId);
            }

            return m_dbContext.Transactions
                .Where(t => relevantAccountIds.Contains(t.CreditAccountId) ||
                            relevantAccountIds.Contains(t.DebitAccountId))
                .OrderBy(t => t.TransactionId);
        }

        public void Update(Transaction transaction)
        {
            // TODO: this is problematic as other changes related to retrieved records will be saved

            m_dbContext.SaveChanges();
        }
    }
}
