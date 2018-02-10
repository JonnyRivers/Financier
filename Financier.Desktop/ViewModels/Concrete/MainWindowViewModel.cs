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
        private IDataConfigService m_dataConfigService;
        private IViewService m_viewService;

        private string m_currentDatabase;

        public MainWindowViewModel(
            ILogger<MainWindowViewModel> logger,
            IDataConfigService dataConfigService,
            IViewService viewService
        )
        {
            m_logger = logger;
            m_dataConfigService = dataConfigService;
            m_viewService = viewService;

            m_currentDatabase = dataConfigService.GetCurrentDatabase();
        }

        public string CurrentDatabase => m_currentDatabase;

        public ICommand AccountsViewCommand => new RelayCommand(AccountsViewExecute);
        public ICommand BudgetsViewCommand => new RelayCommand(BudgetsViewExecute);
        public ICommand TransactionsViewCommand => new RelayCommand(TransactionsViewExecute);

        private void AccountsViewExecute(object obj)
        {
            m_viewService.OpenAccountListView();
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
