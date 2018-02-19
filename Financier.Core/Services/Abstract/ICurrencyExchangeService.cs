using System;
using System.Threading.Tasks;

namespace Financier.Services
{
    public interface ICurrencyExchangeService
    {
        Task<decimal> GetExchangeRateAsync(string sourceCurrencyCode, string destinationCurrencyCode, DateTime at);
    }
}
