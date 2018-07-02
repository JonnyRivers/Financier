using Financier.Services;
using Financier.UnitTesting.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Financier.Core.Tests
{
    [TestClass]
    public class FixerIOCurrencyExchangeServiceTests
    {
        [TestMethod]
        public void TestGetUSDGBPOnNewYearsDay2018()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            var environmentServiceMock = new Mock<IEnvironmentService>();
            string fakeKey = "abcd";
            environmentServiceMock
                .Setup(es => es.GetFixerKey())
                .Returns(fakeKey);

            string date = "2018-01-01";
            decimal GbpPerEur = 0.889131m;
            decimal UsdPerEur = 1.201496m;
            decimal UsdPerGbp = UsdPerEur / GbpPerEur;
            var mockedResponsesByRequestUri = new Dictionary<string, HttpResponseMessage>()
            {
                {
                    $"http://data.fixer.io/{date}?access_key={fakeKey}",
                    new HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent
                        (
                            $"{{ base: \"EUR\", date: \"{date}\", rates: {{ GBP: {GbpPerEur}, USD: {UsdPerEur} }} }}"
                        )
                    }
                }
            };
            HttpClientHandler httpClientHandler = new MockHttpClientHandler(mockedResponsesByRequestUri);
            IHttpClientFactory httpClientFactory = new MockHttpClientFactory(httpClientHandler);

            var service = new FixerIOCurrencyExchangeService(
                loggerFactory.CreateLogger<FixerIOCurrencyExchangeService>(),
                environmentServiceMock.Object,
                httpClientFactory
            );

            decimal gbpUsdRate = service.GetExchangeRate("GBP", "USD", new DateTime(2018, 1, 1));

            Assert.AreEqual(UsdPerGbp, gbpUsdRate);
        }
    }
}
