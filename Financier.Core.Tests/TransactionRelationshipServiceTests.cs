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
    public class TransactionRelationshipServiceTests
    {
        [TestMethod]
        public void TestCreateTransactionRelationship()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();

                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account creditCardAccountEntity =
                    accountFactory.Create(AccountPrefab.CreditCard, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, creditCardAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 60m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = rentExpenseAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 8, 32, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = rentPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 8, 33, 0)
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
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var transactionRelationshipService = new TransactionRelationshipService(
                    loggerFactory.CreateLogger<TransactionRelationshipService>(),
                    sqliteMemoryWrapper.DbContext
                );

                Transaction creditCardToExpenseTransaction = transactionService.Get(transactionEntities[1].TransactionId);
                Transaction prepaymentToCreditCardTransaction = transactionService.Get(transactionEntities[2].TransactionId);
                var newTransactionRelationship = new TransactionRelationship
                {
                    SourceTransaction = creditCardToExpenseTransaction,
                    DestinationTransaction = prepaymentToCreditCardTransaction,
                    Type = TransactionRelationshipType.CreditCardPayment
                };
                transactionRelationshipService.Create(newTransactionRelationship);

                List<Entities.TransactionRelationship> transactionRelationshipEntities = 
                    sqliteMemoryWrapper.DbContext.TransactionRelationships.ToList();

                Assert.AreEqual(1, transactionRelationshipEntities.Count);
                Assert.AreEqual(newTransactionRelationship.TransactionRelationshipId, transactionRelationshipEntities[0].TransactionRelationshipId);
                Assert.AreEqual(newTransactionRelationship.SourceTransaction.TransactionId, transactionRelationshipEntities[0].SourceTransaction.TransactionId);
                Assert.AreEqual(newTransactionRelationship.DestinationTransaction.TransactionId, transactionRelationshipEntities[0].DestinationTransaction.TransactionId);
                Assert.AreEqual(newTransactionRelationship.Type, transactionRelationshipEntities[0].Type);
            }
        }

        [TestMethod]
        public void TestCreateManyTransactionRelationships()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();

                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account creditCardAccountEntity =
                    accountFactory.Create(AccountPrefab.CreditCard, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, creditCardAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 60m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = rentExpenseAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 8, 32, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = rentPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 8, 33, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccountEntity,
                        DebitAccount = rentExpenseAccountEntity,
                        Amount = 20m,
                        At = new DateTime(2018, 1, 1, 8, 34, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = rentPrepaymentAccountEntity,
                        DebitAccount = creditCardAccountEntity,
                        Amount = 20m,
                        At = new DateTime(2018, 1, 1, 8, 35, 0)
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
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var transactionRelationshipService = new TransactionRelationshipService(
                    loggerFactory.CreateLogger<TransactionRelationshipService>(),
                    sqliteMemoryWrapper.DbContext
                );

                Transaction creditCardToExpenseTransaction1 = transactionService.Get(transactionEntities[1].TransactionId);
                Transaction prepaymentToCreditCardTransaction1 = transactionService.Get(transactionEntities[2].TransactionId);
                Transaction creditCardToExpenseTransaction2 = transactionService.Get(transactionEntities[3].TransactionId);
                Transaction prepaymentToCreditCardTransaction2 = transactionService.Get(transactionEntities[4].TransactionId);
                var newTransactionRelationships = new TransactionRelationship[2]
                {
                    new TransactionRelationship
                    {
                        SourceTransaction = creditCardToExpenseTransaction1,
                        DestinationTransaction = prepaymentToCreditCardTransaction1,
                        Type = TransactionRelationshipType.CreditCardPayment
                    },
                    new TransactionRelationship
                    {
                        SourceTransaction = creditCardToExpenseTransaction2,
                        DestinationTransaction = prepaymentToCreditCardTransaction2,
                        Type = TransactionRelationshipType.CreditCardPayment
                    }
                };
                transactionRelationshipService.CreateMany(newTransactionRelationships);

                List<Entities.TransactionRelationship> transactionRelationshipEntities =
                    sqliteMemoryWrapper.DbContext.TransactionRelationships.ToList();

                Assert.AreEqual(2, transactionRelationshipEntities.Count);
                Assert.AreEqual(newTransactionRelationships[0].TransactionRelationshipId, transactionRelationshipEntities[0].TransactionRelationshipId);
                Assert.AreEqual(newTransactionRelationships[0].SourceTransaction.TransactionId, transactionRelationshipEntities[0].SourceTransaction.TransactionId);
                Assert.AreEqual(newTransactionRelationships[0].DestinationTransaction.TransactionId, transactionRelationshipEntities[0].DestinationTransaction.TransactionId);
                Assert.AreEqual(newTransactionRelationships[0].Type, transactionRelationshipEntities[0].Type);
                Assert.AreEqual(newTransactionRelationships[1].TransactionRelationshipId, transactionRelationshipEntities[1].TransactionRelationshipId);
                Assert.AreEqual(newTransactionRelationships[1].SourceTransaction.TransactionId, transactionRelationshipEntities[1].SourceTransaction.TransactionId);
                Assert.AreEqual(newTransactionRelationships[1].DestinationTransaction.TransactionId, transactionRelationshipEntities[1].DestinationTransaction.TransactionId);
                Assert.AreEqual(newTransactionRelationships[1].Type, transactionRelationshipEntities[1].Type);
            }
        }
    }
}
