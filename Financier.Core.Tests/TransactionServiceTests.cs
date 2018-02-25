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
    public class TransactionServiceTests
    {
        [TestMethod]
        public void TestCreateTransaction()
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
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(), 
                    sqliteMemoryWrapper.DbContext
                );
                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var newTransaction = new Transaction
                {
                    CreditAccount = accountService.GetAsLink(incomeAccountEntity.AccountId),
                    DebitAccount = accountService.GetAsLink(checkingAccountEntity.AccountId),
                    Amount = 100m,
                    At = new DateTime(2018, 1, 1, 9, 0, 0)
                };
                transactionService.Create(newTransaction);

                List<Entities.Transaction> transactionEntities = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(1, transactionEntities.Count);
                Assert.AreEqual(newTransaction.TransactionId, transactionEntities[0].TransactionId);
                Assert.AreEqual(newTransaction.CreditAccount.AccountId, transactionEntities[0].CreditAccount.AccountId);
                Assert.AreEqual(newTransaction.DebitAccount.AccountId, transactionEntities[0].DebitAccount.AccountId);
                Assert.AreEqual(newTransaction.Amount, transactionEntities[0].Amount);
                Assert.AreEqual(newTransaction.At, transactionEntities[0].At);
            }
        }

        [TestMethod]
        public void TestCreateManyTransactions()
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
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var newTransactions = new Transaction[]
                {
                    new Transaction {
                        CreditAccount = accountService.GetAsLink(incomeAccountEntity.AccountId),
                        DebitAccount = accountService.GetAsLink(checkingAccountEntity.AccountId),
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 9, 0, 0)
                    },
                    new Transaction {
                        CreditAccount = accountService.GetAsLink(incomeAccountEntity.AccountId),
                        DebitAccount = accountService.GetAsLink(checkingAccountEntity.AccountId),
                        Amount = 70m,
                        At = new DateTime(2018, 1, 1, 9, 25, 0)
                    }
                };
                transactionService.CreateMany(newTransactions);

                List<Entities.Transaction> transactionEntities = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(2, transactionEntities.Count);
                Assert.AreEqual(newTransactions[0].TransactionId, transactionEntities[0].TransactionId);
                Assert.AreEqual(newTransactions[0].CreditAccount.AccountId, transactionEntities[0].CreditAccount.AccountId);
                Assert.AreEqual(newTransactions[0].DebitAccount.AccountId, transactionEntities[0].DebitAccount.AccountId);
                Assert.AreEqual(newTransactions[0].Amount, transactionEntities[0].Amount);
                Assert.AreEqual(newTransactions[0].At, transactionEntities[0].At);
                Assert.AreEqual(newTransactions[1].TransactionId, transactionEntities[1].TransactionId);
                Assert.AreEqual(newTransactions[1].CreditAccount.AccountId, transactionEntities[1].CreditAccount.AccountId);
                Assert.AreEqual(newTransactions[1].DebitAccount.AccountId, transactionEntities[1].DebitAccount.AccountId);
                Assert.AreEqual(newTransactions[1].Amount, transactionEntities[1].Amount);
                Assert.AreEqual(newTransactions[1].At, transactionEntities[1].At);
            }
        }

        [TestMethod]
        public void TestGetAllTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<TransactionService> logger = loggerFactory.CreateLogger<TransactionService>();

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
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 60m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToGroceriesPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactionService = new TransactionService(logger, sqliteMemoryWrapper.DbContext);

                List<Transaction> transactions = transactionService.GetAll().ToList();

                Assert.AreEqual(3, transactions.Count);
                Assert.AreEqual(1, transactions[0].TransactionId);
                Assert.AreEqual(incomeAccountEntity.AccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(incomeAccountEntity.Name, transactions[0].CreditAccount.Name);
                Assert.AreEqual(AccountType.Income, transactions[0].CreditAccount.Type);
                Assert.AreEqual(checkingAccountEntity.AccountId, transactions[0].DebitAccount.AccountId);
                Assert.AreEqual(checkingAccountEntity.Name, transactions[0].DebitAccount.Name);
                Assert.AreEqual(AccountType.Asset, transactions[0].DebitAccount.Type);
                Assert.AreEqual(transactionEntities[0].Amount, transactions[0].Amount);
                Assert.AreEqual(transactionEntities[0].At, transactions[0].At);

                Assert.AreEqual(2, transactions[1].TransactionId);
                Assert.AreEqual(checkingAccountEntity.AccountId, transactions[1].CreditAccount.AccountId);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, transactions[1].DebitAccount.AccountId);
                Assert.AreEqual(transactionEntities[1].Amount, transactions[1].Amount);
                Assert.AreEqual(transactionEntities[1].At, transactions[1].At);

                Assert.AreEqual(3, transactions[2].TransactionId);
                Assert.AreEqual(checkingAccountEntity.AccountId, transactions[2].CreditAccount.AccountId);
                Assert.AreEqual(groceriesPrepaymentAccountEntity.AccountId, transactions[2].DebitAccount.AccountId);
                Assert.AreEqual(transactionEntities[2].Amount, transactions[2].Amount);
                Assert.AreEqual(transactionEntities[2].At, transactions[2].At);
            }
        }

        [TestMethod]
        public void TestGetAllTransactionsFilteredByAccount()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<TransactionService> logger = loggerFactory.CreateLogger<TransactionService>();

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
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 60m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToGroceriesPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactionService = new TransactionService(logger, sqliteMemoryWrapper.DbContext);

                int[] accountIds = new int[1] { rentPrepaymentAccountEntity.AccountId };
                List<Transaction> transactions = transactionService.GetAll(accountIds).ToList();

                Assert.AreEqual(1, transactions.Count);

                Assert.AreEqual(2, transactions[0].TransactionId);
                Assert.AreEqual(checkingAccountEntity.AccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, transactions[0].DebitAccount.AccountId);
                Assert.AreEqual(transactionEntities[1].Amount, transactions[0].Amount);
                Assert.AreEqual(transactionEntities[1].At, transactions[0].At);
            }
        }

        [TestMethod]
        public void TestGetAllTransactionsFilteredByAccountAndDateRange()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<TransactionService> logger = loggerFactory.CreateLogger<TransactionService>();

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
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 60m,
                        At = new DateTime(2018, 1, 1, 8, 35, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 9, 45, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 11, 15, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToGroceriesPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactionService = new TransactionService(logger, sqliteMemoryWrapper.DbContext);

                int[] accountIds = new int[1] { rentPrepaymentAccountEntity.AccountId };
                List<Transaction> transactions = transactionService
                    .GetAll(
                        accountIds, 
                        new DateTime(2018, 1, 1, 8, 35, 0), 
                        new DateTime(2018, 1, 1, 10, 0, 0)
                    )
                    .ToList();

                Assert.AreEqual(2, transactions.Count);

                Assert.AreEqual(2, transactions[0].TransactionId);
                Assert.AreEqual(checkingAccountEntity.AccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, transactions[0].DebitAccount.AccountId);
                Assert.AreEqual(transactionEntities[1].Amount, transactions[0].Amount);
                Assert.AreEqual(transactionEntities[1].At, transactions[0].At);
                Assert.AreEqual(checkingAccountEntity.AccountId, transactions[1].CreditAccount.AccountId);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, transactions[1].DebitAccount.AccountId);
                Assert.AreEqual(transactionEntities[2].Amount, transactions[1].Amount);
                Assert.AreEqual(transactionEntities[2].At, transactions[1].At);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetTransactionFailInvalidId()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<TransactionService> logger = loggerFactory.CreateLogger<TransactionService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var transactionService = new TransactionService(logger, sqliteMemoryWrapper.DbContext);

                Transaction transaction = transactionService.Get(666);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestTransactionDeleteFailInvalidId()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<TransactionService> logger = loggerFactory.CreateLogger<TransactionService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var transactionService = new TransactionService(logger, sqliteMemoryWrapper.DbContext);

                transactionService.Delete(666);
            }
        }

        [TestMethod]
        public void TestTransactionDelete()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<TransactionService> logger = loggerFactory.CreateLogger<TransactionService>();

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
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 60m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactionService = new TransactionService(logger, sqliteMemoryWrapper.DbContext);

                transactionService.Delete(transactionEntities[0].TransactionId);

                List<Transaction> transactions = transactionService.GetAll().ToList();

                Assert.AreEqual(1, transactions.Count);
                Assert.AreEqual(transactionEntities[1].TransactionId, transactions[0].TransactionId);
                Assert.AreEqual(transactionEntities[1].Amount, transactions[0].Amount);
                Assert.AreEqual(transactionEntities[1].At, transactions[0].At);
                Assert.AreEqual(transactionEntities[1].CreditAccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(transactionEntities[1].DebitAccountId, transactions[0].DebitAccount.AccountId);
            }
        }

        [TestMethod]
        public void TestGetPendingCreditCardTransactionsNoResults()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                Entities.Currency usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account creditCardAccountEntity =
                    accountFactory.Create(AccountPrefab.CreditCard, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, creditCardAccountEntity);

                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactionsToAdd = new Entities.Transaction[4]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 1)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 50m,
                        At = new DateTime(2018, 1, 1, 8, 30, 2)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = rentExpenseAccountEntity,
                        Amount = 20m,
                        At = new DateTime(2018, 1, 1, 8, 30, 3)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = rentPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 20m,
                        At = new DateTime(2018, 1, 1, 8, 30, 4)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionsToAdd);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var expenseToPaymentRelationship = new Entities.TransactionRelationship
                {
                    SourceTransaction = transactionsToAdd[2],
                    DestinationTransaction = transactionsToAdd[3],
                    Type = TransactionRelationshipType.CreditCardPayment
                };

                sqliteMemoryWrapper.DbContext.TransactionRelationships.Add(expenseToPaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                ILoggerFactory loggerFactory = new LoggerFactory();
                ILogger<TransactionService> logger = loggerFactory.CreateLogger<TransactionService>();
                TransactionService transactionService = new TransactionService(logger, sqliteMemoryWrapper.DbContext);

                List<Payment> pendingPayments = 
                    transactionService.GetPendingCreditCardPayments(creditCardAccountEntity.AccountId).ToList();

                Assert.AreEqual(0, pendingPayments.Count);
            }
        }

        [TestMethod]
        public void TestGetPendingCreditCardTransactionsOneResult()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                Entities.Currency usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account creditCardAccountEntity =
                    accountFactory.Create(AccountPrefab.CreditCard, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, creditCardAccountEntity);

                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactionsToAdd = new Entities.Transaction[4]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 1)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 50m,
                        At = new DateTime(2018, 1, 1, 8, 30, 2)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = rentExpenseAccountEntity,
                        Amount = 20m,
                        At = new DateTime(2018, 1, 1, 8, 30, 3)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = rentPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 20m,
                        At = new DateTime(2018, 1, 1, 8, 30, 4)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionsToAdd);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                ILoggerFactory loggerFactory = new LoggerFactory();
                ILogger<TransactionService> logger = loggerFactory.CreateLogger<TransactionService>();
                TransactionService transactionService = new TransactionService(logger, sqliteMemoryWrapper.DbContext);

                List<Payment> pendingPayments =
                    transactionService.GetPendingCreditCardPayments(creditCardAccountEntity.AccountId).ToList();

                Assert.AreEqual(1, pendingPayments.Count);
                Assert.AreEqual(creditCardAccountEntity.AccountId, pendingPayments[0].OriginalTransaction.CreditAccount.AccountId);
                Assert.AreEqual(rentExpenseAccountEntity.AccountId, pendingPayments[0].OriginalTransaction.DebitAccount.AccountId);
                Assert.AreEqual(transactionsToAdd[2].Amount, pendingPayments[0].OriginalTransaction.Amount);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, pendingPayments[0].PaymentTransaction.CreditAccount.AccountId);
                Assert.AreEqual(creditCardAccountEntity.AccountId, pendingPayments[0].PaymentTransaction.DebitAccount.AccountId);
                Assert.AreEqual(transactionsToAdd[2].Amount, pendingPayments[0].PaymentTransaction.Amount);
            }
        }
    }
}
