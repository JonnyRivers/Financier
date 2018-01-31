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
            var linkedExpenseAccountIds = new HashSet<int>(
                m_dbContext.AccountRelationships
                    .Where(ar => ar.Type == AccountRelationshipType.PrepaymentToExpense)
                    .Select(ar => ar.DestinationAccountId)
            );

            List<int> linkedOutgoingTransactionsIds = m_dbContext.Transactions
                .Where(t => t.CreditAccountId == accountId && linkedExpenseAccountIds.Contains(t.DebitAccountId))
                .Select(t => t.TransactionId)
                .ToList();

            var paidTransactionIds = new HashSet<int>(
                m_dbContext.TransactionRelationships
                    .Where(tr => tr.Type == TransactionRelationshipType.CreditCardPayment &&
                                 tr.DestinationTransaction.CreditAccountId == accountId)
                    .Select(tr => tr.SourceTransactionId)
            );

            var pendingTransactionIds = new HashSet<int>(
                linkedOutgoingTransactionsIds.Except(paidTransactionIds)
                .ToList()
            );

            if (!pendingTransactionIds.Any())
                return new Payment[0];

            Dictionary<int, AccountLink> prepaymentAccountLinksByLinkedExpenseAccountId =
                m_dbContext.AccountRelationships
                    .Include(ar => ar.SourceAccount)
                    .Where(ar => ar.Type == AccountRelationshipType.PrepaymentToExpense &&
                                 linkedExpenseAccountIds.Contains(ar.DestinationAccountId))
                    .ToDictionary(ar => ar.DestinationAccountId, ar => FromEntity(ar.SourceAccount));

            List<Transaction> pendingTransactions = 
                m_dbContext.Transactions
                    .Where(t => pendingTransactionIds.Contains(t.TransactionId))
                    .Include(t => t.CreditAccount)
                    .Include(t => t.DebitAccount)
                    .Select(FromEntity)
                    .ToList();

            List<Payment> pendingPayments = pendingTransactions
                .Select(t => new Payment(
                    t,
                    new Transaction
                    {
                        At = DateTime.Now,
                        DebitAccount = t.CreditAccount,
                        CreditAccount = prepaymentAccountLinksByLinkedExpenseAccountId[t.DebitAccount.AccountId],
                        Amount = t.Amount
                    }))
                .ToList();

            return pendingPayments;
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
                Type = accountEntity.Type,
                SubType = accountEntity.SubType
            };
        }
    }
}
