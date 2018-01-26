using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class AccountServiceTests
    {
        [TestMethod]
        public void TestGetAllAccounts()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                List<Account> accounts = accountService.GetAll().ToList();

                Assert.AreEqual(2, accounts.Count);
                Assert.AreEqual(incomeAccountEntity.AccountId, accounts[0].AccountId);
                Assert.AreEqual(incomeAccountEntity.Name, accounts[0].Name);
                Assert.AreEqual(AccountType.Income, accounts[0].Type);
                Assert.AreEqual(usdCurrencyEntity.CurrencyId, accounts[0].Currency.CurrencyId);
                Assert.AreEqual(usdCurrencyEntity.IsPrimary, accounts[0].Currency.IsPrimary);
                Assert.AreEqual(usdCurrencyEntity.Name, accounts[0].Currency.Name);
                Assert.AreEqual(usdCurrencyEntity.ShortName, accounts[0].Currency.ShortName);
                Assert.AreEqual(usdCurrencyEntity.Symbol, accounts[0].Currency.Symbol);
                Assert.AreEqual(0, accounts[0].LogicalAccounts.Count());
                Assert.AreEqual(0, accounts[0].Balance);
                Assert.AreEqual(checkingAccountEntity.AccountId, accounts[1].AccountId);
                Assert.AreEqual(checkingAccountEntity.Name, accounts[1].Name);
                Assert.AreEqual(AccountType.Asset, accounts[1].Type);
                Assert.AreEqual(usdCurrencyEntity.CurrencyId, accounts[1].Currency.CurrencyId);
                Assert.AreEqual(usdCurrencyEntity.IsPrimary, accounts[1].Currency.IsPrimary);
                Assert.AreEqual(usdCurrencyEntity.Name, accounts[1].Currency.Name);
                Assert.AreEqual(usdCurrencyEntity.ShortName, accounts[1].Currency.ShortName);
                Assert.AreEqual(usdCurrencyEntity.Symbol, accounts[1].Currency.Symbol);
                Assert.AreEqual(0, accounts[1].LogicalAccounts.Count());
                Assert.AreEqual(0, accounts[1].Balance);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetAccountFailInvalidId()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                Account account = accountService.Get(666);
            }
        }

        [TestMethod]
        public void TestGetAllAccountLinks()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

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
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);

                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = Entities.AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                List<AccountLink> accountLinks = accountService.GetAllAsLinks().ToList();

                Assert.AreEqual(3, accountLinks.Count);
                Assert.AreEqual(incomeAccountEntity.AccountId, accountLinks[0].AccountId);
                Assert.AreEqual(false, accountLinks[0].HasLogicalAccounts);
                Assert.AreEqual(incomeAccountEntity.Name, accountLinks[0].Name);
                Assert.AreEqual(AccountType.Income, accountLinks[0].Type);
                Assert.AreEqual(checkingAccountEntity.AccountId, accountLinks[1].AccountId);
                Assert.AreEqual(true, accountLinks[1].HasLogicalAccounts);
                Assert.AreEqual(checkingAccountEntity.Name, accountLinks[1].Name);
                Assert.AreEqual(AccountType.Asset, accountLinks[1].Type);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, accountLinks[2].AccountId);
                Assert.AreEqual(false, accountLinks[2].HasLogicalAccounts);
                Assert.AreEqual(rentPrepaymentAccountEntity.Name, accountLinks[2].Name);
                Assert.AreEqual(AccountType.Asset, accountLinks[2].Type);
            }
        }

        [TestMethod]
        public void TestGetAccountNoTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account checkingAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                Account checkingAccount = accountService.Get(checkingAccountEntity.AccountId);

                Assert.AreEqual(checkingAccountEntity.AccountId, checkingAccount.AccountId);
                Assert.AreEqual(checkingAccountEntity.Name, checkingAccount.Name);
                Assert.AreEqual(AccountType.Asset, checkingAccount.Type);
                Assert.AreEqual(usdCurrencyEntity.CurrencyId, checkingAccount.Currency.CurrencyId);
                Assert.AreEqual(usdCurrencyEntity.IsPrimary, checkingAccount.Currency.IsPrimary);
                Assert.AreEqual(usdCurrencyEntity.Name, checkingAccount.Currency.Name);
                Assert.AreEqual(usdCurrencyEntity.ShortName, checkingAccount.Currency.ShortName);
                Assert.AreEqual(usdCurrencyEntity.Symbol, checkingAccount.Currency.Symbol);
                Assert.AreEqual(0, checkingAccount.LogicalAccounts.Count());
                Assert.AreEqual(0, checkingAccount.Balance);
            }
        }

        [TestMethod]
        public void TestGetAccountWithTransacions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var transactions = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 1)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactions);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                Account checkingAccount = accountService.Get(checkingAccountEntity.AccountId);
                Account incomeAccount = accountService.Get(incomeAccountEntity.AccountId);

                Assert.AreEqual(0, checkingAccount.LogicalAccounts.Count());
                Assert.AreEqual(100m, checkingAccount.Balance);
                Assert.AreEqual(0, incomeAccount.LogicalAccounts.Count());
                Assert.AreEqual(-100m, incomeAccount.Balance);
            }
        }

        [TestMethod]
        public void TestGetAccountWithLogicalAccounts()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

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

                var transactions = new Entities.Transaction[]
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

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactions);
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

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                Account checkingAccount = accountService.Get(checkingAccountEntity.AccountId);
                Account incomeAccount = accountService.Get(incomeAccountEntity.AccountId);
                Account rentPrepaymentAccount = accountService.Get(rentPrepaymentAccountEntity.AccountId);
                Account groceriesPrepaymentAccount = accountService.Get(groceriesPrepaymentAccountEntity.AccountId);

                Assert.AreEqual(2, checkingAccount.LogicalAccounts.Count());
                Assert.AreEqual(100m, checkingAccount.Balance);
                Assert.AreEqual(0, incomeAccount.LogicalAccounts.Count());
                Assert.AreEqual(-100m, incomeAccount.Balance);
                Assert.AreEqual(0, rentPrepaymentAccount.LogicalAccounts.Count());
                Assert.AreEqual(60m, rentPrepaymentAccount.Balance);
                Assert.AreEqual(0, groceriesPrepaymentAccount.LogicalAccounts.Count());
                Assert.AreEqual(10m, groceriesPrepaymentAccount.Balance);
            }
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                var account = new Account
                {
                    Name = "Checking",
                    Type = AccountType.Asset,
                    Currency = new Currency
                    {
                        CurrencyId = usdCurrencyEntity.CurrencyId,
                        Name = usdCurrencyEntity.Name,
                        ShortName = usdCurrencyEntity.ShortName,
                        Symbol = usdCurrencyEntity.Symbol,
                        IsPrimary = usdCurrencyEntity.IsPrimary
                    },
                    LogicalAccounts = new Account[0],
                    Balance = 0m
                };
                accountService.Create(account);

                List<Entities.Account> accountEntities = sqliteMemoryWrapper.DbContext.Accounts.ToList();

                Assert.AreEqual(1, accountEntities.Count);
                Assert.AreEqual(account.Name, accountEntities[0].Name);
                Assert.AreEqual(Entities.AccountType.Asset, accountEntities[0].Type);
                Assert.AreEqual(usdCurrencyEntity.CurrencyId, accountEntities[0].Currency.CurrencyId);
                Assert.AreEqual(usdCurrencyEntity.IsPrimary, accountEntities[0].Currency.IsPrimary);
                Assert.AreEqual(usdCurrencyEntity.Name, accountEntities[0].Currency.Name);
                Assert.AreEqual(usdCurrencyEntity.ShortName, accountEntities[0].Currency.ShortName);
                Assert.AreEqual(usdCurrencyEntity.Symbol, accountEntities[0].Currency.Symbol);
            }
        }

        [TestMethod]
        public void TestUpdateAccount()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                var gbpCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Gbp, false);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, gbpCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);
                Account checkingAccount = accountService.Get(checkingAccountEntity.AccountId);
                checkingAccount.Name = "Updated";
                checkingAccount.Type = AccountType.Expense;
                checkingAccount.Currency.CurrencyId = gbpCurrencyEntity.CurrencyId;
                accountService.Update(checkingAccount);

                List<Entities.Account> accountEntities = sqliteMemoryWrapper.DbContext.Accounts.ToList();

                Assert.AreEqual(2, accountEntities.Count);
                Assert.AreEqual(checkingAccount.Name, accountEntities[1].Name);
                Assert.AreEqual(Entities.AccountType.Expense, accountEntities[1].Type);
                Assert.AreEqual(gbpCurrencyEntity.CurrencyId, accountEntities[1].Currency.CurrencyId);
                Assert.AreEqual(gbpCurrencyEntity.IsPrimary, accountEntities[1].Currency.IsPrimary);
                Assert.AreEqual(gbpCurrencyEntity.Name, accountEntities[1].Currency.Name);
                Assert.AreEqual(gbpCurrencyEntity.ShortName, accountEntities[1].Currency.ShortName);
                Assert.AreEqual(gbpCurrencyEntity.Symbol, accountEntities[1].Currency.Symbol);
            }
        }
    }
}