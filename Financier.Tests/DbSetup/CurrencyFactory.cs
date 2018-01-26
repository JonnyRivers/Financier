using Financier.Entities;
using System.Collections.Generic;

namespace Financier.Tests.DbSetup
{
    internal class CurrencyFactory
    {
        private Dictionary<CurrencyPrefab, Entities.Currency> m_entitiesByPrefab;

        internal CurrencyFactory()
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

        internal Entities.Currency Create(CurrencyPrefab prefab, bool isPrimary)
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

        internal void Add(FinancierDbContext dbContext, Entities.Currency currencyEntity)
        {
            dbContext.Currencies.Add(currencyEntity);
            dbContext.SaveChanges();
        }
    }
}
