using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Financier.Tests
{
    [TestClass]
    public class FixerIOCurrencyExchangeServiceTests
    {
        [TestMethod]
        [Ignore]// This hits a live API, so shouldn't be aprt of the normal suite
        public void TestGetUSDGBPOnNewYearsDay2018()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            var service = new FixerIOCurrencyExchangeService(
                loggerFactory.CreateLogger<FixerIOCurrencyExchangeService>()
            );

            decimal gbpUsdRate = service.GetExchangeRate("GBP", "USD", new DateTime(2018, 1, 1));
            // => https://api.fixer.io/latest?base=GBP&date=2018-01-01

            Assert.AreEqual(1.3517m, gbpUsdRate);
        }
    }
}
