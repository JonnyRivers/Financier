using Financier.Data;
using Financier.Desktop.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountEditViewModel : IAccountEditViewModel
    {
        public AccountEditViewModel(FinancierDbContext dbContext)
        {
            m_dbContext = dbContext;
            m_accountId = 0;

            AccountTypes = Enum.GetValues(typeof(AccountType)).Cast<AccountType>();
            Currencies = m_dbContext.Currencies.ToList();

            Name = "New Account";
            SelectedAccountType = AccountType.Asset;
            SelectedCurrency = Currencies.Single(c => c.IsPrimary);
        }

        private FinancierDbContext m_dbContext;
        private int m_accountId;

        public IEnumerable<AccountType> AccountTypes { get; }
        public IEnumerable<Currency> Currencies { get; }

        public int AccountId
        {
            get { return m_accountId; }
            set
            {
                if(value != m_accountId)
                {
                    m_accountId = value;

                    Account account = m_dbContext.Accounts.Single(a => a.AccountId == m_accountId);
                    Name = account.Name;
                    SelectedAccountType = account.Type;
                    SelectedCurrency = account.Currency;
                }
            }
        }

        public string Name { get; set; }
        public AccountType SelectedAccountType { get; set; }
        public Currency SelectedCurrency { get; set; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {
            if (m_accountId != 0)
            {
                Account account = m_dbContext.Accounts.Single(a => a.AccountId == m_accountId);
                account.Name = Name;
                account.Type = SelectedAccountType;
                account.CurrencyId = SelectedCurrency.CurrencyId;
                
            }
            else
            {
                var account = new Account
                {
                    Name = Name,
                    Type = SelectedAccountType,
                    CurrencyId = SelectedCurrency.CurrencyId
                };
                m_dbContext.Accounts.Add(account);
            }

            m_dbContext.SaveChanges();
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
