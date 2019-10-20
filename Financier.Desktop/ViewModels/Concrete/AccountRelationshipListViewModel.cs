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
    public class AccountRelationshipListViewModelFactory : IAccountRelationshipListViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IAccountRelationshipService m_accountRelationshipService;
        private readonly IAccountRelationshipItemViewModelFactory m_accountRelationshipItemViewModelFactory;
        private readonly IAccountRelationshipCreateViewService m_accountRelationshipCreateViewService;
        private readonly IAccountRelationshipEditViewService m_accountRelationshipEditViewService;
        private readonly IDeleteConfirmationViewService m_deleteConfirmationViewService;

        public AccountRelationshipListViewModelFactory(
            ILoggerFactory loggerFactory,
            IAccountRelationshipService accountRelationshipService,
            IAccountRelationshipItemViewModelFactory accountRelationshipItemViewModelFactory,
            IAccountRelationshipCreateViewService accountRelationshipCreateViewService,
            IAccountRelationshipEditViewService accountRelationshipEditViewService,
            IDeleteConfirmationViewService deleteConfirmationViewService)
        {
            m_loggerFactory = loggerFactory;
            m_accountRelationshipService = accountRelationshipService;
            m_accountRelationshipItemViewModelFactory = accountRelationshipItemViewModelFactory;
            m_accountRelationshipCreateViewService = accountRelationshipCreateViewService;
            m_accountRelationshipEditViewService = accountRelationshipEditViewService;
            m_deleteConfirmationViewService = deleteConfirmationViewService;
        }

        public IAccountRelationshipListViewModel Create()
        {
            return new AccountRelationshipListViewModel(
                m_loggerFactory,
                m_accountRelationshipService,
                m_accountRelationshipItemViewModelFactory,
                m_accountRelationshipCreateViewService,
                m_accountRelationshipEditViewService,
                m_deleteConfirmationViewService);
        }
    }

    public class AccountRelationshipListViewModel : BaseViewModel, IAccountRelationshipListViewModel
    {
        private const int AllAccountsId = -1;

        private ILogger<AccountRelationshipListViewModel> m_logger;
        private IAccountRelationshipService m_accountRelationshipService;
        private IAccountRelationshipItemViewModelFactory m_accountRelationshipItemViewModelFactory;
        private IAccountRelationshipCreateViewService m_accountRelationshipCreateViewService;
        private IAccountRelationshipEditViewService m_accountRelationshipEditViewService;
        private IDeleteConfirmationViewService m_deleteConfirmationViewService;

        public AccountRelationshipListViewModel(
            ILoggerFactory loggerFactory,
            IAccountRelationshipService accountRelationshipService,
            IAccountRelationshipItemViewModelFactory accountRelationshipItemViewModelFactory,
            IAccountRelationshipCreateViewService accountRelationshipCreateViewService,
            IAccountRelationshipEditViewService accountRelationshipEditViewService,
            IDeleteConfirmationViewService deleteConfirmationViewService)
        {
            m_logger = loggerFactory.CreateLogger<AccountRelationshipListViewModel>();
            m_accountRelationshipService = accountRelationshipService;
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
