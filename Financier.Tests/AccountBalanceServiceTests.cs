using Financier.Data;
using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class AccountBalanceServiceTests
    {
        [TestMethod]
        public void TestGetBalanceNoTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountBalanceService> logger = loggerFactory.CreateLogger<AccountBalanceService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrency = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$"
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingAccount = new Account
                {
                    Name = "Checking",
                    Currency = usdCurrency
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountBalanceService = new AccountBalanceService(logger, sqliteMemoryWrapper.DbContext);

                decimal checkingAccountBalance = accountBalanceService.GetBalance(checkingAccount.AccountId);

                Assert.AreEqual(0, checkingAccountBalance);
            }
        }

        [TestMethod]
        public void TestGetBalanceUnlinked()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountBalanceService> logger = loggerFactory.CreateLogger<AccountBalanceService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrency = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$"
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var incomeAccount = new Account
                {
                    Name = "Income",
                    Currency = usdCurrency
                };
                var checkingAccount = new Account
                {
                    Name = "Checking",
                    Currency = usdCurrency
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
                        CreditAmount = 100m,
                        DebitAmount = 100m
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactions);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountBalanceService = new AccountBalanceService(logger, sqliteMemoryWrapper.DbContext);

                decimal checkingAccountBalance = accountBalanceService.GetBalance(checkingAccount.AccountId);

                Assert.AreEqual(-100m, checkingAccountBalance);
            }
        }

        [TestMethod]
        public void TestGetBalanceWithLogical()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountBalanceService> logger = loggerFactory.CreateLogger<AccountBalanceService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrency = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$"
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var incomeAccount = new Account
                {
                    Name = "Income",
                    Currency = usdCurrency
                };
                var checkingAccount = new Account
                {
                    Name = "Checking",
                    Currency = usdCurrency
                };
                var rentPrepaymentAccount = new Account
                {
                    Name = "Rent Prepayment",
                    Currency = usdCurrency
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
                        CreditAmount = 100m,
                        DebitAmount = 100m
                    },
                    new Transaction
                    {
                        CreditAccount = checkingAccount,
                        DebitAccount = rentPrepaymentAccount,
                        CreditAmount = 60m,
                        DebitAmount = 60m
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

                var accountBalanceService = new AccountBalanceService(logger, sqliteMemoryWrapper.DbContext);

                decimal checkingAccountBalance = accountBalanceService.GetBalance(checkingAccount.AccountId);

                Assert.AreEqual(-100m, checkingAccountBalance);
            }
        }
    }
}