using Financier.Services;
using Financier.UnitTesting;
using Financier.UnitTesting.DbSetup;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Core.Tests
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
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

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
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                var gbpCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Gbp, false);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, gbpCurrencyEntity);

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
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                var gbpCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Gbp, false);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, gbpCurrencyEntity);

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
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, false);
                var gbpCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Gbp, false);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, gbpCurrencyEntity);

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
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                var gbpCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Gbp, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, gbpCurrencyEntity);

                var currencyService = new CurrencyService(logger, sqliteMemoryWrapper.DbContext);
                Currency primaryCurrency = currencyService.GetPrimary();
            }
        }
    }
}
