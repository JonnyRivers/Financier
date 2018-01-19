using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    public class CurrencyService : ICurrencyService
    {
        private ILogger<CurrencyService> m_logger;
        private Entities.FinancierDbContext m_dbContext;

        public CurrencyService(ILogger<CurrencyService> logger, Entities.FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public IEnumerable<Currency> GetAll()
        {
            return m_dbContext.Currencies.Select(FromEntity).ToList();
        }

        public Currency GetPrimary()
        {
            return m_dbContext.Currencies.Select(FromEntity).Single(c => c.IsPrimary);
        }

        private static Currency FromEntity(Entities.Currency currencyEntity)
        {
            return new Currency
            {
                CurrencyId = currencyEntity.CurrencyId,
                IsPrimary = currencyEntity.IsPrimary,
                Name = currencyEntity.Name,
                ShortName = currencyEntity.ShortName,
                Symbol = currencyEntity.Symbol
            };
        }
    }
}
