using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class MainViewModelFactory : IMainViewModelFactory
    {
        private readonly ILogger<MainViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public MainViewModelFactory(ILogger<MainViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IMainViewModel Create()
        {
            return m_serviceProvider.CreateInstance<MainViewModel>();
        }
    }

    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        private ILogger<MainViewModel> m_logger;
        private IAccountRelationshipListViewService m_accountRelationshipListViewService;
        private IAccountTreeViewService m_accountTreeViewService;
        private IBalanceSheetViewService m_balanceSheetViewService;
        private IBudgetListViewService m_budgetListViewService;
        private ITransactionListViewService m_transactionListViewService;

        public MainViewModel(
            ILogger<MainViewModel> logger,
            IAccountRelationshipListViewService accountRelationshipListViewService,
            IAccountTreeViewService accountTreeViewService,
            IBalanceSheetViewService balanceSheetViewService,
            IBudgetListViewService budgetListViewService,
            ITransactionListViewService transactionListViewService
        )
        {
            m_logger = logger;
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
