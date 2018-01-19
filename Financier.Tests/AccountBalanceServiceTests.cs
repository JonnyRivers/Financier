using Financier.Entities;
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
                var usdCurrency = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$",
                    IsPrimary = true
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingAccount = new Account
                {
                    Name = "Checking",
                    Currency = usdCurrency,
                    Type = AccountType.Asset
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                decimal checkingAccountBalance = accountService.GetBalance(checkingAccount.AccountId);

                Assert.AreEqual(0, checkingAccountBalance);
            }
        }

        [TestMethod]
        public void TestGetBalanceUnlinked()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrency = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$",
                    IsPrimary = true
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var incomeAccount = new Account
                {
                    Name = "Income",
                    Currency = usdCurrency,
                    Type = AccountType.Income
                };
                var checkingAccount = new Account
                {
                    Name = "Checking",
                    Currency = usdCurrency,
                    Type = AccountType.Asset
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactions = new Transaction[]
                {
                    new Transaction
                    {
                        CreditAccount = incomeAccount,
                        DebitAccount = checkingAccount,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 1)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactions);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                decimal checkingAccountBalance = accountService.GetBalance(checkingAccount.AccountId);

                Assert.AreEqual(100m, checkingAccountBalance);
            }
        }

        [TestMethod]
        public void TestGetBalanceWithLogical()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountService> logger = loggerFactory.CreateLogger<AccountService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrency = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$",
                    IsPrimary = true
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var incomeAccount = new Account
                {
                    Name = "Income",
                    Currency = usdCurrency,
                    Type = AccountType.Income
                };
                var checkingAccount = new Account
                {
                    Name = "Checking",
                    Currency = usdCurrency,
                    Type = AccountType.Asset
                };
                var rentPrepaymentAccount = new Account
                {
                    Name = "Rent Prepayment",
                    Currency = usdCurrency,
                    Type = AccountType.Asset
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentPrepaymentAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactions = new Transaction[]
                {
                    new Transaction
                    {
                        CreditAccount = incomeAccount,
                        DebitAccount = checkingAccount,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 0)
                    },
                    new Transaction
                    {
                        CreditAccount = checkingAccount,
                        DebitAccount = rentPrepaymentAccount,
                        Amount = 60m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactions);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingToRentPrepaymentRelationship = new AccountRelationship
                {
                    SourceAccount = checkingAccount,
                    DestinationAccount = rentPrepaymentAccount,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(logger, sqliteMemoryWrapper.DbContext);

                decimal checkingAccountBalance = accountService.GetBalance(checkingAccount.AccountId);

                Assert.AreEqual(100m, checkingAccountBalance);
            }
        }
    }
}