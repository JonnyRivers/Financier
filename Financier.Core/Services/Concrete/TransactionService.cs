using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Financier.Services
{
    public class TransactionService : ITransactionService
    {
        private ILogger<TransactionService> m_logger;
        private Entities.FinancierDbContext m_dbContext;

        public TransactionService(ILogger<TransactionService> logger, Entities.FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Create(Transaction transaction)
        {
            var transactionEntity = new Entities.Transaction
            {
                CreditAccountId = transaction.CreditAccount.AccountId,
                DebitAccountId = transaction.DebitAccount.AccountId,
                Amount = transaction.Amount,
                At = transaction.At
            };

            m_dbContext.Transactions.Add(transactionEntity);
            m_dbContext.SaveChanges();

            transaction.TransactionId = transactionEntity.TransactionId;
        }

        public void Delete(int transactionId)
        {
            Entities.Transaction transaction =
                m_dbContext.Transactions.Single(t => t.TransactionId == transactionId);
            m_dbContext.Transactions.Remove(transaction);
            m_dbContext.SaveChanges();
        }

        public Transaction Get(int transactionId)
        {
            Entities.Transaction transaction = 
                m_dbContext.Transactions.Single(t => t.TransactionId == transactionId);

            return FromEntity(transaction);
        }

        public IEnumerable<Transaction> GetAll()
        {
            return m_dbContext.Transactions.Select(FromEntity);
        }

        public IEnumerable<Transaction> GetAll(int accountId, bool includeLogicalAccounts)
        {
            var relevantAccountIds = new HashSet<int>();
            relevantAccountIds.Add(accountId);
            if (includeLogicalAccounts)
            {
                IEnumerable<int> logicalAccountIds = m_dbContext.AccountRelationships
                    .Where(r => r.SourceAccountId == accountId &&
                                r.Type == Entities.AccountRelationshipType.PhysicalToLogical)
                    .Select(r => r.DestinationAccountId);
                foreach (int logicalAccountId in logicalAccountIds)
                    relevantAccountIds.Add(logicalAccountId);
            }

            return m_dbContext.Transactions
                .Where(t => relevantAccountIds.Contains(t.CreditAccountId) ||
                            relevantAccountIds.Contains(t.DebitAccountId))
                .OrderBy(t => t.TransactionId)
                .Select(FromEntity);
        }

        public void Update(Transaction transaction)
        {
            Entities.Transaction transactionEntity = m_dbContext.Transactions
                .Single(t => t.TransactionId == transaction.TransactionId);

            transactionEntity.CreditAccountId = transaction.CreditAccount.AccountId;
            transactionEntity.DebitAccountId = transaction.DebitAccount.AccountId;
            transactionEntity.Amount = transaction.Amount;
            transactionEntity.At = transaction.At;

            m_dbContext.SaveChanges();
        }

        private static Transaction FromEntity(Entities.Transaction transactionEntity)
        {
            return new Transaction
            {
                CreditAccount = FromEntity(transactionEntity.CreditAccount),
                DebitAccount = FromEntity(transactionEntity.DebitAccount),
                Amount = transactionEntity.Amount,
                At = transactionEntity.At
            };
        }

        private static AccountSummary FromEntity(Entities.Account accountEntity)
        {
            return new AccountSummary
            {
                AccountId = accountEntity.AccountId,
                Name = accountEntity.Name,
                Type = (AccountType)accountEntity.Type
            };
        }
    }
}
