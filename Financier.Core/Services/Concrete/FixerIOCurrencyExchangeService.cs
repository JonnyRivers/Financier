using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace Financier.Services
{
    public class FixerIOCurrencyExchangeService : ICurrencyExchangeService
    {
        private ILogger<FixerIOCurrencyExchangeService> m_logger;
        private IEnvironmentService m_environmentService;
        private IHttpClientFactory m_httpClientFactory;

        public FixerIOCurrencyExchangeService(
            ILoggerFactory loggerFactory,
            IEnvironmentService environmentService,
            IHttpClientFactory httpClientFactory)
        {
            m_logger = loggerFactory.CreateLogger<FixerIOCurrencyExchangeService>();
            m_environmentService = environmentService;
            m_httpClientFactory = httpClientFactory;
        }

        public decimal GetExchangeRate(
            string sourceCurrencyCode, 
            string destinationCurrencyCode, 
            DateTime at)
        {
            HttpClient client = m_httpClientFactory.Create();
            string fixerApiKey = m_environmentService.GetFixerKey();
            string atParameter = $"{at.Year}-{at.Month:00}-{at.Day:00}";
            string requestUri = $"http://data.fixer.io/{atParameter}?access_key={fixerApiKey}";
            HttpResponseMessage responseMessage = client.GetAsync(requestUri).Result;

            if (!responseMessage.IsSuccessStatusCode)
                throw new HttpRequestException($"Received {responseMessage.StatusCode} from call to {requestUri}");

            string responseContent = responseMessage.Content.ReadAsStringAsync().Result;

            JObject responseJsonObject = JObject.Parse(responseContent);
            decimal sourceRate = GetCountryRate(responseJsonObject, sourceCurrencyCode);
            decimal destinationRate = GetCountryRate(responseJsonObject, destinationCurrencyCode);

            return destinationRate / sourceRate;
        }

        private static decimal GetCountryRate(JObject responseJsonObject, string currencyCode)
        {
            if (currencyCode == (string)responseJsonObject["base"])
                return 1m;

            JObject ratesJsonObject = (JObject)responseJsonObject["rates"];
            string rateText = (string)ratesJsonObject[currencyCode];
            return Decimal.Parse(rateText);
        }
    }
}
