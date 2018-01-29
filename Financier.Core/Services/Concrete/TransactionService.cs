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

        public IEnumerable<Transaction> GetAll()
        {
            return m_dbContext.Transactions
                .Include(t => t.CreditAccount)
                .Include(t => t.DebitAccount)
                .Select(FromEntity)
                .ToList();
        }

        public IEnumerable<Transaction> GetAll(IEnumerable<int> accountIds)
        {
            var accountIdSet = new HashSet<int>(accountIds);

            return m_dbContext.Transactions
                .Where(t => accountIdSet.Contains(t.CreditAccountId) || 
                            accountIdSet.Contains(t.DebitAccountId))
                .Include(t => t.CreditAccount)
                .Include(t => t.DebitAccount)
                .Select(FromEntity)
                .ToList();
        }

        public IEnumerable<Payment> GetPendingCreditCardPayments(int accountId)
        {
            throw new NotImplementedException();
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
            // TODO: LogicalAccountIds are not fully populated in some places
            // https://github.com/JonnyRivers/Financier/issues/34
            return new AccountLink
            {
                AccountId = accountEntity.AccountId,
                Name = accountEntity.Name,
                Type = accountEntity.Type,
                SubType = accountEntity.SubType
            };
        }
    }
}
