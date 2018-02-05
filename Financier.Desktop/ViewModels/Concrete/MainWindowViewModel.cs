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
        private IRegistryService m_registryService;
        private IViewService m_viewService;

        private string m_currentDatabase;

        public MainWindowViewModel(
            ILogger<MainWindowViewModel> logger,
            IRegistryService registryService,
            IViewService viewService
        )
        {
            m_logger = logger;
            m_registryService = registryService;
            m_viewService = viewService;

            m_currentDatabase = m_registryService.GetCurrentDatabase();
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
