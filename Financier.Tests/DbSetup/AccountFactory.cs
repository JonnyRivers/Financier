using System.Collections.Generic;

namespace Financier.Tests.DbSetup
{
    class AccountFactory
    {
        private Dictionary<AccountPrefab, Entities.Account> m_entitiesByPrefab;

        internal AccountFactory()
        {
            m_entitiesByPrefab = new Dictionary<AccountPrefab, Entities.Account>
            {
                {
                    AccountPrefab.Capital,
                    new Entities.Account
                    {
                        Name = "Capital",
                        Type = Entities.AccountType.Capital
                    }
                },
                {
                    AccountPrefab.Checking,
                    new Entities.Account
                    {
                        Name = "Checking",
                        Type = Entities.AccountType.Asset
                    }
                },
                {
                    AccountPrefab.CreditCard,
                    new Entities.Account
                    {
                        Name = "Credit Card",
                        Type = Entities.AccountType.Liability
                    }
                },
                {
                    AccountPrefab.GroceriesExpense,
                    new Entities.Account
                    {
                        Name = "Groceries Expense",
                        Type = Entities.AccountType.Expense
                    }
                },
                {
                    AccountPrefab.GroceriesPrepayment,
                    new Entities.Account
                    {
                        Name = "Groceries Prepayment",
                        Type = Entities.AccountType.Asset
                    }
                },
                {
                    AccountPrefab.Income,
                    new Entities.Account
                    {
                        Name = "Income",
                        Type = Entities.AccountType.Income
                    }
                },
                {
                    AccountPrefab.RentExpense,
                    new Entities.Account
                    {
                        Name = "Rent Expense",
                        Type = Entities.AccountType.Expense
                    }
                },
                {
                    AccountPrefab.RentPrepayment,
                    new Entities.Account
                    {
                        Name = "Rent Prepayment",
                        Type = Entities.AccountType.Asset
                    }
                },
                {
                    AccountPrefab.Savings,
                    new Entities.Account
                    {
                        Name = "Savings",
                        Type = Entities.AccountType.Asset
                    }
                }
            };
        }

        internal Entities.Account Create(AccountPrefab prefab, Entities.Currency currencyEntity)
        {
            var accountEntity = new Entities.Account
            {
                Currency = currencyEntity,
                Name = m_entitiesByPrefab[prefab].Name,
                Type = m_entitiesByPrefab[prefab].Type
            };

            return accountEntity;
        }

        internal void Add(Entities.FinancierDbContext dbContext, Entities.Account accountEntity)
        {
            dbContext.Accounts.Add(accountEntity);
            dbContext.SaveChanges();
        }
    }
}
