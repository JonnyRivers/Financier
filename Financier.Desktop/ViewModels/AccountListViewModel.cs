using Financier.Data;
using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountListViewModel : BaseViewModel, IAccountListViewModel
    {
        public AccountListViewModel(FinancierDbContext dbContext, IAccountBalanceService accountBalanceService)
        {
            m_dbContext = dbContext;
            m_accountBalanceService = accountBalanceService;

            PopulateAccounts();
        }

        private FinancierDbContext m_dbContext;
        private IAccountBalanceService m_accountBalanceService;

        private IAccountItemViewModel m_selectedAccount;

        public ObservableCollection<IAccountItemViewModel> Accounts { get; set; }
        public IAccountItemViewModel SelectedAccount
        {
            get { return m_selectedAccount; }
            set
            {
                if (m_selectedAccount != value)
                {
                    m_selectedAccount = value;

                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand CreateCommand => new RelayCommand(CreateExecute);
        public ICommand EditCommand => new RelayCommand(EditExecute, EditCanExecute);

        private void CreateExecute(object obj)
        {
            IWindowFactory windowFactory = IoC.ServiceProvider.Instance.GetRequiredService<IWindowFactory>();
            Window accountCreateWindow = windowFactory.CreateAccountCreateWindow();

            bool? result = accountCreateWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                PopulateAccounts();
            }
        }

        private void EditExecute(object obj)
        {
            IWindowFactory windowFactory = IoC.ServiceProvider.Instance.GetRequiredService<IWindowFactory>();
            Window accountEditWindow = windowFactory.CreateAccountEditWindow(SelectedAccount.AccountId);

            accountEditWindow.ShowDialog();

            PopulateAccounts();
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedAccount != null);
        }

        private void PopulateAccounts()
        {
            // It is much more efficient to batch the balance retrieval.
            // We are hitting the database for each account rather than once.
            IEnumerable<IAccountItemViewModel> accountVMs = m_dbContext.Accounts
                .OrderBy(a => a.Name)
                .Select(a =>
                    new AccountItemViewModel(
                        a.AccountId,
                        a.Name,
                        a.Type,
                        a.Currency.Name,
                        m_accountBalanceService.GetBalance(a.AccountId)));
            Accounts = new ObservableCollection<IAccountItemViewModel>(accountVMs);
            OnPropertyChanged(nameof(Accounts));
        }
    }
}
