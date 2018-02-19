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
    public class AccountRelationshipListViewModel : BaseViewModel, IAccountRelationshipListViewModel
    {
        private const int AllAccountsId = -1;

        private ILogger<AccountRelationshipListViewModel> m_logger;
        private IAccountService m_accountService;
        private IAccountRelationshipService m_accountRelationshipService;
        private IViewModelFactory m_viewModelFactory;
        private IViewService m_viewService;

        public AccountRelationshipListViewModel(
            ILogger<AccountRelationshipListViewModel> logger,
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            IViewModelFactory viewModelFactory,
            IViewService viewService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;
            m_viewModelFactory = viewModelFactory;
            m_viewService = viewService;

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
            if (m_viewService.OpenAccountRelationshipCreateView(hint, out newRelationship))
            {
                PopulateAccountRelationships();
            }
        }

        private void EditExecute(object obj)
        {
            if (m_viewService.OpenAccountRelationshipEditView(SelectedAccountRelationship.AccountRelationshipId))
            {
                PopulateAccountRelationships();
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedAccountRelationship != null);
        }

        private void DeleteExecute(object obj)
        {
            if (m_viewService.OpenAccountRelationshipDeleteConfirmationView())
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
                    .Select(ar => m_viewModelFactory.CreateAccountRelationshipItemViewModel(ar))
                    .OrderBy(ar => ar.SourceAccount.Name)
                    .ThenBy(ar => ar.DestinationAccount.Name)
                    .ThenBy(ar => ar.AccountRelationshipId);
            AccountRelationships = new ObservableCollection<IAccountRelationshipItemViewModel>(
                accountRelationshipVMs);
            OnPropertyChanged(nameof(AccountRelationships));
        }
    }
}
