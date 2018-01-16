using Financier.Data;
using Financier.Desktop.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountCreateViewModel : IAccountCreateViewModel
    {
        public AccountCreateViewModel(FinancierDbContext dbContext)
        {
            m_dbContext = dbContext;

            AccountTypes = Enum.GetValues(typeof(AccountType)).Cast<AccountType>();
            Currencies = m_dbContext.Currencies.ToList();

            Name = "New Account";
            SelectedAccountType = AccountType.Asset;
            SelectedCurrency = Currencies.Single(c => c.IsPrimary);
        }

        private FinancierDbContext m_dbContext;

        public IEnumerable<AccountType> AccountTypes { get; }
        public IEnumerable<Currency> Currencies { get; }

        public string Name { get; set; }
        public AccountType SelectedAccountType { get; set; }
        public Currency SelectedCurrency { get; set; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {
            var account = new Account
            {
                Name = Name,
                Type = SelectedAccountType,
                CurrencyId = SelectedCurrency.CurrencyId
            };
            m_dbContext.Accounts.Add(account);
            m_dbContext.SaveChanges();
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
