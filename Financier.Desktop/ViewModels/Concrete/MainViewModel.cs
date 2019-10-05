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
        private IViewService m_viewService;

        public MainViewModel(
            ILogger<MainViewModel> logger,
            IViewService viewService
        )
        {
            m_logger = logger;
            m_viewService = viewService;
        }

        public ICommand AccountsViewCommand => new RelayCommand(AccountsViewExecute);
        public ICommand AccountRelationshipsViewCommand => new RelayCommand(AccountRelationshipsViewExecute);
        public ICommand BalanceSheetViewCommand => new RelayCommand(BalanceSheetViewExecute);
        public ICommand BudgetsViewCommand => new RelayCommand(BudgetsViewExecute);
        public ICommand TransactionsViewCommand => new RelayCommand(TransactionsViewExecute);

        private void AccountsViewExecute(object obj)
        {
            m_viewService.OpenAccountTreeView();
        }

        private void AccountRelationshipsViewExecute(object obj)
        {
            m_viewService.OpenAccountRelationshipListView();
        }

        private void BalanceSheetViewExecute(object obj)
        {
            m_viewService.OpenBalanceSheetView();
        }

        private void BudgetsViewExecute(object obj)
        {
            m_viewService.OpenBudgetListView();
        }

        private void TransactionsViewExecute(object obj)
        {
            m_viewService.OpenTransactionListView();
        }
    }
}
