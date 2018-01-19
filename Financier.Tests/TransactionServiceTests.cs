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
                var usdCurrencyEntity = new Entities.Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$",
                    IsPrimary = true
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrencyEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var incomeAccountEntity = new Entities.Account
                {
                    Name = "Income",
                    Currency = usdCurrencyEntity,
                    Type = Entities.AccountType.Income
                };
                var checkingAccountEntity = new Entities.Account
                {
                    Name = "Checking",
                    Currency = usdCurrencyEntity,
                    Type = Entities.AccountType.Asset
                };
                var rentPrepaymentAccountEntity = new Entities.Account
                {
                    Name = "Rent Prepayment",
                    Currency = usdCurrencyEntity,
                    Type = Entities.AccountType.Asset
                };
                var groceriesPrepaymentAccountEntity = new Entities.Account
                {
                    Name = "Groceries Prepayment",
                    Currency = usdCurrencyEntity,
                    Type = Entities.AccountType.Asset
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccountEntity);
                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccountEntity);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentPrepaymentAccountEntity);
                sqliteMemoryWrapper.DbContext.Accounts.Add(groceriesPrepaymentAccountEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

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
                    Type = Entities.AccountRelationshipType.PhysicalToLogical
                };
                var checkingToGroceriesPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = Entities.AccountRelationshipType.PhysicalToLogical
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
    }
}
