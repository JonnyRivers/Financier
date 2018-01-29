using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    public class BudgetService : IBudgetService
    {
        private ILogger<BudgetService> m_logger;
        private Entities.FinancierDbContext m_dbContext;

        public BudgetService(ILogger<BudgetService> logger, Entities.FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Create(Budget budget)
        {
            if (budget == null)
                throw new ArgumentNullException(nameof(budget));

            if (budget.InitialTransaction == null)
                throw new ArgumentException("InitialTransaction cannot be null", nameof(budget));

            if (budget.SurplusTransaction == null)
                throw new ArgumentException("SurplusTransaction cannot be null", nameof(budget));

            using (var transaction = m_dbContext.Database.BeginTransaction())
            {
                var budgetEntity = new Entities.Budget
                {
                    Name = budget.Name,
                    Period = budget.Period
                };
                m_dbContext.Budgets.Add(budgetEntity);
                m_dbContext.SaveChanges();

                Entities.BudgetTransaction initialTransactionEntity = ToEntity(budgetEntity, budget.InitialTransaction);
                initialTransactionEntity.IsInitial = true;
                m_dbContext.BudgetTransactions.Add(initialTransactionEntity);

                List<Entities.BudgetTransaction> transactionEntities = 
                    budget.Transactions
                        .Select(t => ToEntity(budgetEntity, t))
                        .ToList();
                m_dbContext.BudgetTransactions.AddRange(transactionEntities);

                Entities.BudgetTransaction surplusTransactionEntity = ToEntity(budgetEntity, budget.SurplusTransaction);
                surplusTransactionEntity.IsSurplus = true;
                m_dbContext.BudgetTransactions.Add(surplusTransactionEntity);

                m_dbContext.SaveChanges();

                transaction.Commit();

                // Update IDs
                budget.BudgetId = budgetEntity.BudgetId;
                budget.InitialTransaction.BudgetTransactionId = initialTransactionEntity.BudgetTransactionId;
                for(int transactionIndex = 0; transactionIndex < budget.Transactions.Count(); ++transactionIndex)
                {
                    budget.Transactions.ElementAt(transactionIndex).BudgetTransactionId =
                        transactionEntities[transactionIndex].BudgetTransactionId;
                }
                budget.SurplusTransaction.BudgetTransactionId = surplusTransactionEntity.BudgetTransactionId;
            }
        }

        public void Delete(int budgetId)
        {
            Entities.Budget budgetEntity =
                m_dbContext.Budgets.SingleOrDefault(b => b.BudgetId == budgetId);

            if (budgetEntity == null)
                throw new ArgumentException($"No Budget exists with BudgetId {budgetId}", nameof(budgetId));

            m_dbContext.Budgets.Remove(budgetEntity);
            m_dbContext.SaveChanges();
        }

        public Budget Get(int budgetId)
        {
            Entities.Budget budgetEntity =
                m_dbContext.Budgets
                    .Include(b => b.Transactions)
                        .ThenInclude(t => t.CreditAccount)
                    .Include(b => b.Transactions)
                        .ThenInclude(t => t.DebitAccount)
                    .SingleOrDefault(b => b.BudgetId == budgetId);

            if (budgetEntity == null)
                throw new ArgumentException($"No Budget exists with BudgetId {budgetId}", nameof(budgetId));

            return FromEntity(budgetEntity);
        }

        public IEnumerable<Budget> GetAll()
        {
            List<Entities.Budget> budgetEntities = 
                m_dbContext.Budgets
                    .Include(b => b.Transactions)
                        .ThenInclude(t => t.CreditAccount)
                    .Include(b => b.Transactions)
                        .ThenInclude(t => t.DebitAccount)
                    .ToList();
            List<Budget> budgets = budgetEntities.Select(FromEntity).ToList();

            return budgets;
        }

        public void Update(Budget budget)
        {
            Entities.Budget budgetEntity = m_dbContext.Budgets
                .Include(b => b.Transactions)
                .Single(b => b.BudgetId == budget.BudgetId);
            budgetEntity.Name = budget.Name;
            budgetEntity.Period = (BudgetPeriod)budget.Period;

            Entities.BudgetTransaction initialTransactionEntity = budgetEntity.Transactions.Single(t => t.IsInitial);
            UpdateTransactionEntity(budget.InitialTransaction, initialTransactionEntity);

            List<Entities.BudgetTransaction> transactionEntities =
                budgetEntity.Transactions
                    .Where(t => !t.IsInitial && !t.IsSurplus)
                    .ToList();
            var transactionIds = new HashSet<int>(
                budget.Transactions
                    .Select(t => t.BudgetTransactionId)
            );

            foreach (Entities.BudgetTransaction transactionEntity in transactionEntities)
            {
                if (transactionIds.Contains(transactionEntity.BudgetTransactionId))
                {
                    // update existing transaction
                    BudgetTransaction transaction = budget.Transactions
                        .Single(t => t.BudgetTransactionId == transactionEntity.BudgetTransactionId);
                    UpdateTransactionEntity(transaction, transactionEntity);
                }
                else
                {
                    // remove missing transaction
                    budgetEntity.Transactions.Remove(transactionEntity);
                }
            }

            // add new transactions
            var transactionEntityIds = new HashSet<int>(transactionEntities.Select(t => t.BudgetTransactionId));
            IEnumerable<BudgetTransaction> transactionsWithNoEntity = 
                budget.Transactions.Where(t => !transactionEntityIds.Contains(t.BudgetTransactionId));
            foreach (BudgetTransaction transactionWithNoEntity in transactionsWithNoEntity)
            {
                budgetEntity.Transactions.Add(ToEntity(budgetEntity, transactionWithNoEntity));
            }

            Entities.BudgetTransaction surplusTransactionEntity = budgetEntity.Transactions.Single(t => t.IsSurplus);
            UpdateTransactionEntity(budget.SurplusTransaction, surplusTransactionEntity);

            m_dbContext.SaveChanges();
        }

        public IEnumerable<Transaction> MakePaydayTransactions(int budgetId, PaydayStart paydayStart)
        {
            var paydayTransactions = new List<Transaction>();

            Budget budget = Get(budgetId);

            Transaction initialTransaction = new Transaction
            {
                CreditAccount = budget.InitialTransaction.CreditAccount,
                DebitAccount = budget.InitialTransaction.DebitAccount,
                Amount = paydayStart.AmountPaid,
                At = paydayStart.At
            };
            paydayTransactions.Add(initialTransaction);

            foreach (BudgetTransaction budgetTransaction in budget.Transactions)
            {
                Transaction paydayTransaction = new Transaction
                {
                    CreditAccount = budgetTransaction.CreditAccount,
                    DebitAccount = budgetTransaction.DebitAccount,
                    Amount = budgetTransaction.Amount,
                    At = paydayStart.At
                };
                paydayTransactions.Add(paydayTransaction);
            }

            IEnumerable<BudgetTransaction> initialDebitOutgoingTransactions = 
                budget.Transactions
                    .Where(t => t.CreditAccount.AccountId == initialTransaction.DebitAccount.AccountId);
            IEnumerable<BudgetTransaction> initialDebitIncomingTransactions =
                budget.Transactions
                    .Where(t => t.DebitAccount.AccountId == initialTransaction.DebitAccount.AccountId);
            decimal surplusAmount = 
                paydayStart.AmountPaid + 
                initialDebitIncomingTransactions.Select(t => t.Amount).Sum() - 
                initialDebitOutgoingTransactions.Select(t => t.Amount).Sum();
            Transaction surplusTransaction = new Transaction
            {
                CreditAccount = budget.SurplusTransaction.CreditAccount,
                DebitAccount = budget.SurplusTransaction.DebitAccount,
                Amount = surplusAmount,
                At = paydayStart.At
            };
            paydayTransactions.Add(surplusTransaction);

            return paydayTransactions;
        }

        private static Budget FromEntity(Entities.Budget budgetEntity)
        {
            Entities.BudgetTransaction initialTransactionEntity = 
                budgetEntity.Transactions.Single(t => t.IsInitial);
            IEnumerable<Entities.BudgetTransaction> transactionEntities = 
                budgetEntity.Transactions.Where(t => !t.IsInitial && !t.IsSurplus);
            Entities.BudgetTransaction surplusTransactionEntity =
                budgetEntity.Transactions.Single(t => t.IsSurplus);

            var budget = new Budget
            {
                BudgetId = budgetEntity.BudgetId,
                Name = budgetEntity.Name,
                Period = (BudgetPeriod)budgetEntity.Period,
                InitialTransaction = FromEntity(initialTransactionEntity),
                Transactions = transactionEntities.Select(FromEntity).ToList(),
                SurplusTransaction = FromEntity(surplusTransactionEntity),
            };

            return budget;
        }

        private static BudgetTransaction FromEntity(Entities.BudgetTransaction budgetTransactionEntity)
        {
            var budgetTransaction = new BudgetTransaction
            {
                BudgetTransactionId = budgetTransactionEntity.BudgetTransactionId,
                CreditAccount = new AccountLink
                {
                    AccountId = budgetTransactionEntity.CreditAccount.AccountId,
                    Name = budgetTransactionEntity.CreditAccount.Name,
                    Type = budgetTransactionEntity.CreditAccount.Type,
                    SubType = budgetTransactionEntity.CreditAccount.SubType
                },
                DebitAccount = new AccountLink
                {
                    AccountId = budgetTransactionEntity.DebitAccount.AccountId,
                    Name = budgetTransactionEntity.DebitAccount.Name,
                    Type = budgetTransactionEntity.DebitAccount.Type,
                    SubType = budgetTransactionEntity.DebitAccount.SubType
                },
                Amount = budgetTransactionEntity.Amount
            };

            return budgetTransaction;
        }

        private static Entities.BudgetTransaction ToEntity(Entities.Budget budgetEntity, BudgetTransaction budgetTransaction)
        {
            var transactionEntity = new Entities.BudgetTransaction
            {
                Budget = budgetEntity,
                CreditAccountId = budgetTransaction.CreditAccount.AccountId,
                DebitAccountId = budgetTransaction.DebitAccount.AccountId,
                Amount = budgetTransaction.Amount,
                IsInitial = false,
                IsSurplus = false
            };

            return transactionEntity;
        }

        private static void UpdateTransactionEntity(
            BudgetTransaction budgetTransaction, 
            Entities.BudgetTransaction budgetTransactionEntity)
        {
            budgetTransactionEntity.Amount = budgetTransaction.Amount;
            budgetTransactionEntity.CreditAccountId = budgetTransaction.CreditAccount.AccountId;
            budgetTransactionEntity.DebitAccountId = budgetTransaction.DebitAccount.AccountId;
        }
    }
}
