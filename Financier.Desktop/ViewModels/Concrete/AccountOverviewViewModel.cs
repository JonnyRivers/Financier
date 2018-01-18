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
    public class AccountOverviewViewModel : IAccountOverviewViewModel
    {
        public AccountOverviewViewModel(FinancierDbContext dbContext, int accountId)
        {
            m_dbContext = dbContext;
            m_accountId = accountId;

            AccountTypes = Enum.GetValues(typeof(AccountType)).Cast<AccountType>();
            Currencies = m_dbContext.Currencies.ToList();

            Account account = dbContext.Accounts.Single(a => a.AccountId == m_accountId);

            Name = account.Name;
            SelectedAccountType = account.Type;
            SelectedCurrency = account.Currency;
        }

        private FinancierDbContext m_dbContext;
        private int m_accountId;

        public IEnumerable<AccountType> AccountTypes { get; }
        public IEnumerable<Currency> Currencies { get; }

        public string Name { get; set; }
        public AccountType SelectedAccountType { get; set; }
        public Currency SelectedCurrency { get; set; }

        public ICommand ApplyCommand => new RelayCommand(ApplyExecute);

        private void ApplyExecute(object obj)
        {
            Account account = m_dbContext.Accounts.Single(a => a.AccountId == m_accountId);
            account.Currency = SelectedCurrency;
            account.Name = Name;
            account.Type = SelectedAccountType;

            m_dbContext.SaveChanges();
        }
    }
}
