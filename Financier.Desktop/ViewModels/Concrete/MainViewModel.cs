using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class MainViewModelFactory : IMainViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IAccountRelationshipListViewService m_accountRelationshipListViewService;
        private readonly IAccountTreeViewService m_accountTreeViewService;
        private readonly IBalanceSheetViewService m_balanceSheetViewService;
        private readonly IBudgetListViewService m_budgetListViewService;
        private readonly ITransactionListViewService m_transactionListViewService;

        public MainViewModelFactory(
            ILoggerFactory loggerFactory,
            IAccountRelationshipListViewService accountRelationshipListViewService,
            IAccountTreeViewService accountTreeViewService,
            IBalanceSheetViewService balanceSheetViewService,
            IBudgetListViewService budgetListViewService,
            ITransactionListViewService transactionListViewService)
        {
            m_loggerFactory = loggerFactory;
            m_accountRelationshipListViewService = accountRelationshipListViewService;
            m_accountTreeViewService = accountTreeViewService;
            m_balanceSheetViewService = balanceSheetViewService;
            m_budgetListViewService = budgetListViewService;
            m_transactionListViewService = transactionListViewService;
        }

        public IMainViewModel Create()
        {
            return new MainViewModel(
                m_loggerFactory,
                m_accountRelationshipListViewService,
                m_accountTreeViewService,
                m_balanceSheetViewService,
                m_budgetListViewService,
                m_transactionListViewService);
        }
    }

    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        private readonly ILogger<MainViewModel> m_logger;
        private readonly IAccountRelationshipListViewService m_accountRelationshipListViewService;
        private readonly IAccountTreeViewService m_accountTreeViewService;
        private readonly IBalanceSheetViewService m_balanceSheetViewService;
        private readonly IBudgetListViewService m_budgetListViewService;
        private readonly ITransactionListViewService m_transactionListViewService;

        public MainViewModel(
            ILoggerFactory loggerFactory,
            IAccountRelationshipListViewService accountRelationshipListViewService,
            IAccountTreeViewService accountTreeViewService,
            IBalanceSheetViewService balanceSheetViewService,
            IBudgetListViewService budgetListViewService,
            ITransactionListViewService transactionListViewService)
        {
            m_logger = loggerFactory.CreateLogger<MainViewModel>();
            m_accountRelationshipListViewService = accountRelationshipListViewService;
            m_accountTreeViewService = accountTreeViewService;
            m_balanceSheetViewService = balanceSheetViewService;
            m_budgetListViewService = budgetListViewService;
            m_transactionListViewService = transactionListViewService;
        }

        public ICommand AccountsViewCommand => new RelayCommand(AccountsViewExecute);
        public ICommand AccountRelationshipsViewCommand => new RelayCommand(AccountRelationshipsViewExecute);
        public ICommand BalanceSheetViewCommand => new RelayCommand(BalanceSheetViewExecute);
        public ICommand BudgetsViewCommand => new RelayCommand(BudgetsViewExecute);
        public ICommand TransactionsViewCommand => new RelayCommand(TransactionsViewExecute);

        private void AccountsViewExecute(object obj)
        {
            m_accountTreeViewService.Show();
        }

        private void AccountRelationshipsViewExecute(object obj)
        {
            m_accountRelationshipListViewService.Show();
        }

        private void BalanceSheetViewExecute(object obj)
        {
            m_balanceSheetViewService.Show();
        }

        private void BudgetsViewExecute(object obj)
        {
            m_budgetListViewService.Show();
        }

        private void TransactionsViewExecute(object obj)
        {
            m_transactionListViewService.Show();
        }
    }
}
