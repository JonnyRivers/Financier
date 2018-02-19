using System;

namespace Financier.Services
{
    public interface ICurrencyExchangeService
    {
        decimal GetExchangeRate(string sourceCurrencyCode, string destinationCurrencyCode, DateTime at);
    }
}
