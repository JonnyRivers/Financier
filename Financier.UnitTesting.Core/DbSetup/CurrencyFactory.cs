using Financier.Entities;
using System.Collections.Generic;

namespace Financier.UnitTesting.DbSetup
{
    public class CurrencyFactory
    {
        private Dictionary<CurrencyPrefab, Entities.Currency> m_entitiesByPrefab;

        public CurrencyFactory()
        {
            m_entitiesByPrefab = new Dictionary<CurrencyPrefab, Currency>
            {
                {
                    CurrencyPrefab.Usd,
                    new Currency
                    {
                        Name = "US Dollar",
                        ShortName = "USD",
                        Symbol = "$"
                    }
                },
                {
                    CurrencyPrefab.Gbp,
                    new Currency
                    {
                        Name = "UK Sterling",
                        ShortName = "GBP",
                        Symbol = "£"
                    }
                }
            };
        }

        public Entities.Currency Create(CurrencyPrefab prefab, bool isPrimary)
        {
            var currencyEntity = new Entities.Currency
            {
                Name = m_entitiesByPrefab[prefab].Name,
                ShortName = m_entitiesByPrefab[prefab].ShortName,
                Symbol = m_entitiesByPrefab[prefab].Symbol,
                IsPrimary = isPrimary
            };

            return currencyEntity;
        }

        public void Add(FinancierDbContext dbContext, Entities.Currency currencyEntity)
        {
            dbContext.Currencies.Add(currencyEntity);
            dbContext.SaveChanges();
        }
    }
}
