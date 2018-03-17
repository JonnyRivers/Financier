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
        private IAccountRelationshipService m_accountRelationshipService;
        private ITransactionService m_transactionService;
        private ITransactionRelationshipService m_transactionRelationshipService;
        private IViewModelFactory m_viewModelFactory;
        private IViewService m_viewService;

        public AccountTreeViewModel(
            ILogger<AccountTreeViewModel> logger,
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            ITransactionService transactionService,
            ITransactionRelationshipService transactionRelationshipService,
            IViewModelFactory viewModelFactory,
            IViewService viewService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;
            m_transactionService = transactionService;
            m_transactionRelationshipService = transactionRelationshipService;
            m_viewModelFactory = viewModelFactory;
            m_viewService = viewService;

            PopulateAccountTreeItems();
        }

        private ObservableCollection<IAccountTreeItemViewModel> m_accountItems;

        public ObservableCollection<IAccountTreeItemViewModel> AccountItems
        {
            get { return m_accountItems; }
            set
            {
                if (m_accountItems != value)
                {
                    m_accountItems = value;

                    OnPropertyChanged();
                }
            }
        }

        private void PopulateAccountTreeItems()
        {
            IEnumerable<AccountRelationship> accountRelationships = m_accountRelationshipService.GetAll();
            
            IDictionary<int, List<int>> logicalAccountIdsByParentAccountId = 
                accountRelationships
                    .Where(ar => ar.Type == AccountRelationshipType.PhysicalToLogical)
                    .GroupBy(ar => ar.SourceAccount.AccountId)
                    .ToDictionary(
                        g => g.Key, 
                        g => new List<int>(g.Select(ar => ar.DestinationAccount.AccountId)));
            var logicalAccountIds = new HashSet<int>(
                accountRelationships
                    .Where(ar => ar.Type == AccountRelationshipType.PhysicalToLogical)
                    .Select(ar => ar.DestinationAccount.AccountId));

            IEnumerable<Account> accounts = m_accountService.GetAll();
            IEnumerable<Transaction> transactions = m_transactionService.GetAll();

            IDictionary<int, IAccountTreeItemViewModel> logicalAccountVMsById =
                accounts
                    .Where(a => logicalAccountIds.Contains(a.AccountId))
                    .ToDictionary(
                        a => a.AccountId,
                        a => m_viewModelFactory.CreateAccountTreeItemViewModel(a));

            var physicalAccountVMs = new List<IAccountTreeItemViewModel>();
            foreach(Account physicalAccount in accounts.Where(a => !logicalAccountIds.Contains(a.AccountId)))
            {
                if (logicalAccountIdsByParentAccountId.ContainsKey(physicalAccount.AccountId))
                {
                    List<int> childAccountIds = logicalAccountIdsByParentAccountId[physicalAccount.AccountId];
                    IEnumerable<IAccountTreeItemViewModel> childAccountVMs = childAccountIds.Select(id => logicalAccountVMsById[id]);
                    physicalAccountVMs.Add(m_viewModelFactory.CreateAccountTreeItemViewModel(physicalAccount, childAccountVMs));
                }
                else
                {
                    physicalAccountVMs.Add(m_viewModelFactory.CreateAccountTreeItemViewModel(physicalAccount));
                }
            }

            AccountItems = new ObservableCollection<IAccountTreeItemViewModel>(physicalAccountVMs);
        }
    }
}
