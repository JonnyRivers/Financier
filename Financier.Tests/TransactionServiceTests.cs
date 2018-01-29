using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class TransactionServiceTests
    {
        [TestMethod]
        public void TestGetAllTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<TransactionService> logger = loggerFactory.CreateLogger<TransactionService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
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
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
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
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
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
    }
}
