using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountTreeViewModel : BaseViewModel, IAccountTreeViewModel
    {
        private ILogger<AccountTreeViewModel> m_logger;
        private IAccountService m_accountService;
        private ITransactionService m_transactionService;
        private ITransactionRelationshipService m_transactionRelationshipService;
        private IViewModelFactory m_viewModelFactory;
        private IViewService m_viewService;

        public AccountTreeViewModel(
            ILogger<AccountTreeViewModel> logger,
            IAccountService accountService,
            ITransactionService transactionService,
            ITransactionRelationshipService transactionRelationshipService,
            IViewModelFactory viewModelFactory,
            IViewService viewService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_transactionService = transactionService;
            m_transactionRelationshipService = transactionRelationshipService;
            m_viewModelFactory = viewModelFactory;
            m_viewService = viewService;

            PopulateAccountTreeItems();
        }

        private ObservableCollection<IAccountTreeItemViewModel> m_accountTreeItems;

        public ObservableCollection<IAccountTreeItemViewModel> AccountTreeItems
        {
            get { return m_accountTreeItems; }
            set
            {
                if (m_accountTreeItems != value)
                {
                    m_accountTreeItems = value;

                    OnPropertyChanged();
                }
            }
        }

        private void PopulateAccountTreeItems()
        {
            IEnumerable<Account> accounts = m_accountService.GetAll();
            IEnumerable<IAccountTreeItemViewModel> accountTreeItemVMs =
                accounts.Select(a => m_viewModelFactory.CreateAccountTreeItemViewModel(a));
            AccountTreeItems = new ObservableCollection<IAccountTreeItemViewModel>(accountTreeItemVMs);
        }
    }
}
