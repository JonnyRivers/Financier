using Financier.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    public class CurrencyService : ICurrencyService
    {
        private ILogger<CurrencyService> m_logger;
        private FinancierDbContext m_dbContext;

        public CurrencyService(ILogger<CurrencyService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public IEnumerable<Currency> GetAll()
        {
            return m_dbContext.Currencies;
        }

        public Currency GetPrimary()
        {
            return m_dbContext.Currencies.Single(c => c.IsPrimary);
        }
    }
}
