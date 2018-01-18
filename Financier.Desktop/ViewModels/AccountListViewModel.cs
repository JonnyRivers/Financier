using Financier.Data;
using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountListViewModel : BaseViewModel, IAccountListViewModel
    {
        private ILogger<AccountListViewModel> m_logger;
        private FinancierDbContext m_dbContext;
        private IAccountBalanceService m_accountBalanceService;
        private IViewService m_viewService;

        public AccountListViewModel(
            ILogger<AccountListViewModel> logger,
            FinancierDbContext dbContext, 
            IAccountBalanceService accountBalanceService,
            IViewService viewService)
        {
            m_logger = logger;
            m_dbContext = dbContext;
            m_accountBalanceService = accountBalanceService;
            m_viewService = viewService;

            PopulateAccounts();
        }

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
            if (m_viewService.OpenAccountCreateView())
            {
                PopulateAccounts();
            }
        }

        private void EditExecute(object obj)
        {
            if (m_viewService.OpenAccountEditView(SelectedAccount.AccountId))
            {
                PopulateAccounts();
            }
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
