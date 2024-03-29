﻿using Financier.Services;
using Financier.UnitTesting;
using Financier.UnitTesting.DbSetup;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Core.Tests
{
    [TestClass]
    public class BudgetServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBudgetCreateFailNullInitialTransaction()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                var budget = new Budget
                {
                    Name = "Budget",
                    InitialTransaction = null,
                    Transactions = new BudgetTransaction[0],
                    SurplusTransaction = new BudgetTransaction
                    {
                        CreditAccount = null,
                        DebitAccount = null
                    }
                };

                budgetService.Create(budget);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBudgetCreateFailNullSurplusTransaction()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                var budget = new Budget
                {
                    Name = "Budget",
                    InitialTransaction = new BudgetTransaction
                    {
                        CreditAccount = null,
                        DebitAccount = null
                    },
                    Transactions = new BudgetTransaction[0],
                    SurplusTransaction = null
                };

                budgetService.Create(budget);
            }
        }

        [TestMethod]
        public void TestBudgetCreate()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);

                var checkingToRentPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext
                );
                var budgetService = new BudgetService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext
                );

                Account incomeAccount = accountService.Get(incomeAccountEntity.AccountId);
                Account checkingAccount = accountService.Get(checkingAccountEntity.AccountId);
                Account savingsAccount = accountService.Get(savingsAccountEntity.AccountId);
                Account rentPrepaymentAccount = accountService.Get(rentPrepaymentAccountEntity.AccountId);

                var budget = new Budget
                {
                    Name = "Budget",
                    Period = BudgetPeriod.Fortnightly,
                    InitialTransaction = new BudgetTransaction
                    {
                        CreditAccount = new AccountLink
                        {
                            AccountId = incomeAccount.AccountId
                        },
                        DebitAccount = new AccountLink
                        {
                            AccountId = checkingAccount.AccountId
                        },
                        Amount = 200m
                    },
                    Transactions = new BudgetTransaction[1] {
                        new BudgetTransaction
                        {
                            CreditAccount = new AccountLink
                            {
                                AccountId = checkingAccount.AccountId
                            },
                            DebitAccount = new AccountLink
                            {
                                AccountId = rentPrepaymentAccount.AccountId
                            },
                            Amount = 100m
                        }
                    },
                    SurplusTransaction = new BudgetTransaction
                    {
                        CreditAccount = new AccountLink
                        {
                            AccountId = checkingAccount.AccountId
                        },
                        DebitAccount = new AccountLink
                        {
                            AccountId = savingsAccount.AccountId
                        }
                    }
                };

                budgetService.Create(budget);

                List<Entities.Budget> budgetEntities = sqliteMemoryWrapper.DbContext.Budgets.ToList();
                List<Entities.BudgetTransaction> budgetTransactionEntities = 
                    sqliteMemoryWrapper.DbContext.BudgetTransactions.ToList();

                Assert.AreEqual(1, budgetEntities.Count);
                Assert.AreEqual(budget.BudgetId, budgetEntities[0].BudgetId);
                Assert.AreEqual(budget.Name, budgetEntities[0].Name);
                Assert.AreEqual(BudgetPeriod.Fortnightly, budgetEntities[0].Period);
                Assert.AreEqual(3, budgetEntities[0].Transactions.Count);

                Assert.AreEqual(3, budgetTransactionEntities.Count);

                // TODO: check the data in a way that isn not order dependent
                //Assert.AreEqual(budget.InitialTransaction.BudgetTransactionId, 
                //    budgetTransactionEntities[0].BudgetTransactionId);
                //Assert.AreEqual(budget.Transactions.ElementAt(0).BudgetTransactionId, 
                //    budgetTransactionEntities[1].BudgetTransactionId);
                //Assert.AreEqual(budget.SurplusTransaction.BudgetTransactionId, 
                //    budgetTransactionEntities[2].BudgetTransactionId);
                //Assert.AreEqual(budgetTransactionEntities[0].CreditAccountId, 
                //    budget.InitialTransaction.CreditAccount.AccountId);
                //Assert.AreEqual(budgetTransactionEntities[0].DebitAccountId, 
                //    budget.InitialTransaction.DebitAccount.AccountId);
                //Assert.AreEqual(budgetTransactionEntities[1].CreditAccountId, 
                //    budget.Transactions.ElementAt(0).CreditAccount.AccountId);
                //Assert.AreEqual(budgetTransactionEntities[1].DebitAccountId, 
                //    budget.Transactions.ElementAt(0).DebitAccount.AccountId);
                //Assert.AreEqual(budgetTransactionEntities[2].CreditAccountId, 
                //    budget.SurplusTransaction.CreditAccount.AccountId);
                //Assert.AreEqual(budgetTransactionEntities[2].DebitAccountId, 
                //    budget.SurplusTransaction.DebitAccount.AccountId);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBudgetDeleteFailInvalidId()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                budgetService.Delete(666);
            }
        }

        [TestMethod]
        public void TestBudgetDelete()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                Entities.Account groceriesExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesExpenseAccountEntity);

                var checkingToRentPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToGroceriesPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };
                var groceriesPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = groceriesPrepaymentAccountEntity,
                    DestinationAccount = groceriesExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(groceriesPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budget1Entity = new Entities.Budget
                {
                    Name = "Full Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                var budget2Entity = new Entities.Budget
                {
                    Name = "Half Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budget1Entity);
                sqliteMemoryWrapper.DbContext.Budgets.Add(budget2Entity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budget1TransactionEntities = new Entities.BudgetTransaction[4]
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 200m,
                        IsInitial = true,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 100m,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 50m,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        IsSurplus = true,
                        Budget = budget1Entity
                    }
                };

                var budget2TransactionEntities = new Entities.BudgetTransaction[4]
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        IsInitial = true,
                        Budget = budget2Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 50m,
                        Budget = budget2Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 25m,
                        Budget = budget2Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        IsSurplus = true,
                        Budget = budget2Entity
                    }
                };

                sqliteMemoryWrapper.DbContext.BudgetTransactions.AddRange(budget1TransactionEntities);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.AddRange(budget2TransactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                List<Entities.Budget> budgetEntitiesBeforeDelete = sqliteMemoryWrapper.DbContext.Budgets.ToList();
                List<Entities.BudgetTransaction> budgetTransactionEntitiesBeforeDelete = sqliteMemoryWrapper.DbContext.BudgetTransactions.ToList();

                Assert.AreEqual(2, budgetEntitiesBeforeDelete.Count);
                Assert.AreEqual(8, budgetTransactionEntitiesBeforeDelete.Count);

                budgetService.Delete(budget1Entity.BudgetId);

                List<Entities.Budget> budgetEntitiesAfterDelete = sqliteMemoryWrapper.DbContext.Budgets.ToList();
                List<Entities.BudgetTransaction> budgetTransactionEntitiesAfterDelete = sqliteMemoryWrapper.DbContext.BudgetTransactions.ToList();

                Assert.AreEqual(1, budgetEntitiesAfterDelete.Count);
                Assert.AreEqual(4, budgetTransactionEntitiesAfterDelete.Count);
            }
        }

        [TestMethod]
        public void TestBudgetGetAllEmpty()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                IEnumerable<Budget> budgets = budgetService.GetAll();

                Assert.AreEqual(0, budgets.Count());
            }
        }

        [TestMethod]
        public void TestBudgetGetAllMany()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                Entities.Account groceriesExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesExpenseAccountEntity);

                var checkingToRentPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToGroceriesPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };
                var groceriesPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = groceriesPrepaymentAccountEntity,
                    DestinationAccount = groceriesExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(groceriesPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budget1Entity = new Entities.Budget
                {
                    Name = "Full Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                var budget2Entity = new Entities.Budget
                {
                    Name = "Half Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budget1Entity);
                sqliteMemoryWrapper.DbContext.Budgets.Add(budget2Entity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budget1TransactionEntities = new Entities.BudgetTransaction[4]
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 200m,
                        IsInitial = true,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 100m,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 50m,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        IsSurplus = true,
                        Budget = budget1Entity
                    }
                };

                var budget2TransactionEntities = new Entities.BudgetTransaction[4]
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        IsInitial = true,
                        Budget = budget2Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 50m,
                        Budget = budget2Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 25m,
                        Budget = budget2Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        IsSurplus = true,
                        Budget = budget2Entity
                    }
                };

                sqliteMemoryWrapper.DbContext.BudgetTransactions.AddRange(budget1TransactionEntities);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.AddRange(budget2TransactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                List<Budget> budgets = budgetService.GetAll().ToList();

                Assert.AreEqual(2, budgets.Count);

                List<BudgetTransaction> budget1Transactions = budgets[0].Transactions.ToList();
                List<BudgetTransaction> budget2Transactions = budgets[1].Transactions.ToList();

                Assert.AreEqual(2, budget1Transactions.Count);
                Assert.AreEqual(2, budget2Transactions.Count);

                // TODO: check the data in a way that is not order dependent
                //Assert.AreEqual(budget1Entity.Name, budgets[0].Name);
                //Assert.AreEqual(BudgetPeriod.Fortnightly, budgets[0].Period);
                //Assert.AreEqual(budget1TransactionEntities[0].CreditAccountId, 
                //    budgets[0].InitialTransaction.CreditAccount.AccountId);
                //Assert.AreEqual(budget1TransactionEntities[0].DebitAccountId,
                //    budgets[0].InitialTransaction.DebitAccount.AccountId);
                //Assert.AreEqual(budget1TransactionEntities[0].Amount,
                //    budgets[0].InitialTransaction.Amount);
                //Assert.AreEqual(budget1TransactionEntities[1].CreditAccountId,
                //    budget1Transactions[0].CreditAccount.AccountId);
                //Assert.AreEqual(budget1TransactionEntities[1].DebitAccountId,
                //    budget1Transactions[0].DebitAccount.AccountId);
                //Assert.AreEqual(budget1TransactionEntities[1].Amount,
                //    budget1Transactions[0].Amount);
                //Assert.AreEqual(budget1TransactionEntities[2].CreditAccountId,
                //    budget1Transactions[1].CreditAccount.AccountId);
                //Assert.AreEqual(budget1TransactionEntities[2].DebitAccountId,
                //    budget1Transactions[1].DebitAccount.AccountId);
                //Assert.AreEqual(budget1TransactionEntities[2].Amount,
                //    budget1Transactions[1].Amount);
                //Assert.AreEqual(budget1TransactionEntities[3].CreditAccountId,
                //    budgets[0].SurplusTransaction.CreditAccount.AccountId);
                //Assert.AreEqual(budget1TransactionEntities[3].DebitAccountId,
                //    budgets[0].SurplusTransaction.DebitAccount.AccountId);
                //Assert.AreEqual(budget1TransactionEntities[3].Amount,
                //    budgets[0].SurplusTransaction.Amount);

                //Assert.AreEqual(budget2Entity.Name, budgets[1].Name);
                //Assert.AreEqual(BudgetPeriod.Fortnightly, budgets[1].Period);
                //Assert.AreEqual(budget2TransactionEntities[0].CreditAccountId,
                //    budgets[1].InitialTransaction.CreditAccount.AccountId);
                //Assert.AreEqual(budget2TransactionEntities[0].DebitAccountId,
                //    budgets[1].InitialTransaction.DebitAccount.AccountId);
                //Assert.AreEqual(budget2TransactionEntities[0].Amount,
                //    budgets[1].InitialTransaction.Amount);
                //Assert.AreEqual(budget2TransactionEntities[1].CreditAccountId,
                //    budget2Transactions[0].CreditAccount.AccountId);
                //Assert.AreEqual(budget2TransactionEntities[1].DebitAccountId,
                //    budget2Transactions[0].DebitAccount.AccountId);
                //Assert.AreEqual(budget2TransactionEntities[1].Amount,
                //    budget2Transactions[0].Amount);
                //Assert.AreEqual(budget2TransactionEntities[2].CreditAccountId,
                //    budget2Transactions[1].CreditAccount.AccountId);
                //Assert.AreEqual(budget2TransactionEntities[2].DebitAccountId,
                //    budget2Transactions[1].DebitAccount.AccountId);
                //Assert.AreEqual(budget2TransactionEntities[2].Amount,
                //    budget2Transactions[1].Amount);
                //Assert.AreEqual(budget2TransactionEntities[3].CreditAccountId,
                //    budgets[1].SurplusTransaction.CreditAccount.AccountId);
                //Assert.AreEqual(budget2TransactionEntities[3].DebitAccountId,
                //    budgets[1].SurplusTransaction.DebitAccount.AccountId);
                //Assert.AreEqual(budget2TransactionEntities[3].Amount,
                //    budgets[1].SurplusTransaction.Amount);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestBudgetGetFailInvalidId()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                Budget budgets = budgetService.Get(666);
            }
        }

        [TestMethod]
        public void TestBudgetGet()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);

                var checkingToRentPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budget1Entity = new Entities.Budget
                {
                    Name = "Full Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budget1Entity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetTransactionEntities = new Entities.BudgetTransaction[3]
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 200m,
                        IsInitial = true,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 100m,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = savingsAccountEntity,
                        IsSurplus = true,
                        Budget = budget1Entity
                    }
                };

                sqliteMemoryWrapper.DbContext.BudgetTransactions.AddRange(budgetTransactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                Budget budget = budgetService.Get(budget1Entity.BudgetId);

                List<BudgetTransaction> budgetTransactions = budget.Transactions.ToList();

                Assert.AreEqual(1, budgetTransactions.Count);

                Assert.AreEqual(budget1Entity.Name, budget.Name);
                Assert.AreEqual(BudgetPeriod.Fortnightly, budget.Period);
                Assert.AreEqual(budgetTransactionEntities[0].CreditAccountId,
                    budget.InitialTransaction.CreditAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[0].DebitAccountId,
                    budget.InitialTransaction.DebitAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[0].Amount,
                    budget.InitialTransaction.Amount);
                Assert.AreEqual(budgetTransactionEntities[1].CreditAccountId,
                    budgetTransactions[0].CreditAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[1].DebitAccountId,
                    budgetTransactions[0].DebitAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[1].Amount,
                    budgetTransactions[0].Amount);
                Assert.AreEqual(budgetTransactionEntities[2].CreditAccountId,
                    budget.SurplusTransaction.CreditAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[2].DebitAccountId,
                    budget.SurplusTransaction.DebitAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[2].Amount,
                    budget.SurplusTransaction.Amount);
            }
        }

        [TestMethod]
        public void TestBudgetUpdate()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);

                var checkingToRentPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetEntity = new Entities.Budget
                {
                    Name = "Full Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budgetEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetTransactionEntities = new Entities.BudgetTransaction[3]
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 200m,
                        IsInitial = true,
                        Budget = budgetEntity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 100m,
                        Budget = budgetEntity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = savingsAccountEntity,
                        IsSurplus = true,
                        Budget = budgetEntity
                    }
                };

                sqliteMemoryWrapper.DbContext.BudgetTransactions.AddRange(budgetTransactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                Budget budget = budgetService.Get(budgetEntity.BudgetId);

                // Change *everything*
                budget.Name = "Updated";
                budget.Period = BudgetPeriod.Weekly;
                budget.InitialTransaction.Amount = 50m;
                budget.InitialTransaction.CreditAccount.AccountId = savingsAccountEntity.AccountId;
                budget.InitialTransaction.DebitAccount.AccountId = incomeAccountEntity.AccountId;
                BudgetTransaction transaction = budget.Transactions.ElementAt(0);
                transaction.Amount = 25m;
                transaction.CreditAccount.AccountId = savingsAccountEntity.AccountId;
                transaction.DebitAccount.AccountId = checkingAccountEntity.AccountId;
                budget.SurplusTransaction.CreditAccount.AccountId = savingsAccountEntity.AccountId;
                budget.SurplusTransaction.DebitAccount.AccountId = checkingAccountEntity.AccountId;

                budgetService.Update(budget);

                List<Entities.Budget> updatedBudgetEntities = sqliteMemoryWrapper.DbContext.Budgets.ToList();
                List<Entities.BudgetTransaction> updatedBudgetTransactionEntities =
                    sqliteMemoryWrapper.DbContext.BudgetTransactions.ToList();

                Assert.AreEqual(1, updatedBudgetEntities.Count);
                Assert.AreEqual(3, updatedBudgetTransactionEntities.Count);
                Assert.AreEqual(budget.Name, updatedBudgetEntities[0].Name);
                Assert.AreEqual(BudgetPeriod.Weekly, updatedBudgetEntities[0].Period);
                // TODO: check the data in a way that is not order dependent
                //Assert.AreEqual(budget.InitialTransaction.Amount, 
                //    updatedBudgetTransactionEntities[0].Amount);
                //Assert.AreEqual(budget.InitialTransaction.CreditAccount.AccountId, 
                //    updatedBudgetTransactionEntities[0].CreditAccount.AccountId);
                //Assert.AreEqual(budget.InitialTransaction.DebitAccount.AccountId, 
                //    updatedBudgetTransactionEntities[0].DebitAccount.AccountId);
                //Assert.AreEqual(transaction.Amount, updatedBudgetTransactionEntities[1].Amount);
                //Assert.AreEqual(transaction.CreditAccount.AccountId,
                //    updatedBudgetTransactionEntities[1].CreditAccount.AccountId);
                //Assert.AreEqual(transaction.DebitAccount.AccountId,
                //    updatedBudgetTransactionEntities[1].DebitAccount.AccountId);
                //Assert.AreEqual(budget.SurplusTransaction.Amount, 
                //    updatedBudgetTransactionEntities[2].Amount);
                //Assert.AreEqual(budget.SurplusTransaction.CreditAccount.AccountId,
                //    updatedBudgetTransactionEntities[2].CreditAccount.AccountId);
                //Assert.AreEqual(budget.SurplusTransaction.DebitAccount.AccountId,
                //    updatedBudgetTransactionEntities[2].DebitAccount.AccountId);
            }
        }

        [TestMethod]
        public void TestBudgetUpdateRemoveTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);

                var checkingToRentPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetEntity = new Entities.Budget
                {
                    Name = "Full Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budgetEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetTransactionEntities = new Entities.BudgetTransaction[3]
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 200m,
                        IsInitial = true,
                        Budget = budgetEntity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 100m,
                        Budget = budgetEntity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = savingsAccountEntity,
                        IsSurplus = true,
                        Budget = budgetEntity
                    }
                };

                sqliteMemoryWrapper.DbContext.BudgetTransactions.AddRange(budgetTransactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                Budget budget = budgetService.Get(budgetEntity.BudgetId);

                // Remove the one 'regular' transaction
                budget.Transactions = new BudgetTransaction[0];

                budgetService.Update(budget);

                List<Entities.Budget> updatedBudgetEntities = sqliteMemoryWrapper.DbContext.Budgets.ToList();
                List<Entities.BudgetTransaction> updatedBudgetTransactionEntities =
                    sqliteMemoryWrapper.DbContext.BudgetTransactions.ToList();

                Assert.AreEqual(1, updatedBudgetEntities.Count);
                Assert.AreEqual(2, updatedBudgetTransactionEntities.Count);
                Assert.AreEqual(budget.Name, updatedBudgetEntities[0].Name);
                Assert.AreEqual(BudgetPeriod.Fortnightly, updatedBudgetEntities[0].Period);
                Assert.AreEqual(budget.InitialTransaction.Amount,
                    updatedBudgetTransactionEntities[0].Amount);
                Assert.AreEqual(budget.InitialTransaction.CreditAccount.AccountId,
                    updatedBudgetTransactionEntities[0].CreditAccount.AccountId);
                Assert.AreEqual(budget.InitialTransaction.DebitAccount.AccountId,
                    updatedBudgetTransactionEntities[0].DebitAccount.AccountId);
                Assert.AreEqual(budget.SurplusTransaction.Amount,
                    updatedBudgetTransactionEntities[1].Amount);
                Assert.AreEqual(budget.SurplusTransaction.CreditAccount.AccountId,
                    updatedBudgetTransactionEntities[1].CreditAccount.AccountId);
                Assert.AreEqual(budget.SurplusTransaction.DebitAccount.AccountId,
                    updatedBudgetTransactionEntities[1].DebitAccount.AccountId);
            }
        }

        [TestMethod]
        public void TestBudgetUpdateAddTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);

                var checkingToRentPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetEntity = new Entities.Budget
                {
                    Name = "Full Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budgetEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetTransactionEntities = new Entities.BudgetTransaction[3]
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 200m,
                        IsInitial = true,
                        Budget = budgetEntity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 100m,
                        Budget = budgetEntity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = savingsAccountEntity,
                        IsSurplus = true,
                        Budget = budgetEntity
                    }
                };

                sqliteMemoryWrapper.DbContext.BudgetTransactions.AddRange(budgetTransactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                Budget budget = budgetService.Get(budgetEntity.BudgetId);

                // Add one 'regular' transaction
                List<BudgetTransaction> transactions = budget.Transactions.ToList();
                var newTransaction = new BudgetTransaction
                {
                    CreditAccount = new AccountLink { AccountId = checkingAccountEntity.AccountId },
                    DebitAccount = new AccountLink { AccountId = rentPrepaymentAccountEntity.AccountId },
                    Amount = 20m
                };
                transactions.Add(newTransaction);
                budget.Transactions = transactions;

                budgetService.Update(budget);

                List<Entities.Budget> updatedBudgetEntities = sqliteMemoryWrapper.DbContext.Budgets.ToList();
                List<Entities.BudgetTransaction> updatedBudgetTransactionEntities =
                    sqliteMemoryWrapper.DbContext.BudgetTransactions.ToList();

                Assert.AreEqual(1, updatedBudgetEntities.Count);
                Assert.AreEqual(4, updatedBudgetTransactionEntities.Count);
                Assert.AreEqual(budget.Name, updatedBudgetEntities[0].Name);
                Assert.AreEqual(BudgetPeriod.Fortnightly, updatedBudgetEntities[0].Period);
                // TODO: check the data in a way that is not order dependent
                //Assert.AreEqual(budget.InitialTransaction.Amount,
                //    updatedBudgetTransactionEntities[0].Amount);
                //Assert.AreEqual(budget.InitialTransaction.CreditAccount.AccountId,
                //    updatedBudgetTransactionEntities[0].CreditAccount.AccountId);
                //Assert.AreEqual(budget.InitialTransaction.DebitAccount.AccountId,
                //    updatedBudgetTransactionEntities[0].DebitAccount.AccountId);
                //Assert.AreEqual(transactions[0].Amount,
                //    updatedBudgetTransactionEntities[1].Amount);
                //Assert.AreEqual(transactions[0].CreditAccount.AccountId,
                //    updatedBudgetTransactionEntities[1].CreditAccount.AccountId);
                //Assert.AreEqual(transactions[0].DebitAccount.AccountId,
                //    updatedBudgetTransactionEntities[1].DebitAccount.AccountId);
                //Assert.AreEqual(budget.SurplusTransaction.Amount,
                //    updatedBudgetTransactionEntities[2].Amount);
                //Assert.AreEqual(budget.SurplusTransaction.CreditAccount.AccountId,
                //    updatedBudgetTransactionEntities[2].CreditAccount.AccountId);
                //Assert.AreEqual(budget.SurplusTransaction.DebitAccount.AccountId,
                //    updatedBudgetTransactionEntities[2].DebitAccount.AccountId);
                //Assert.AreEqual(transactions[1].Amount,
                //    updatedBudgetTransactionEntities[3].Amount);
                //Assert.AreEqual(transactions[1].CreditAccount.AccountId,
                //    updatedBudgetTransactionEntities[3].CreditAccount.AccountId);
                //Assert.AreEqual(transactions[1].DebitAccount.AccountId,
                //    updatedBudgetTransactionEntities[3].DebitAccount.AccountId);
            }
        }

        [TestMethod]
        public void TestBudgetMakePaydayTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);

                var checkingToRentPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budget1Entity = new Entities.Budget
                {
                    Name = "Full Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budget1Entity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetTransactionEntities = new Entities.BudgetTransaction[3]
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 200m,
                        IsInitial = true,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 100m,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = savingsAccountEntity,
                        IsSurplus = true,
                        Budget = budget1Entity
                    }
                };

                sqliteMemoryWrapper.DbContext.BudgetTransactions.AddRange(budgetTransactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                var paydayStart = new PaydayStart
                {
                    AmountPaid = 180m,
                    At = new DateTime(2018, 1, 1, 9, 0, 0),
                    IncludeSurplus = true
                };
                List<Transaction> transactions = 
                    budgetService.MakePaydayTransactions(
                        budget1Entity.BudgetId, 
                        paydayStart)
                    .ToList();

                Assert.AreEqual(3, transactions.Count);

                Assert.AreEqual(budgetTransactionEntities[0].CreditAccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[0].DebitAccountId, transactions[0].DebitAccount.AccountId);
                Assert.AreEqual(paydayStart.AmountPaid, transactions[0].Amount);
                Assert.AreEqual(paydayStart.At, transactions[0].At);
                Assert.AreEqual(budgetTransactionEntities[1].CreditAccountId, transactions[1].CreditAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[1].DebitAccountId, transactions[1].DebitAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[1].Amount, transactions[1].Amount);
                Assert.AreEqual(paydayStart.At, transactions[1].At);
                Assert.AreEqual(budgetTransactionEntities[2].CreditAccountId, transactions[2].CreditAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[2].DebitAccountId, transactions[2].DebitAccount.AccountId);
                Assert.AreEqual(transactions[0].Amount - transactions[1].Amount, transactions[2].Amount);
                Assert.AreEqual(paydayStart.At, transactions[2].At);
            }
        }

        [TestMethod]
        public void TestBudgetMakePaydayTransactionsSkipSurplus()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);

                var checkingToRentPrepaymentRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationshipEntity = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationshipEntity);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationshipEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budget1Entity = new Entities.Budget
                {
                    Name = "Full Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budget1Entity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetTransactionEntities = new Entities.BudgetTransaction[3]
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 200m,
                        IsInitial = true,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 100m,
                        Budget = budget1Entity
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = savingsAccountEntity,
                        IsSurplus = true,
                        Budget = budget1Entity
                    }
                };

                sqliteMemoryWrapper.DbContext.BudgetTransactions.AddRange(budgetTransactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budgetService = new BudgetService(loggerFactory, sqliteMemoryWrapper.DbContext);

                var paydayStart = new PaydayStart
                {
                    AmountPaid = 180m,
                    At = new DateTime(2018, 1, 1, 9, 0, 0),
                    IncludeSurplus = false
                };
                List<Transaction> transactions =
                    budgetService.MakePaydayTransactions(
                        budget1Entity.BudgetId,
                        paydayStart)
                    .ToList();

                Assert.AreEqual(2, transactions.Count);

                Assert.AreEqual(budgetTransactionEntities[0].CreditAccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[0].DebitAccountId, transactions[0].DebitAccount.AccountId);
                Assert.AreEqual(paydayStart.AmountPaid, transactions[0].Amount);
                Assert.AreEqual(paydayStart.At, transactions[0].At);
                Assert.AreEqual(budgetTransactionEntities[1].CreditAccountId, transactions[1].CreditAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[1].DebitAccountId, transactions[1].DebitAccount.AccountId);
                Assert.AreEqual(budgetTransactionEntities[1].Amount, transactions[1].Amount);
                Assert.AreEqual(paydayStart.At, transactions[1].At);
            }
        }
    }
}
