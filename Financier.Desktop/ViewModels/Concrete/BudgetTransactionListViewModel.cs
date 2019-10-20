using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class BudgetTransactionListViewModelFactory : IBudgetTransactionListViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IAccountService m_accountService;
        private readonly IBudgetService m_budgetService;
        private readonly IAccountLinkViewModelFactory m_accountLinkViewModelFactory;
        private readonly IBudgetTransactionItemViewModelFactory m_budgetTransactionItemViewModelFactory;
        private readonly IDeleteConfirmationViewService m_deleteConfirmationViewService;

        public BudgetTransactionListViewModelFactory(
            ILoggerFactory loggerFactory,
            IAccountService accountService,
            IBudgetService budgetService,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            IBudgetTransactionItemViewModelFactory budgetTransactionItemViewModelFactory,
            IDeleteConfirmationViewService deleteConfirmationViewService)
        {
            m_loggerFactory = loggerFactory;
            m_accountService = accountService;
            m_budgetService = budgetService;
            m_accountLinkViewModelFactory = accountLinkViewModelFactory;
            m_budgetTransactionItemViewModelFactory = budgetTransactionItemViewModelFactory;
            m_deleteConfirmationViewService = deleteConfirmationViewService;
        }

        public IBudgetTransactionListViewModel Create(int budgetId)
        {
            return new BudgetTransactionListViewModel(
                m_loggerFactory,
                m_accountService,
                m_budgetService,
                m_accountLinkViewModelFactory,
                m_budgetTransactionItemViewModelFactory,
                m_deleteConfirmationViewService,
                budgetId);
        }
    }

    public class BudgetTransactionListViewModel : BaseViewModel, IBudgetTransactionListViewModel
    {
        private readonly ILogger<BudgetTransactionListViewModel> m_logger;
        private readonly IAccountService m_accountService;
        private readonly IBudgetService m_budgetService;
        private readonly IAccountLinkViewModelFactory m_accountLinkViewModelFactory;
        private readonly IBudgetTransactionItemViewModelFactory m_budgetTransactionItemViewModelFactory;
        private readonly IDeleteConfirmationViewService m_deleteConfirmationViewService;

        private ObservableCollection<IAccountLinkViewModel> m_accountLinks;

        private int m_budgetId;
        private IBudgetTransactionItemViewModel m_selectedTransaction;

        public BudgetTransactionListViewModel(
            ILoggerFactory loggerFactory,
            IAccountService accountService,
            IBudgetService budgetService,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            IBudgetTransactionItemViewModelFactory budgetTransactionItemViewModelFactory,
            IDeleteConfirmationViewService deleteConfirmationViewService,
            int budgetId)
        {
            m_logger = loggerFactory.CreateLogger<BudgetTransactionListViewModel>();
            m_accountService = accountService;
            m_budgetService = budgetService;
            m_accountLinkViewModelFactory = accountLinkViewModelFactory;
            m_budgetTransactionItemViewModelFactory = budgetTransactionItemViewModelFactory;
            m_deleteConfirmationViewService = deleteConfirmationViewService;

            m_accountLinks = new ObservableCollection<IAccountLinkViewModel>(
                m_accountService
                    .GetAllAsLinks()
                    .OrderBy(al => al.Name)
                    .Select(al => m_accountLinkViewModelFactory.Create(al)));

            m_budgetId = budgetId;

            if (m_budgetId == 0)
            {
                int firstIncomeAccountId = 0;
                int firstAssetAccountId = 0;
                int secondAssetAccountId = 0;

                IAccountLinkViewModel firstIncomeAccount =
                    m_accountLinks.FirstOrDefault(al => al.Type == AccountType.Income);
                IAccountLinkViewModel firstAssetAccount =
                    m_accountLinks.FirstOrDefault(al => al.Type == AccountType.Asset);
                IAccountLinkViewModel secondAssetAccount =
                    m_accountLinks.Where(al => al.Type == AccountType.Asset)
                                  .ElementAtOrDefault(1);

                if (firstIncomeAccount != null)
                    firstIncomeAccountId = firstIncomeAccount.AccountId;
                if (firstAssetAccount != null)
                    firstAssetAccountId = secondAssetAccount.AccountId;
                if (secondAssetAccount != null)
                    secondAssetAccountId = secondAssetAccount.AccountId;

                var transactions = new List<IBudgetTransactionItemViewModel>();
                BudgetTransaction initialTransaction = new BudgetTransaction
                {
                    CreditAccount = new AccountLink { AccountId = firstIncomeAccountId },
                    DebitAccount = new AccountLink { AccountId = firstAssetAccountId },
                    Amount = 0
                };
                BudgetTransaction surplusTransaction = new BudgetTransaction
                {
                    CreditAccount = new AccountLink { AccountId = firstAssetAccountId },
                    DebitAccount = new AccountLink { AccountId = secondAssetAccountId },
                    Amount = 0
                };
                transactions.Add(CreateItemViewModel(initialTransaction, BudgetTransactionType.Initial));
                transactions.Add(CreateItemViewModel(surplusTransaction, BudgetTransactionType.Surplus));

                Transactions = new ObservableCollection<IBudgetTransactionItemViewModel>(transactions);
            }
            else
            {
                Budget budget = m_budgetService.Get(m_budgetId);

                var transactions = new List<IBudgetTransactionItemViewModel>();
                transactions.Add(CreateItemViewModel(budget.InitialTransaction, BudgetTransactionType.Initial));
                transactions.AddRange(
                    budget.Transactions.Select(t => CreateItemViewModel(t, BudgetTransactionType.Regular))
                );
                transactions.Add(CreateItemViewModel(budget.SurplusTransaction, BudgetTransactionType.Surplus));

                Transactions = new ObservableCollection<IBudgetTransactionItemViewModel>(transactions);
            }

            CalculateSurplusAmount();
        }

        public ObservableCollection<IBudgetTransactionItemViewModel> Transactions { get; set; }
        public IBudgetTransactionItemViewModel SelectedTransaction
        {
            get { return m_selectedTransaction; }
            set
            {
                if (m_selectedTransaction != value)
                {
                    m_selectedTransaction = value;

                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand CreateCommand => new RelayCommand(CreateExecute);
        public ICommand DeleteCommand => new RelayCommand(DeleteExecute, DeleteCanExecute);

        private void CreateExecute(object obj)
        {
            IBudgetTransactionItemViewModel initialTransactionViewModel = 
                Transactions
                    .Single(t => t.Type == BudgetTransactionType.Initial);
            IAccountLinkViewModel suggestedCreditAccount =
                initialTransactionViewModel.SelectedDebitAccount;

            // For the debit account, we want the next unused asset account.
            // Failing that, we want an asset account.
            // Failing that, we'll take anything!
            IEnumerable<IAccountLinkViewModel> assetAccountLinks = m_accountLinks
                .Where(alvm => alvm.Type == AccountType.Asset &&
                               alvm != suggestedCreditAccount);
            IEnumerable<IAccountLinkViewModel> usedAssetAccountLinks = 
                Transactions
                    .Select(t => t.SelectedDebitAccount)
                    .Where(al => al.Type == AccountType.Asset);
            IEnumerable<IAccountLinkViewModel> unusedAssetAccountLinks =
                assetAccountLinks
                    .Except(usedAssetAccountLinks);

            IAccountLinkViewModel suggestedDebitAccount = null;
            if (unusedAssetAccountLinks.Any())
                suggestedDebitAccount = unusedAssetAccountLinks.First();
            else if (usedAssetAccountLinks.Any())
                suggestedDebitAccount = usedAssetAccountLinks.First();
            else if (m_accountLinks.Any())
                suggestedDebitAccount = m_accountLinks.First();

            var newTransaction = new BudgetTransaction
            {
                CreditAccount = suggestedCreditAccount.ToAccountLink(),
                DebitAccount = suggestedDebitAccount.ToAccountLink(),
                Amount = 0m
            };

            Transactions.Insert(
                Transactions.Count - 1,// we maintain that the surplus transaction is last, so - 1
                CreateItemViewModel(newTransaction, BudgetTransactionType.Regular)
            );
        }

        private void DeleteExecute(object obj)
        {
            if (m_deleteConfirmationViewService.Show("budget transaction"))
            {
                Transactions.Remove(SelectedTransaction);
                SelectedTransaction = null;// is this necessary?
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (
                SelectedTransaction != null && 
                SelectedTransaction.Type == BudgetTransactionType.Regular
            );
        }

        private IBudgetTransactionItemViewModel CreateItemViewModel(
            BudgetTransaction budgetTransaction, 
            BudgetTransactionType type)
        {
            IBudgetTransactionItemViewModel transactionViewModel =
                m_budgetTransactionItemViewModelFactory.Create(
                    m_accountLinks,
                    budgetTransaction,
                    type
            );

            if(type != BudgetTransactionType.Surplus)
            {
                ((BaseViewModel)transactionViewModel).PropertyChanged += OnItemPropertyChanged;
            }

            return transactionViewModel;
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Amount")
            {
                CalculateSurplusAmount();
            }
        }

        private void CalculateSurplusAmount()
        {
            IBudgetTransactionItemViewModel initialTransaction =
                Transactions.Single(t => t.Type == BudgetTransactionType.Initial);

            decimal surplusAmount = initialTransaction.Amount;

            IEnumerable<IBudgetTransactionItemViewModel> regularTransactions =
                Transactions.Where(t => t.Type == BudgetTransactionType.Regular);
            foreach (IBudgetTransactionItemViewModel regularTransaction in regularTransactions)
            {
                if (regularTransaction.SelectedCreditAccount == initialTransaction.SelectedDebitAccount)
                    surplusAmount -= regularTransaction.Amount;
            }

            IBudgetTransactionItemViewModel surplusTransaction =
                Transactions.Single(t => t.Type == BudgetTransactionType.Surplus);
            surplusTransaction.Amount = surplusAmount;
        }
    }
}
