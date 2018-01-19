using Financier.Entities;
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
                
                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var currencyService = new CurrencyService(
                    loggerFactory.CreateLogger<CurrencyService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var balanceSheetService = new BalanceSheetService(
                    loggerFactory.CreateLogger<BalanceSheetService>(), 
                    accountService, 
                    currencyService
                );

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

                var checkingAccount = new Entities.Account
                {
                    Name = "Checking",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Asset
                };
                var incomeAccount = new Entities.Account
                {
                    Name = "Income",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Income
                };
                var capitalAccount = new Entities.Account
                {
                    Name = "Capital",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Capital
                };
                var rentAccount = new Entities.Account
                {
                    Name = "Rent",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Expense
                };
                var creditCardAccount = new Entities.Account
                {
                    Name = "Credit Card",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Liability
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(capitalAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(creditCardAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var currencyService = new CurrencyService(
                    loggerFactory.CreateLogger<CurrencyService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var balanceSheetService = new BalanceSheetService(
                    loggerFactory.CreateLogger<BalanceSheetService>(),
                    accountService,
                    currencyService
                );

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

                var checkingAccount = new Entities.Account
                {
                    Name = "Checking",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Asset
                };
                var incomeAccount = new Entities.Account
                {
                    Name = "Income",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Income
                };
                var capitalAccount = new Entities.Account
                {
                    Name = "Capital",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Capital
                };
                var rentAccount = new Entities.Account
                {
                    Name = "Rent",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Expense
                };
                var creditCardAccount = new Entities.Account
                {
                    Name = "Credit Card",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Liability
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(capitalAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(creditCardAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactions = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = capitalAccount,
                        DebitAccount = checkingAccount,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1)
                    },// capital=100CR,checking=100DR
                    new Entities.Transaction
                    {
                        CreditAccount = creditCardAccount,
                        DebitAccount = rentAccount,
                        Amount = 40m,
                        At = new DateTime(2018, 1, 2)
                    },// capital=100CR,checking=100DR,credit-card=40CR,rent=40DR
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccount,
                        DebitAccount = creditCardAccount,
                        Amount = 40m,
                        At = new DateTime(2018, 1, 3)
                    },// capital=100CR,checking=60DR,rent=40DR
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccount,
                        DebitAccount = checkingAccount,
                        Amount = 200m,
                        At = new DateTime(2018, 1, 4)
                    }// capital=100CR,checking=260DR,rent=40DR,income=200CR
                };
                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactions);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var currencyService = new CurrencyService(
                    loggerFactory.CreateLogger<CurrencyService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var balanceSheetService = new BalanceSheetService(
                    loggerFactory.CreateLogger<BalanceSheetService>(),
                    accountService,
                    currencyService
                );

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
                Assert.AreEqual(-40, balanceSheetTwoTransactions.TotalLiabilities);
                Assert.AreEqual(60, balanceSheetTwoTransactions.NetWorth);
                Assert.AreEqual(1, balanceSheetTwoTransactionsAssets.Count);
                Assert.AreEqual(1, balanceSheetTwoTransactionsLiabilities.Count);
                Assert.AreEqual(checkingAccount.Name, balanceSheetTwoTransactionsAssets[0].Name);
                Assert.AreEqual(100, balanceSheetTwoTransactionsAssets[0].Balance);
                Assert.AreEqual(creditCardAccount.Name, balanceSheetTwoTransactionsLiabilities[0].Name);
                Assert.AreEqual(-40, balanceSheetTwoTransactionsLiabilities[0].Balance);

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

        [TestMethod]
        public void TestBalanceSheetWithLogicalAccounts()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

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

                var checkingAccount = new Entities.Account
                {
                    Name = "Checking",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Asset
                };
                var incomeAccount = new Entities.Account
                {
                    Name = "Income",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Income
                };
                var rentPrepaymentAccount = new Entities.Account
                {
                    Name = "Rent Prepayment",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Asset
                };
                var groceriesPrepaymentAccount = new Entities.Account
                {
                    Name = "Groceries Prepayment",
                    Currency = usdCurrency,
                    Type = Entities.AccountType.Asset
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentPrepaymentAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(groceriesPrepaymentAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingToRentPrepaymentRelationship = new AccountRelationship
                {
                    SourceAccount = checkingAccount,
                    DestinationAccount = rentPrepaymentAccount,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToGroceriesPrepaymentRelationship = new AccountRelationship
                {
                    SourceAccount = checkingAccount,
                    DestinationAccount = groceriesPrepaymentAccount,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactions = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccount,
                        DebitAccount = checkingAccount,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1)
                    },// income=100CR,checking=100DR
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccount,
                        DebitAccount = rentPrepaymentAccount,
                        Amount = 40m,
                        At = new DateTime(2018, 1, 1)
                    },// income=100CR,(checking=60DR,rent-prepayment=40DR)=100DR
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccount,
                        DebitAccount = groceriesPrepaymentAccount,
                        Amount = 20m,
                        At = new DateTime(2018, 1, 1)
                    }// income=100CR,(checking=40DR,rent-prepayment=40DR,groceries-prepayment=20DR)=100DR
                };
                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactions);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var currencyService = new CurrencyService(
                    loggerFactory.CreateLogger<CurrencyService>(),
                    sqliteMemoryWrapper.DbContext
                );
                var balanceSheetService = new BalanceSheetService(
                    loggerFactory.CreateLogger<BalanceSheetService>(),
                    accountService,
                    currencyService
                );

                BalanceSheet balanceSheet = balanceSheetService.Generate(new DateTime(2018, 1, 1));
                List<BalanceSheetItem> balanceSheetAssets = balanceSheet.Assets.ToList();
                List<BalanceSheetItem> balanceSheetLiabilities = balanceSheet.Liabilities.ToList();

                Assert.AreEqual(usdCurrency.Symbol, balanceSheet.CurrencySymbol);
                Assert.AreEqual(100, balanceSheet.TotalAssets);
                Assert.AreEqual(0, balanceSheet.TotalLiabilities);
                Assert.AreEqual(100, balanceSheet.NetWorth);
                Assert.AreEqual(1, balanceSheetAssets.Count);
                Assert.AreEqual(0, balanceSheetLiabilities.Count);
                Assert.AreEqual(checkingAccount.Name, balanceSheetAssets[0].Name);
                Assert.AreEqual(100, balanceSheetAssets[0].Balance);
            }
        }
    }
}
