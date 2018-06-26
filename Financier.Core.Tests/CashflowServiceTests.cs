using Financier.Services;
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
    public class CashflowServiceTests
    {
        [TestMethod]
        public void TestGenerateCashflowStatementOnePeriodMultipleAccounts()
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
                Entities.Account creditCardAccountEntity =
                    accountFactory.Create(AccountPrefab.CreditCard, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account groceriesExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesExpense, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);

                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, creditCardAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 50m,
                        At = new DateTime(2018, 1, 2, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 20m,
                        At = new DateTime(2018, 1, 3, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 5m,
                        At = new DateTime(2018, 1, 4, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 8m,
                        At = new DateTime(2018, 1, 5, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 3m,
                        At = new DateTime(2018, 1, 6, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = groceriesPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 5m,
                        At = new DateTime(2018, 1, 7, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = groceriesPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 8m,
                        At = new DateTime(2018, 1, 7, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = groceriesPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 3m,
                        At = new DateTime(2018, 1, 7, 9, 0, 0)
                    },
                    new Entities.Transaction // this should be excluded from the report
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 1m,
                        At = new DateTime(2018, 1, 10, 9, 0, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountRelationshipEntities = new Entities.AccountRelationship[]
                {
                    new Entities.AccountRelationship
                    {
                        SourceAccount = checkingAccountEntity,
                        DestinationAccount = rentPrepaymentAccountEntity,
                        Type = AccountRelationshipType.PhysicalToLogical
                    },
                    new Entities.AccountRelationship
                    {
                        SourceAccount = checkingAccountEntity,
                        DestinationAccount = groceriesPrepaymentAccountEntity,
                        Type = AccountRelationshipType.PhysicalToLogical
                    },
                    new Entities.AccountRelationship
                    {
                        SourceAccount = rentPrepaymentAccountEntity,
                        DestinationAccount = rentExpenseAccountEntity,
                        Type = AccountRelationshipType.PrepaymentToExpense
                    },
                    new Entities.AccountRelationship
                    {
                        SourceAccount = groceriesPrepaymentAccountEntity,
                        DestinationAccount = groceriesExpenseAccountEntity,
                        Type = AccountRelationshipType.PrepaymentToExpense
                    }
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.AddRange(accountRelationshipEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountRelationshipService = new AccountRelationshipService(
                    loggerFactory.CreateLogger<AccountRelationshipService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var currencyService = new CurrencyService(
                    loggerFactory.CreateLogger<CurrencyService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var cashflowService = new CashflowService(
                    loggerFactory.CreateLogger<CashflowService>(),
                    accountRelationshipService,
                    accountService,
                    currencyService,
                    transactionService
                );

                CashflowStatement cashflowStatement = 
                    cashflowService.Generate(CashflowPeriod.Fortnightly, new DateTime(2018, 1, 1), new DateTime(2018, 1, 8));
                List<CashflowAccount> cashflowAccounts = 
                    cashflowStatement
                        .Accounts
                        .OrderBy(i => i.Name)
                        .ToList();

                Assert.AreEqual(2, cashflowAccounts.Count);
                Assert.AreEqual("Groceries", cashflowAccounts[0].Name);
                Assert.AreEqual(1, cashflowAccounts[0].Periods.Count());
                Assert.AreEqual(20m, cashflowAccounts[0].Periods.ElementAt(0).Inflow);
                Assert.AreEqual(16m, cashflowAccounts[0].Periods.ElementAt(0).Outflow);
                Assert.AreEqual(4m, cashflowAccounts[0].Periods.ElementAt(0).Cashflow);
                Assert.AreEqual(20m, cashflowAccounts[0].Inflow);
                Assert.AreEqual(16m, cashflowAccounts[0].Outflow);
                Assert.AreEqual(4m, cashflowAccounts[0].Cashflow);
                Assert.AreEqual("Rent", cashflowAccounts[1].Name);
                Assert.AreEqual(1, cashflowAccounts[1].Periods.Count());
                Assert.AreEqual(50m, cashflowAccounts[1].Periods.ElementAt(0).Inflow);
                Assert.AreEqual(0m, cashflowAccounts[1].Periods.ElementAt(0).Outflow);
                Assert.AreEqual(50m, cashflowAccounts[1].Periods.ElementAt(0).Cashflow);
                Assert.AreEqual(50m, cashflowAccounts[1].Inflow);
                Assert.AreEqual(0m, cashflowAccounts[1].Outflow);
                Assert.AreEqual(50m, cashflowAccounts[1].Cashflow);
            }
        }

        [TestMethod]
        public void TestGenerateCashflowStatementMultiplePeriodsMultipleAccounts()
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
                Entities.Account creditCardAccountEntity =
                    accountFactory.Create(AccountPrefab.CreditCard, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account groceriesExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesExpense, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);

                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, creditCardAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 50m,
                        At = new DateTime(2018, 1, 2, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 20m,
                        At = new DateTime(2018, 1, 3, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 5m,
                        At = new DateTime(2018, 1, 4, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 8m,
                        At = new DateTime(2018, 1, 5, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 3m,
                        At = new DateTime(2018, 1, 6, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = groceriesPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 5m,
                        At = new DateTime(2018, 1, 7, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = groceriesPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 8m,
                        At = new DateTime(2018, 1, 8, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = groceriesPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 3m,
                        At = new DateTime(2018, 1, 9, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 80m,
                        At = new DateTime(2018, 1, 14, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 50m,
                        At = new DateTime(2018, 1, 15, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 20m,
                        At = new DateTime(2018, 1, 16, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 3m,
                        At = new DateTime(2018, 1, 17, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 7m,
                        At = new DateTime(2018, 1, 18, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 12m,
                        At = new DateTime(2018, 1, 19, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = groceriesPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 3m,
                        At = new DateTime(2018, 1, 20, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = groceriesPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 7m,
                        At = new DateTime(2018, 1, 21, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = groceriesPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 12m,
                        At = new DateTime(2018, 1, 22, 9, 0, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = rentPrepaymentAccountEntity,
                        DebitAccount = rentExpenseAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 27, 9, 0, 0)
                    },
                    new Entities.Transaction // this should be excluded from the report
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = groceriesExpenseAccountEntity,
                        Amount = 7m,
                        At = new DateTime(2018, 1, 31, 9, 0, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountRelationshipEntities = new Entities.AccountRelationship[]
                {
                    new Entities.AccountRelationship
                    {
                        SourceAccount = checkingAccountEntity,
                        DestinationAccount = rentPrepaymentAccountEntity,
                        Type = AccountRelationshipType.PhysicalToLogical
                    },
                    new Entities.AccountRelationship
                    {
                        SourceAccount = checkingAccountEntity,
                        DestinationAccount = groceriesPrepaymentAccountEntity,
                        Type = AccountRelationshipType.PhysicalToLogical
                    },
                    new Entities.AccountRelationship
                    {
                        SourceAccount = rentPrepaymentAccountEntity,
                        DestinationAccount = rentExpenseAccountEntity,
                        Type = AccountRelationshipType.PrepaymentToExpense
                    },
                    new Entities.AccountRelationship
                    {
                        SourceAccount = groceriesPrepaymentAccountEntity,
                        DestinationAccount = groceriesExpenseAccountEntity,
                        Type = AccountRelationshipType.PrepaymentToExpense
                    }
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.AddRange(accountRelationshipEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountRelationshipService = new AccountRelationshipService(
                    loggerFactory.CreateLogger<AccountRelationshipService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var currencyService = new CurrencyService(
                    loggerFactory.CreateLogger<CurrencyService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var cashflowService = new CashflowService(
                    loggerFactory.CreateLogger<CashflowService>(),
                    accountRelationshipService,
                    accountService,
                    currencyService,
                    transactionService
                );

                CashflowStatement cashflowStatement =
                    cashflowService.Generate(CashflowPeriod.Fortnightly, new DateTime(2018, 1, 1), new DateTime(2018, 1, 28));
                List<CashflowAccount> cashflowAccounts =
                    cashflowStatement
                        .Accounts
                        .OrderBy(i => i.Name)
                        .ToList();

                Assert.AreEqual(2, cashflowAccounts.Count);
                Assert.AreEqual("Groceries", cashflowAccounts[0].Name);
                Assert.AreEqual(2, cashflowAccounts[0].Periods.Count());
                Assert.AreEqual(20m, cashflowAccounts[0].Periods.ElementAt(0).Inflow);
                Assert.AreEqual(16m, cashflowAccounts[0].Periods.ElementAt(0).Outflow);
                Assert.AreEqual(4m, cashflowAccounts[0].Periods.ElementAt(0).Cashflow);
                Assert.AreEqual(20m, cashflowAccounts[0].Periods.ElementAt(1).Inflow);
                Assert.AreEqual(22m, cashflowAccounts[0].Periods.ElementAt(1).Outflow);
                Assert.AreEqual(-2m, cashflowAccounts[0].Periods.ElementAt(1).Cashflow);
                Assert.AreEqual(40m, cashflowAccounts[0].Inflow);
                Assert.AreEqual(38m, cashflowAccounts[0].Outflow);
                Assert.AreEqual(2m, cashflowAccounts[0].Cashflow);
                Assert.AreEqual("Rent", cashflowAccounts[1].Name);
                Assert.AreEqual(2, cashflowAccounts[1].Periods.Count());
                Assert.AreEqual(50m, cashflowAccounts[1].Periods.ElementAt(0).Inflow);
                Assert.AreEqual(0m, cashflowAccounts[1].Periods.ElementAt(0).Outflow);
                Assert.AreEqual(50m, cashflowAccounts[1].Periods.ElementAt(0).Cashflow);
                Assert.AreEqual(50m, cashflowAccounts[1].Periods.ElementAt(1).Inflow);
                Assert.AreEqual(100m, cashflowAccounts[1].Periods.ElementAt(1).Outflow);
                Assert.AreEqual(-50m, cashflowAccounts[1].Periods.ElementAt(1).Cashflow);
                Assert.AreEqual(100m, cashflowAccounts[1].Inflow);
                Assert.AreEqual(100m, cashflowAccounts[1].Outflow);
                Assert.AreEqual(0m, cashflowAccounts[1].Cashflow);
            }
        }

        // TODO - test other periods
    }
}