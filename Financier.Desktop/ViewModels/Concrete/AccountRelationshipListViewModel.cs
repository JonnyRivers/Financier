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

            PopulateAccountFilters();
            PopulateRelationshipTypeFilters();

            PopulateAccountRelationships();
        }

        private ObservableCollection<IAccountLinkViewModel> m_accountFilters;
        private IAccountLinkViewModel m_selectedAccountFilter;

        public ObservableCollection<IAccountLinkViewModel> AccountFilters
        {
            get { return m_accountFilters; }
            set
            {
                if (m_accountFilters != value)
                {
                    m_accountFilters = value;

                    OnPropertyChanged();
                }
            }
        }

        public IAccountLinkViewModel SelectedAccountFilter
        {
            get { return m_selectedAccountFilter; }
            set
            {
                if (m_selectedAccountFilter != value)
                {
                    m_selectedAccountFilter = value;

                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<IAccountRelationshipTypeViewModel> m_relationshipTypeFilters;
        private IAccountRelationshipTypeViewModel m_selectedRelationshipTpeFilters;

        public ObservableCollection<IAccountRelationshipTypeViewModel> RelationshipTypeFilters
        {
            get { return m_relationshipTypeFilters; }
            set
            {
                if (m_relationshipTypeFilters != value)
                {
                    m_relationshipTypeFilters = value;

                    OnPropertyChanged();
                }
            }
        }

        public IAccountRelationshipTypeViewModel SelectedRelationshipTypeFilter
        {
            get { return m_selectedRelationshipTpeFilters; }
            set
            {
                if (m_selectedRelationshipTpeFilters != value)
                {
                    m_selectedRelationshipTpeFilters = value;

                    OnPropertyChanged();
                }
            }
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

        public ICommand CreateCommand => throw new NotImplementedException();

        public ICommand EditCommand => throw new NotImplementedException();

        public ICommand DeleteCommand => throw new NotImplementedException();

        private void PopulateAccountFilters()
        {
            var accountFilters = new List<AccountLink>();
            accountFilters.Add(
                new AccountLink
                {
                    AccountId = AllAccountsId,
                    Name = "(All Accounts)"
                }
            );
            accountFilters.AddRange(m_accountService.GetAllAsLinks());
            IEnumerable<IAccountLinkViewModel> accountLinkViewModels =
                accountFilters.Select(al => m_viewModelFactory.CreateAccountLinkViewModel(al));

            AccountFilters = new ObservableCollection<IAccountLinkViewModel>(accountLinkViewModels.OrderBy(alvm => alvm.Name));
            SelectedAccountFilter = AccountFilters.Single(af => af.AccountId == AllAccountsId);
        }

        private void PopulateRelationshipTypeFilters()
        {
            var relationshipTypeFilters = new List<AccountRelationshipType?>();
            relationshipTypeFilters.Add(null);
            relationshipTypeFilters.AddRange(Enum.GetValues(typeof(AccountRelationshipType)).Cast<AccountRelationshipType?>());
            IEnumerable<IAccountRelationshipTypeViewModel> viewModels =
                relationshipTypeFilters.Select(t => m_viewModelFactory.CreateAccountRelationshipTypeViewModel(t));
            RelationshipTypeFilters = new ObservableCollection<IAccountRelationshipTypeViewModel>(viewModels);
            SelectedRelationshipTypeFilter = RelationshipTypeFilters.Single(t => !t.Type.HasValue);
        }

        private void PopulateAccountRelationships()
        {
            IEnumerable<AccountRelationship> accountRelationships = m_accountRelationshipService.GetAll();
            IEnumerable<IAccountRelationshipItemViewModel> accountRelationshipVMs =
                accountRelationships.Select(ar => m_viewModelFactory.CreateAccountRelationshipItemViewModel(ar));
            AccountRelationships = new ObservableCollection<IAccountRelationshipItemViewModel>(
                accountRelationshipVMs.OrderBy(ar => ar.AccountRelationshipId));
            OnPropertyChanged(nameof(AccountRelationships));
        }
    }
}
