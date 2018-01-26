using Financier.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class FinancierDbContextTests
    {
        [TestMethod]
        public void TestCreateInMemory()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                Assert.AreEqual(0, sqliteMemoryWrapper.DbContext.Accounts.Count());
                Assert.AreEqual(0, sqliteMemoryWrapper.DbContext.Currencies.Count());
                Assert.AreEqual(0, sqliteMemoryWrapper.DbContext.Transactions.Count());
            }
        }

        [TestMethod]
        public void TestCreateCurrency()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                Entities.Currency usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(0, accounts.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);
            }
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                Entities.Currency usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(1, accounts.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);
                Assert.AreEqual(checkingAccountEntity.Currency.CurrencyId, accounts[0].Currency.CurrencyId);
            }
        }

        [TestMethod]
        public void TestCreateTransaction()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                Entities.Currency usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var transaction = new Transaction
                {
                    CreditAccount = incomeAccountEntity,
                    DebitAccount = checkingAccountEntity,
                    Amount = 10m,
                    At = new DateTime(2018, 1, 1, 8, 30, 1)
                };

                sqliteMemoryWrapper.DbContext.Transactions.Add(transaction);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(2, accounts.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(1, transactions.Count);
                Assert.AreEqual(transaction.CreditAccount.AccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(transaction.Amount, transactions[0].Amount);
                Assert.AreEqual(transaction.DebitAccount.AccountId, transactions[0].DebitAccount.AccountId);
            }
        }

        [TestMethod]
        public void TestCreateAccountRelationships()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                Entities.Currency usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);

                var checkingToRentPrepaymentRelationship = new AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationship = new AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<AccountRelationship> accountRelationsips = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(3, accounts.Count);
                Assert.AreEqual(2, accountRelationsips.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);
                Assert.AreEqual(checkingToRentPrepaymentRelationship.SourceAccount.Name, accountRelationsips[0].SourceAccount.Name);
                Assert.AreEqual(checkingToRentPrepaymentRelationship.DestinationAccount.Name, accountRelationsips[0].DestinationAccount.Name);
                Assert.AreEqual(rentPrepaymentToExpenseRelationship.SourceAccount.Name, accountRelationsips[1].SourceAccount.Name);
                Assert.AreEqual(rentPrepaymentToExpenseRelationship.DestinationAccount.Name, accountRelationsips[1].DestinationAccount.Name);
            }
        }

        [TestMethod]
        public void TestCreateBudget()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                Entities.Currency usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                Entities.Account groceriesExpenseAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesExpenseAccountEntity);

                var checkingToRentPrepaymentRelationship = new AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToGroceriesPrepaymentRelationship = new AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationship = new AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };
                var groceriesPrepaymentToExpenseRelationship = new AccountRelationship
                {
                    SourceAccount = groceriesPrepaymentAccountEntity,
                    DestinationAccount = groceriesExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(groceriesPrepaymentToExpenseRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budget = new Budget
                {
                    Name = "Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budget);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var initialTransaction = new BudgetTransaction
                {
                    CreditAccount = incomeAccountEntity,
                    DebitAccount = checkingAccountEntity,
                    Amount = 200m,
                    IsInitial = true,
                    Budget = budget
                };
                var rentTransaction = new BudgetTransaction
                {
                    CreditAccount = checkingAccountEntity,
                    DebitAccount = rentPrepaymentAccountEntity,
                    Amount = 100m,
                    Budget = budget
                };
                var groceriesTransaction = new BudgetTransaction
                {
                    CreditAccount = checkingAccountEntity,
                    DebitAccount = groceriesPrepaymentAccountEntity,
                    Amount = 50m,
                    Budget = budget
                };
                var surplusTransaction = new BudgetTransaction
                {
                    CreditAccount = checkingAccountEntity,
                    DebitAccount = groceriesPrepaymentAccountEntity,
                    IsSurplus = true,
                    Budget = budget
                };
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(initialTransaction);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(rentTransaction);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(groceriesTransaction);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(surplusTransaction);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<AccountRelationship> accountRelationsips = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Budget> budgets = sqliteMemoryWrapper.DbContext.Budgets.ToList();
                List<BudgetTransaction> budgetTransactions = sqliteMemoryWrapper.DbContext.BudgetTransactions.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(7, accounts.Count);
                Assert.AreEqual(4, accountRelationsips.Count);
                Assert.AreEqual(1, budgets.Count);
                Assert.AreEqual(4, budgetTransactions.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);

                Assert.AreEqual(budget.Name, budgets[0].Name);
                Assert.AreEqual(budget.Period, budgets[0].Period);
                Assert.AreEqual(budgetTransactions.Count, budgets[0].Transactions.Count);

                Assert.AreEqual(budget.BudgetId, budgetTransactions[0].BudgetId);
                Assert.AreEqual(initialTransaction.CreditAccountId, budgetTransactions[0].CreditAccountId);
                Assert.AreEqual(initialTransaction.DebitAccountId, budgetTransactions[0].DebitAccountId);
                Assert.AreEqual(initialTransaction.Amount, budgetTransactions[0].Amount);
                Assert.AreEqual(true, budgetTransactions[0].IsInitial);
                Assert.AreEqual(false, budgetTransactions[0].IsSurplus);
                Assert.AreEqual(budget.BudgetId, budgetTransactions[1].BudgetId);
                Assert.AreEqual(rentTransaction.CreditAccountId, budgetTransactions[1].CreditAccountId);
                Assert.AreEqual(rentTransaction.DebitAccountId, budgetTransactions[1].DebitAccountId);
                Assert.AreEqual(rentTransaction.Amount, budgetTransactions[1].Amount);
                Assert.AreEqual(false, budgetTransactions[1].IsInitial);
                Assert.AreEqual(false, budgetTransactions[1].IsSurplus);
                Assert.AreEqual(budget.BudgetId, budgetTransactions[2].BudgetId);
                Assert.AreEqual(groceriesTransaction.CreditAccountId, budgetTransactions[2].CreditAccountId);
                Assert.AreEqual(groceriesTransaction.DebitAccountId, budgetTransactions[2].DebitAccountId);
                Assert.AreEqual(groceriesTransaction.Amount, budgetTransactions[2].Amount);
                Assert.AreEqual(false, budgetTransactions[2].IsInitial);
                Assert.AreEqual(false, budgetTransactions[2].IsSurplus);
                Assert.AreEqual(budget.BudgetId, budgetTransactions[3].BudgetId);
                Assert.AreEqual(surplusTransaction.CreditAccountId, budgetTransactions[3].CreditAccountId);
                Assert.AreEqual(surplusTransaction.DebitAccountId, budgetTransactions[3].DebitAccountId);
                Assert.AreEqual(surplusTransaction.Amount, budgetTransactions[3].Amount);
                Assert.AreEqual(false, budgetTransactions[3].IsInitial);
                Assert.AreEqual(true, budgetTransactions[3].IsSurplus);
            }
        }
    }
}
