using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class AccountRelationshipServiceTests
    {
        [TestMethod]
        public void TestCreateAccountRelationship()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountRelationshipService> logger = loggerFactory.CreateLogger<AccountRelationshipService>();

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
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(), 
                    sqliteMemoryWrapper.DbContext
                );

                AccountLink checkingAccountLink = accountService.GetAsLink(checkingAccountEntity.AccountId);
                AccountLink rentPrepaymentAccountLink = accountService.GetAsLink(rentPrepaymentAccountEntity.AccountId);

                var accountRelationshipService = new AccountRelationshipService(
                    loggerFactory.CreateLogger<AccountRelationshipService>(),
                    sqliteMemoryWrapper.DbContext
                );

                var newRelationship = new AccountRelationship
                {
                    SourceAccount = checkingAccountLink,
                    DestinationAccount = rentPrepaymentAccountLink,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                accountRelationshipService.Create(newRelationship);

                List<Entities.AccountRelationship> accountRelationshipEntities = 
                    sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();

                Assert.AreEqual(1, accountRelationshipEntities.Count);
                Assert.AreEqual(AccountRelationshipType.PhysicalToLogical, accountRelationshipEntities[0].Type);
                Assert.AreEqual(checkingAccountEntity.AccountId, accountRelationshipEntities[0].SourceAccount.AccountId);
                Assert.AreEqual(checkingAccountEntity.Name, accountRelationshipEntities[0].SourceAccount.Name);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, accountRelationshipEntities[0].DestinationAccount.AccountId);
                Assert.AreEqual(rentPrepaymentAccountEntity.Name, accountRelationshipEntities[0].DestinationAccount.Name);
            }
        }

        [TestMethod]
        public void TestDeleteAccountRelationship()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountRelationshipService> logger = loggerFactory.CreateLogger<AccountRelationshipService>();

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
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);

                var checkingToGroceriesPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountRelationshipService = new AccountRelationshipService(logger, sqliteMemoryWrapper.DbContext);

                accountRelationshipService.Delete(checkingToRentPrepaymentRelationship.AccountRelationshipId);

                List<Entities.AccountRelationship> accountRelationshipEntities =
                    sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();

                Assert.AreEqual(1, accountRelationshipEntities.Count);
                Assert.AreEqual(AccountRelationshipType.PhysicalToLogical, accountRelationshipEntities[0].Type);
                Assert.AreEqual(checkingAccountEntity.AccountId, accountRelationshipEntities[0].SourceAccount.AccountId);
                Assert.AreEqual(checkingAccountEntity.Name, accountRelationshipEntities[0].SourceAccount.Name);
                Assert.AreEqual(groceriesPrepaymentAccountEntity.AccountId, accountRelationshipEntities[0].DestinationAccount.AccountId);
                Assert.AreEqual(groceriesPrepaymentAccountEntity.Name, accountRelationshipEntities[0].DestinationAccount.Name);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestDeleteAccountRelationshipInvalidId()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountRelationshipService> logger = loggerFactory.CreateLogger<AccountRelationshipService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var accountRelationshipService = new AccountRelationshipService(logger, sqliteMemoryWrapper.DbContext);

                accountRelationshipService.Delete(666);
            }
        }

        [TestMethod]
        public void TestGetAllAccountRelationships()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountRelationshipService> logger = loggerFactory.CreateLogger<AccountRelationshipService>();

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
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);

                var checkingToGroceriesPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountRelationshipService = new AccountRelationshipService(logger, sqliteMemoryWrapper.DbContext);

                List<AccountRelationship> accountRelationships = accountRelationshipService.GetAll().ToList();

                Assert.AreEqual(2, accountRelationships.Count);
                Assert.AreEqual(AccountRelationshipType.PhysicalToLogical, accountRelationships[0].Type);
                Assert.AreEqual(checkingAccountEntity.AccountId, accountRelationships[0].SourceAccount.AccountId);
                Assert.AreEqual(checkingAccountEntity.Name, accountRelationships[0].SourceAccount.Name);
                Assert.AreEqual(groceriesPrepaymentAccountEntity.AccountId, accountRelationships[0].DestinationAccount.AccountId);
                Assert.AreEqual(groceriesPrepaymentAccountEntity.Name, accountRelationships[0].DestinationAccount.Name);
                Assert.AreEqual(AccountRelationshipType.PhysicalToLogical, accountRelationships[1].Type);
                Assert.AreEqual(checkingAccountEntity.AccountId, accountRelationships[1].SourceAccount.AccountId);
                Assert.AreEqual(checkingAccountEntity.Name, accountRelationships[1].SourceAccount.Name);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, accountRelationships[1].DestinationAccount.AccountId);
                Assert.AreEqual(rentPrepaymentAccountEntity.Name, accountRelationships[1].DestinationAccount.Name);
            }
        }

        [TestMethod]
        public void TestGetAccountRelationship()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountRelationshipService> logger = loggerFactory.CreateLogger<AccountRelationshipService>();

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
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);

                var checkingToGroceriesPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountRelationshipService = new AccountRelationshipService(logger, sqliteMemoryWrapper.DbContext);

                AccountRelationship accountRelationship = accountRelationshipService.Get(
                    checkingToRentPrepaymentRelationship.AccountRelationshipId);

                Assert.AreEqual(AccountRelationshipType.PhysicalToLogical, accountRelationship.Type);
                Assert.AreEqual(checkingAccountEntity.AccountId, accountRelationship.SourceAccount.AccountId);
                Assert.AreEqual(checkingAccountEntity.Name, accountRelationship.SourceAccount.Name);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, accountRelationship.DestinationAccount.AccountId);
                Assert.AreEqual(rentPrepaymentAccountEntity.Name, accountRelationship.DestinationAccount.Name);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetAccountRelationshipInvalidId()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountRelationshipService> logger = loggerFactory.CreateLogger<AccountRelationshipService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                 var accountRelationshipService = new AccountRelationshipService(logger, sqliteMemoryWrapper.DbContext);

                accountRelationshipService.Get(666);
            }
        }

        [TestMethod]
        public void TestUpdateAccountRelationship()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

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
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);

                var checkingToGroceriesPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext
                );

                AccountLink rentPrepaymentAccountLink = accountService.GetAsLink(rentPrepaymentAccountEntity.AccountId);
                AccountLink rentExpenseAccountLink = accountService.GetAsLink(rentExpenseAccountEntity.AccountId);

                var accountRelationshipService = new AccountRelationshipService(
                    loggerFactory.CreateLogger<AccountRelationshipService>(),
                    sqliteMemoryWrapper.DbContext
                );

                AccountRelationship relationship = accountRelationshipService.Get(
                    checkingToRentPrepaymentRelationship.AccountRelationshipId);
                relationship.Type = AccountRelationshipType.PrepaymentToExpense;
                relationship.SourceAccount = rentPrepaymentAccountLink;
                relationship.DestinationAccount = rentExpenseAccountLink;
                accountRelationshipService.Update(relationship);

                List<Entities.AccountRelationship> accountRelationshipEntities =
                    sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();

                Assert.AreEqual(2, accountRelationshipEntities.Count);
                Assert.AreEqual(AccountRelationshipType.PhysicalToLogical, accountRelationshipEntities[0].Type);
                Assert.AreEqual(checkingAccountEntity.AccountId, accountRelationshipEntities[0].SourceAccount.AccountId);
                Assert.AreEqual(checkingAccountEntity.Name, accountRelationshipEntities[0].SourceAccount.Name);
                Assert.AreEqual(groceriesPrepaymentAccountEntity.AccountId, accountRelationshipEntities[0].DestinationAccount.AccountId);
                Assert.AreEqual(groceriesPrepaymentAccountEntity.Name, accountRelationshipEntities[0].DestinationAccount.Name);
                Assert.AreEqual(AccountRelationshipType.PrepaymentToExpense, accountRelationshipEntities[1].Type);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, accountRelationshipEntities[1].SourceAccount.AccountId);
                Assert.AreEqual(rentPrepaymentAccountEntity.Name, accountRelationshipEntities[1].SourceAccount.Name);
                Assert.AreEqual(rentExpenseAccountEntity.AccountId, accountRelationshipEntities[1].DestinationAccount.AccountId);
                Assert.AreEqual(rentExpenseAccountEntity.Name, accountRelationshipEntities[1].DestinationAccount.Name);
            }
        }
    }
}
