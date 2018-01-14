using Financier.Data;
using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

                BalanceSheet balanceSheet = balanceSheetService.Generate();

                Assert.AreEqual(usdCurrency.Symbol, balanceSheet.CurrencySymbol);
                Assert.AreEqual(0, balanceSheet.TotalAssets);
                Assert.AreEqual(0, balanceSheet.TotalLiabilitiesAndEquity);
                Assert.AreEqual(0, balanceSheet.Assets.Count());
                Assert.AreEqual(0, balanceSheet.Liabilities.Count());
                Assert.AreEqual(0, balanceSheet.Equities.Count());
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

                BalanceSheet balanceSheet = balanceSheetService.Generate();
                List<BalanceSheetItem> balanceSheetAssets = balanceSheet.Assets.ToList();
                List<BalanceSheetItem> balanceSheetLiabilities = balanceSheet.Liabilities.ToList();
                List<BalanceSheetItem> balanceSheetEquities = balanceSheet.Equities.ToList();

                Assert.AreEqual(usdCurrency.Symbol, balanceSheet.CurrencySymbol);
                Assert.AreEqual(0, balanceSheet.TotalAssets);
                Assert.AreEqual(0, balanceSheet.TotalLiabilitiesAndEquity);
                Assert.AreEqual(1, balanceSheetAssets.Count);
                Assert.AreEqual(1, balanceSheetLiabilities.Count);
                Assert.AreEqual(1, balanceSheetEquities.Count);
                Assert.AreEqual(checkingAccount.Name, balanceSheetAssets[0].Name);
                Assert.AreEqual(0, balanceSheetAssets[0].Balance);
                Assert.AreEqual(creditCardAccount.Name, balanceSheetLiabilities[0].Name);
                Assert.AreEqual(0, balanceSheetLiabilities[0].Balance);
                Assert.AreEqual(capitalAccount.Name, balanceSheetEquities[0].Name);
                Assert.AreEqual(0, balanceSheetEquities[0].Balance);
            }
        }
    }
}
