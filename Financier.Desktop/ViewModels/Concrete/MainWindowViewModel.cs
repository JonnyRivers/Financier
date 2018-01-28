using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IMainWindowViewModel
    {
        private ILogger<MainWindowViewModel> m_logger;
        private IViewService m_viewService;

        public MainWindowViewModel(
            ILogger<MainWindowViewModel> logger,
            IViewService viewService
        )
        {
            m_logger = logger;
            m_viewService = viewService;
        }

        public ICommand AccountsViewCommand => new RelayCommand(AccountsViewExecute);
        public ICommand BudgetsViewCommand => new RelayCommand(BudgetsViewExecute);
        public ICommand TransactionsViewCommand => new RelayCommand(TransactionsViewExecute);

        private void AccountsViewExecute(object obj)
        {
            throw new NotImplementedException();
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
