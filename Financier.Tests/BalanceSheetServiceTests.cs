﻿using Financier.Data;
using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class BalanceSheetServiceTests
    {
        [TestMethod]
        public void TestBalanceSheetNoAccounts()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BalanceSheetService> logger = loggerFactory.CreateLogger<BalanceSheetService>();

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

                var balanceSheetService = new BalanceSheetService(logger, sqliteMemoryWrapper.DbContext);

                BalanceSheet balanceSheet = balanceSheetService.Generate(new DateTime(2018,1, 1));

                Assert.AreEqual(usdCurrency.Symbol, balanceSheet.CurrencySymbol);
                Assert.AreEqual(0, balanceSheet.TotalAssets);
                Assert.AreEqual(0, balanceSheet.TotalLiabilities);
                Assert.AreEqual(0, balanceSheet.Assets.Count());
                Assert.AreEqual(0, balanceSheet.Liabilities.Count());
            }
        }

        [TestMethod]
        public void TestBalanceSheetNoTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BalanceSheetService> logger = loggerFactory.CreateLogger<BalanceSheetService>();

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
                var incomeAccount = new Account
                {
                    Name = "Income",
                    Currency = usdCurrency,
                    Type = AccountType.Income
                };
                var capitalAccount = new Account
                {
                    Name = "Capital",
                    Currency = usdCurrency,
                    Type = AccountType.Capital
                };
                var rentAccount = new Account
                {
                    Name = "Rent",
                    Currency = usdCurrency,
                    Type = AccountType.Expense
                };
                var creditCardAccount = new Account
                {
                    Name = "Credit Card",
                    Currency = usdCurrency,
                    Type = AccountType.Liability
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(capitalAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(creditCardAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var balanceSheetService = new BalanceSheetService(logger, sqliteMemoryWrapper.DbContext);

                BalanceSheet balanceSheet = balanceSheetService.Generate(new DateTime(2018, 1, 1));
                List<BalanceSheetItem> balanceSheetAssets = balanceSheet.Assets.ToList();
                List<BalanceSheetItem> balanceSheetLiabilities = balanceSheet.Liabilities.ToList();

                Assert.AreEqual(usdCurrency.Symbol, balanceSheet.CurrencySymbol);
                Assert.AreEqual(0, balanceSheet.TotalAssets);
                Assert.AreEqual(0, balanceSheet.TotalLiabilities);
                Assert.AreEqual(1, balanceSheetAssets.Count);
                Assert.AreEqual(1, balanceSheetLiabilities.Count);
                Assert.AreEqual(checkingAccount.Name, balanceSheetAssets[0].Name);
                Assert.AreEqual(0, balanceSheetAssets[0].Balance);
                Assert.AreEqual(creditCardAccount.Name, balanceSheetLiabilities[0].Name);
                Assert.AreEqual(0, balanceSheetLiabilities[0].Balance);
            }
        }

        [TestMethod]
        public void TestBalanceSheetWithTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BalanceSheetService> logger = loggerFactory.CreateLogger<BalanceSheetService>();

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
                var incomeAccount = new Account
                {
                    Name = "Income",
                    Currency = usdCurrency,
                    Type = AccountType.Income
                };
                var capitalAccount = new Account
                {
                    Name = "Capital",
                    Currency = usdCurrency,
                    Type = AccountType.Capital
                };
                var rentAccount = new Account
                {
                    Name = "Rent",
                    Currency = usdCurrency,
                    Type = AccountType.Expense
                };
                var creditCardAccount = new Account
                {
                    Name = "Credit Card",
                    Currency = usdCurrency,
                    Type = AccountType.Liability
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(capitalAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(creditCardAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactions = new Transaction[]
                {
                    new Transaction
                    {
                        CreditAccount = capitalAccount,
                        DebitAccount = checkingAccount,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1)
                    },// capital=100CR,checking=100DR
                    new Transaction
                    {
                        CreditAccount = creditCardAccount,
                        DebitAccount = rentAccount,
                        Amount = 40m,
                        At = new DateTime(2018, 1, 2)
                    },// capital=100CR,checking=100DR,credit-card=40CR,rent=40DR
                    new Transaction
                    {
                        CreditAccount = checkingAccount,
                        DebitAccount = creditCardAccount,
                        Amount = 40m,
                        At = new DateTime(2018, 1, 3)
                    },// capital=100CR,checking=60DR,rent=40DR
                    new Transaction
                    {
                        CreditAccount = incomeAccount,
                        DebitAccount = checkingAccount,
                        Amount = 200m,
                        At = new DateTime(2018, 1, 4)
                    }// capital=100CR,checking=260DR,rent=40DR,income=200CR
                };
                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactions);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var balanceSheetService = new BalanceSheetService(logger, sqliteMemoryWrapper.DbContext);

                BalanceSheet balanceSheetBeforeTransactions = balanceSheetService.Generate(new DateTime(2017, 1, 1));
                List<BalanceSheetItem> balanceSheetBeforeTransactionsAssets = balanceSheetBeforeTransactions.Assets.ToList();
                List<BalanceSheetItem> balanceSheetBeforeTransactionsLiabilities = balanceSheetBeforeTransactions.Liabilities.ToList();

                Assert.AreEqual(usdCurrency.Symbol, balanceSheetBeforeTransactions.CurrencySymbol);
                Assert.AreEqual(0, balanceSheetBeforeTransactions.TotalAssets);
                Assert.AreEqual(0, balanceSheetBeforeTransactions.TotalLiabilities);
                Assert.AreEqual(0, balanceSheetBeforeTransactions.NetWorth);
                Assert.AreEqual(1, balanceSheetBeforeTransactionsAssets.Count);
                Assert.AreEqual(1, balanceSheetBeforeTransactionsLiabilities.Count);
                Assert.AreEqual(checkingAccount.Name, balanceSheetBeforeTransactionsAssets[0].Name);
                Assert.AreEqual(0, balanceSheetBeforeTransactionsAssets[0].Balance);
                Assert.AreEqual(creditCardAccount.Name, balanceSheetBeforeTransactionsLiabilities[0].Name);
                Assert.AreEqual(0, balanceSheetBeforeTransactionsLiabilities[0].Balance);

                BalanceSheet balanceSheetTwoTransactions = balanceSheetService.Generate(new DateTime(2018, 1, 2));
                List<BalanceSheetItem> balanceSheetTwoTransactionsAssets = balanceSheetTwoTransactions.Assets.ToList();
                List<BalanceSheetItem> balanceSheetTwoTransactionsLiabilities = balanceSheetTwoTransactions.Liabilities.ToList();

                Assert.AreEqual(usdCurrency.Symbol, balanceSheetTwoTransactions.CurrencySymbol);
                Assert.AreEqual(100, balanceSheetTwoTransactions.TotalAssets);
                Assert.AreEqual(40, balanceSheetTwoTransactions.TotalLiabilities);
                Assert.AreEqual(60, balanceSheetTwoTransactions.NetWorth);
                Assert.AreEqual(1, balanceSheetTwoTransactionsAssets.Count);
                Assert.AreEqual(1, balanceSheetTwoTransactionsLiabilities.Count);
                Assert.AreEqual(checkingAccount.Name, balanceSheetTwoTransactionsAssets[0].Name);
                Assert.AreEqual(100, balanceSheetTwoTransactionsAssets[0].Balance);
                Assert.AreEqual(creditCardAccount.Name, balanceSheetTwoTransactionsLiabilities[0].Name);
                Assert.AreEqual(40, balanceSheetTwoTransactionsLiabilities[0].Balance);

                BalanceSheet balanceSheetAtEnd = balanceSheetService.Generate(new DateTime(2018, 2, 1));
                List<BalanceSheetItem> balanceSheetAtEndAssets = balanceSheetAtEnd.Assets.ToList();
                List<BalanceSheetItem> balanceSheetAtEndLiabilities = balanceSheetAtEnd.Liabilities.ToList();

                Assert.AreEqual(usdCurrency.Symbol, balanceSheetAtEnd.CurrencySymbol);
                Assert.AreEqual(260, balanceSheetAtEnd.TotalAssets);
                Assert.AreEqual(0, balanceSheetAtEnd.TotalLiabilities);
                Assert.AreEqual(260, balanceSheetAtEnd.NetWorth);
                Assert.AreEqual(1, balanceSheetAtEndAssets.Count);
                Assert.AreEqual(1, balanceSheetAtEndLiabilities.Count);
                Assert.AreEqual(checkingAccount.Name, balanceSheetAtEndAssets[0].Name);
                Assert.AreEqual(260, balanceSheetAtEndAssets[0].Balance);
                Assert.AreEqual(creditCardAccount.Name, balanceSheetAtEndLiabilities[0].Name);
                Assert.AreEqual(0, balanceSheetAtEndLiabilities[0].Balance);
            }
        }
    }
}
