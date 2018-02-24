using Financier.Services;
using Financier.Tests.Concrete;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Financier.Tests
{
    [TestClass]
    public class FixerIOCurrencyExchangeServiceTests
    {
        [TestMethod]
        public void TestGetUSDGBPOnNewYearsDay2018()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            var mockedResponsesByRequestUri = new Dictionary<string, HttpResponseMessage>()
            {
                {
                    "https://api.fixer.io/latest?base=GBP&date=2018-1-1",
                    new HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent
                        (
                            "{ base: \"GBP\", date: \"2017-12-29\", rates: { USD: 1.3517 } }"
                        )
                    }
                }
            };
            HttpClientHandler httpClientHandler = new MockHttpClientHandler(mockedResponsesByRequestUri);
            IHttpClientFactory httpClientFactory = new MockHttpClientFactory(httpClientHandler);

            var service = new FixerIOCurrencyExchangeService(
                loggerFactory.CreateLogger<FixerIOCurrencyExchangeService>(),
                httpClientFactory
            );

            decimal gbpUsdRate = service.GetExchangeRate("GBP", "USD", new DateTime(2018, 1, 1));

            Assert.AreEqual(1.3517m, gbpUsdRate);
        }
    }
}
