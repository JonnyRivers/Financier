using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountRelationshipListViewModelFactory : IAccountRelationshipListViewModelFactory
    {
        private readonly ILogger<AccountRelationshipListViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public AccountRelationshipListViewModelFactory(ILogger<AccountRelationshipListViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IAccountRelationshipListViewModel Create()
        {
            return m_serviceProvider.CreateInstance<AccountRelationshipListViewModel>();
        }
    }

    public class AccountRelationshipListViewModel : BaseViewModel, IAccountRelationshipListViewModel
    {
        private const int AllAccountsId = -1;

        private ILogger<AccountRelationshipListViewModel> m_logger;
        private IAccountService m_accountService;
        private IAccountRelationshipService m_accountRelationshipService;
        private IAccountRelationshipDetailsViewModelFactory m_accountRelationshipDetailsViewModelFactory;
        private IAccountRelationshipItemViewModelFactory m_accountRelationshipItemViewModelFactory;
        private IAccountRelationshipCreateViewService m_accountRelationshipCreateViewService;
        private IAccountRelationshipEditViewService m_accountRelationshipEditViewService;
        private IDeleteConfirmationViewService m_deleteConfirmationViewService;

        public AccountRelationshipListViewModel(
            ILogger<AccountRelationshipListViewModel> logger,
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            IAccountRelationshipDetailsViewModelFactory accountRelationshipDetailsViewModelFactory,
            IAccountRelationshipItemViewModelFactory accountRelationshipItemViewModelFactory,
            IAccountRelationshipCreateViewService accountRelationshipCreateViewService,
            IAccountRelationshipEditViewService accountRelationshipEditViewService,
            IDeleteConfirmationViewService deleteConfirmationViewService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;
            m_accountRelationshipDetailsViewModelFactory = accountRelationshipDetailsViewModelFactory;
            m_accountRelationshipItemViewModelFactory = accountRelationshipItemViewModelFactory;
            m_accountRelationshipCreateViewService = accountRelationshipCreateViewService;
            m_accountRelationshipEditViewService = accountRelationshipEditViewService;
            m_deleteConfirmationViewService = deleteConfirmationViewService;

            PopulateAccountRelationships();
        }
        
        private ObservableCollection<IAccountRelationshipItemViewModel> m_accountRelationships;
        private IAccountRelationshipItemViewModel m_selectedAccountRelationship;

        public ObservableCollection<IAccountRelationshipItemViewModel> AccountRelationships
        {
            get { return m_accountRelationships; }
            set
            {
                if (m_accountRelationships != value)
                {
                    m_accountRelationships = value;

                    OnPropertyChanged();
                }
            }
        }

        public IAccountRelationshipItemViewModel SelectedAccountRelationship
        {
            get { return m_selectedAccountRelationship; }
            set
            {
                if (m_selectedAccountRelationship != value)
                {
                    m_selectedAccountRelationship = value;

                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand CreateCommand => new RelayCommand(CreateExecute);
        public ICommand EditCommand => new RelayCommand(EditExecute, EditCanExecute);
        public ICommand DeleteCommand => new RelayCommand(DeleteExecute, DeleteCanExecute);

        private void CreateExecute(object obj)
        {
            AccountRelationship hint = null;
            IAccountRelationshipItemViewModel hintViewModel = AccountRelationships.FirstOrDefault();
            if (AccountRelationships.Any())
            {
                hint = m_accountRelationshipService.Get(AccountRelationships.First().AccountRelationshipId);
            }

            AccountRelationship newRelationship;
            if (m_accountRelationshipCreateViewService.Show(hint, out newRelationship))
            {
                IAccountRelationshipItemViewModel viewModel = 
                    m_accountRelationshipItemViewModelFactory.Create(newRelationship);
                AccountRelationships.Add(viewModel);
            }
        }

        private void EditExecute(object obj)
        {
            AccountRelationship updatedRelationship;
            if (m_accountRelationshipEditViewService.Show(
                SelectedAccountRelationship.AccountRelationshipId, 
                out updatedRelationship))
            {
                AccountRelationships.Remove(SelectedAccountRelationship);
                SelectedAccountRelationship = m_accountRelationshipItemViewModelFactory.Create(updatedRelationship);
                AccountRelationships.Add(SelectedAccountRelationship);
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedAccountRelationship != null);
        }

        private void DeleteExecute(object obj)
        {
            if (m_deleteConfirmationViewService.Show("account relationship"))
            {
                // Update model
                m_accountRelationshipService.Delete(SelectedAccountRelationship.AccountRelationshipId);

                // Update view model
                AccountRelationships.Remove(SelectedAccountRelationship);
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedAccountRelationship != null);
        }

        private void PopulateAccountRelationships()
        {
            IEnumerable<AccountRelationship> accountRelationships = m_accountRelationshipService.GetAll();
            IEnumerable<IAccountRelationshipItemViewModel> accountRelationshipVMs =
                accountRelationships
                    .Select(ar => m_accountRelationshipItemViewModelFactory.Create(ar));
            AccountRelationships = new ObservableCollection<IAccountRelationshipItemViewModel>(
                accountRelationshipVMs);
            OnPropertyChanged(nameof(AccountRelationships));
        }
    }
}
