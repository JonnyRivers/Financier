using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Financier.Tests
{
    [TestClass]
    public class AccountServiceTests
    {
        [TestMethod]
        public void TestGetBalanceNoTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrency = new Entities.Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$",
                    IsPrimary = true
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingAccountEntity = new Entities.Account
                {
                    Name = "Checking",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Asset
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccountEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                Account checkingAccount = accountService.Get(checkingAccountEntity.AccountId);

                Assert.AreEqual(0, checkingAccount.Balance);
            }
        }

        [TestMethod]
        public void TestGetBalanceUnlinked()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

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

                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccountEntity);
                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccountEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

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

                Assert.AreEqual(100m, checkingAccount.Balance);
            }
        }

        [TestMethod]
        public void TestGetBalanceWithLogical()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

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

                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccountEntity);
                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccountEntity);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentPrepaymentAccountEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

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

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                Account checkingAccount = accountService.Get(checkingAccountEntity.AccountId);

                Assert.AreEqual(100m, checkingAccount.Balance);
            }
        }
    }
}