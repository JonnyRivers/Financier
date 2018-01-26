using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        public bool Any()
        {
            return m_dbContext.Transactions.Any();
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
            Entities.Transaction transactionEntity =
                m_dbContext.Transactions.SingleOrDefault(t => t.TransactionId == transactionId);

            if (transactionEntity == null)
                throw new ArgumentException(
                    $"No Transaction exists with TransactionId {transactionId}",
                    nameof(transactionId)
                );

            m_dbContext.Transactions.Remove(transactionEntity);
            m_dbContext.SaveChanges();
        }

        public Transaction Get(int transactionId)
        {
            Entities.Transaction transactionEntity = 
                m_dbContext.Transactions
                    .Include(t => t.CreditAccount)
                    .Include(t => t.DebitAccount)
                    .SingleOrDefault(t => t.TransactionId == transactionId);

            if (transactionEntity == null)
                throw new ArgumentException(
                    $"No Transaction exists with TransactionId {transactionId}",
                    nameof(transactionId));

            return FromEntity(transactionEntity);
        }

        public Transaction GetMostRecent()
        {
            Entities.Transaction transactionEntity =
                m_dbContext.Transactions
                    .Include(t => t.CreditAccount)
                    .Include(t => t.DebitAccount)
                    .LastOrDefault();

            if (transactionEntity == null)
                throw new InvalidOperationException($"Unable to get most recent transaction as there are none");

            return FromEntity(transactionEntity);
        }

        public IEnumerable<Transaction> GetAll()
        {
            return m_dbContext.Transactions
                .Include(t => t.CreditAccount)
                .Include(t => t.DebitAccount)
                .Select(FromEntity)
                .ToList();
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
                .Include(t => t.CreditAccount)
                .Include(t => t.DebitAccount)
                .Where(t => relevantAccountIds.Contains(t.CreditAccountId) ||
                            relevantAccountIds.Contains(t.DebitAccountId))
                .OrderBy(t => t.TransactionId)
                .Select(FromEntity)
                .ToList();
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
                TransactionId = transactionEntity.TransactionId,
                CreditAccount = FromEntity(transactionEntity.CreditAccount),
                DebitAccount = FromEntity(transactionEntity.DebitAccount),
                Amount = transactionEntity.Amount,
                At = transactionEntity.At
            };
        }

        private static AccountLink FromEntity(Entities.Account accountEntity)
        {
            return new AccountLink
            {
                AccountId = accountEntity.AccountId,
                Name = accountEntity.Name,
                Type = (AccountType)accountEntity.Type
            };
        }
    }
}
