using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IMainWindowViewModel
    {
        private ILogger<MainWindowViewModel> m_logger;
        private IEnvironmentService m_environmentService;
        private IViewService m_viewService;

        public MainWindowViewModel(
            ILogger<MainWindowViewModel> logger,
            IEnvironmentService environmentService,
            IViewService viewService
        )
        {
            m_logger = logger;
            m_environmentService = environmentService;
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
