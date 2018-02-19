using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Services
{
    public class FixerIOCurrencyExchangeService : ICurrencyExchangeService
    {
        private ILogger<FixerIOCurrencyExchangeService> m_logger;

        public FixerIOCurrencyExchangeService(ILogger<FixerIOCurrencyExchangeService> logger)
        {
            m_logger = logger;
        }

        public decimal GetExchangeRate(
            string sourceCurrencyCode, 
            string destinationCurrencyCode, 
            DateTime at)
        {
            var client = new HttpClient();
            string atParameter = $"{at.Year}-{at.Month}-{at.Day}";
            string requestUri = $"https://api.fixer.io/latest?base={sourceCurrencyCode}&date={atParameter}";
            HttpResponseMessage responseMessage = client.GetAsync(requestUri).Result;

            if (!responseMessage.IsSuccessStatusCode)
                throw new HttpRequestException($"Received {responseMessage.StatusCode} from call to {requestUri}");

            string responseContent = responseMessage.Content.ReadAsStringAsync().Result;

            JObject responseJsonObject = JObject.Parse(responseContent);
            JObject ratesJsonObject = (JObject)responseJsonObject["rates"];
            string rateText = (string)ratesJsonObject[destinationCurrencyCode];
            decimal rate = Decimal.Parse(rateText);

            return rate;
        }
    }
}
