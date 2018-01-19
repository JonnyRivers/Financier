using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class CurrencyServiceTests
    {
        [TestMethod]
        public void TestGetAllNoCurrencies()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<CurrencyService> logger = loggerFactory.CreateLogger<CurrencyService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyService = new CurrencyService(logger, sqliteMemoryWrapper.DbContext);
                IEnumerable<Currency> currencies = currencyService.GetAll();

                Assert.AreEqual(0, currencies.Count());
            }
        }

        [TestMethod]
        public void TestGetAllOneCurrencies()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<CurrencyService> logger = loggerFactory.CreateLogger<CurrencyService>();

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

                var currencyService = new CurrencyService(logger, sqliteMemoryWrapper.DbContext);
                List<Currency> currencies = currencyService.GetAll().ToList();

                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(usdCurrencyEntity.Name, currencies[0].Name);
                Assert.AreEqual(usdCurrencyEntity.ShortName, currencies[0].ShortName);
                Assert.AreEqual(usdCurrencyEntity.Symbol, currencies[0].Symbol);
                Assert.AreEqual(usdCurrencyEntity.IsPrimary, currencies[0].IsPrimary);
            }
        }

        [TestMethod]
        public void TestGetAllManyCurrencies()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<CurrencyService> logger = loggerFactory.CreateLogger<CurrencyService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrencyEntity = new Entities.Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$",
                    IsPrimary = true
                };
                var gbpCurrencyEntity = new Entities.Currency
                {
                    Name = "UK Sterling",
                    ShortName = "GBP",
                    Symbol = "£",
                    IsPrimary = false
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrencyEntity);
                sqliteMemoryWrapper.DbContext.Currencies.Add(gbpCurrencyEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var currencyService = new CurrencyService(logger, sqliteMemoryWrapper.DbContext);
                List<Currency> currencies = currencyService.GetAll().ToList();

                Assert.AreEqual(2, currencies.Count);
                Assert.AreEqual(usdCurrencyEntity.Name, currencies[0].Name);
                Assert.AreEqual(usdCurrencyEntity.ShortName, currencies[0].ShortName);
                Assert.AreEqual(usdCurrencyEntity.Symbol, currencies[0].Symbol);
                Assert.AreEqual(usdCurrencyEntity.IsPrimary, currencies[0].IsPrimary);
                Assert.AreEqual(gbpCurrencyEntity.Name, currencies[1].Name);
                Assert.AreEqual(gbpCurrencyEntity.ShortName, currencies[1].ShortName);
                Assert.AreEqual(gbpCurrencyEntity.Symbol, currencies[1].Symbol);
                Assert.AreEqual(gbpCurrencyEntity.IsPrimary, currencies[1].IsPrimary);
            }
        }

        [TestMethod]
        public void TestGetPrimaryValid()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<CurrencyService> logger = loggerFactory.CreateLogger<CurrencyService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrencyEntity = new Entities.Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$",
                    IsPrimary = true
                };
                var gbpCurrencyEntity = new Entities.Currency
                {
                    Name = "UK Sterling",
                    ShortName = "GBP",
                    Symbol = "£",
                    IsPrimary = false
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrencyEntity);
                sqliteMemoryWrapper.DbContext.Currencies.Add(gbpCurrencyEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var currencyService = new CurrencyService(logger, sqliteMemoryWrapper.DbContext);
                Currency primaryCurrency = currencyService.GetPrimary();

                Assert.AreEqual(usdCurrencyEntity.Name, primaryCurrency.Name);
                Assert.AreEqual(usdCurrencyEntity.ShortName, primaryCurrency.ShortName);
                Assert.AreEqual(usdCurrencyEntity.Symbol, primaryCurrency.Symbol);
                Assert.AreEqual(true, primaryCurrency.IsPrimary);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetPrimaryFailsWithNoPrimary()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<CurrencyService> logger = loggerFactory.CreateLogger<CurrencyService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrencyEntity = new Entities.Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$",
                    IsPrimary = false
                };
                var gbpCurrencyEntity = new Entities.Currency
                {
                    Name = "UK Sterling",
                    ShortName = "GBP",
                    Symbol = "£",
                    IsPrimary = false
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrencyEntity);
                sqliteMemoryWrapper.DbContext.Currencies.Add(gbpCurrencyEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var currencyService = new CurrencyService(logger, sqliteMemoryWrapper.DbContext);
                Currency primaryCurrency = currencyService.GetPrimary();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetPrimaryFailsWithMultiplePrimaries()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<CurrencyService> logger = loggerFactory.CreateLogger<CurrencyService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrencyEntity = new Entities.Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$",
                    IsPrimary = true
                };
                var gbpCurrencyEntity = new Entities.Currency
                {
                    Name = "UK Sterling",
                    ShortName = "GBP",
                    Symbol = "£",
                    IsPrimary = true
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrencyEntity);
                sqliteMemoryWrapper.DbContext.Currencies.Add(gbpCurrencyEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var currencyService = new CurrencyService(logger, sqliteMemoryWrapper.DbContext);
                Currency primaryCurrency = currencyService.GetPrimary();
            }
        }
    }
}
