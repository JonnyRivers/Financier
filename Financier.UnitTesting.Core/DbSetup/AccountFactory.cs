using System.Collections.Generic;

namespace Financier.UnitTesting.DbSetup
{
    public class AccountFactory
    {
        private Dictionary<AccountPrefab, Entities.Account> m_entitiesByPrefab;

        public AccountFactory()
        {
            m_entitiesByPrefab = new Dictionary<AccountPrefab, Entities.Account>
            {
                {
                    AccountPrefab.Capital,
                    new Entities.Account
                    {
                        Name = "Capital",
                        Type = AccountType.Capital,
                        SubType = AccountSubType.None
                    }
                },
                {
                    AccountPrefab.Checking,
                    new Entities.Account
                    {
                        Name = "Checking",
                        Type = AccountType.Asset,
                        SubType = AccountSubType.Checking
                    }
                },
                {
                    AccountPrefab.CreditCard,
                    new Entities.Account
                    {
                        Name = "Credit Card",
                        Type = AccountType.Liability,
                        SubType = AccountSubType.CreditCard
                    }
                },
                {
                    AccountPrefab.GroceriesExpense,
                    new Entities.Account
                    {
                        Name = "Groceries Expense",
                        Type = AccountType.Expense,
                        SubType = AccountSubType.None
                    }
                },
                {
                    AccountPrefab.GroceriesPrepayment,
                    new Entities.Account
                    {
                        Name = "Groceries Prepayment",
                        Type = AccountType.Asset,
                        SubType = AccountSubType.None
                    }
                },
                {
                    AccountPrefab.Income,
                    new Entities.Account
                    {
                        Name = "Income",
                        Type = AccountType.Income,
                        SubType = AccountSubType.None
                    }
                },
                {
                    AccountPrefab.RentExpense,
                    new Entities.Account
                    {
                        Name = "Rent Expense",
                        Type = AccountType.Expense,
                        SubType = AccountSubType.None
                    }
                },
                {
                    AccountPrefab.RentPrepayment,
                    new Entities.Account
                    {
                        Name = "Rent Prepayment",
                        Type = AccountType.Asset,
                        SubType = AccountSubType.None
                    }
                },
                {
                    AccountPrefab.Savings,
                    new Entities.Account
                    {
                        Name = "Savings",
                        Type = AccountType.Asset,
                        SubType = AccountSubType.None
                    }
                }
            };
        }

        public Entities.Account Create(AccountPrefab prefab, Entities.Currency currencyEntity)
        {
            var accountEntity = new Entities.Account
            {
                Currency = currencyEntity,
                Name = m_entitiesByPrefab[prefab].Name,
                Type = m_entitiesByPrefab[prefab].Type,
                SubType = m_entitiesByPrefab[prefab].SubType
            };

            return accountEntity;
        }

        public void Add(Entities.FinancierDbContext dbContext, Entities.Account accountEntity)
        {
            dbContext.Accounts.Add(accountEntity);
            dbContext.SaveChanges();
        }
    }
}
