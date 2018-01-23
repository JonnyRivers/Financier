using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace Financier.Desktop.Services
{
    public class ViewService : IViewService
    {
        private ILogger<ViewService> m_logger;

        public ViewService(ILogger<ViewService> logger)
        {
            m_logger = logger;
        }

        public void OpenMainView()
        {
            var mainWindow = new MainWindow(
                IoC.ServiceProvider.Instance.GetRequiredService<IMainWindowViewModel>());
            mainWindow.Show();
        }

        public bool OpenAccountCreateView()
        {
            var accountEditWindow = new AccountEditWindow(
                IoC.ServiceProvider.Instance.GetRequiredService<IAccountEditViewModel>());
            bool? result = accountEditWindow.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }

        public bool OpenAccountEditView(int accountId)
        {
            IAccountEditViewModel accountEditViewModel = 
                IoC.ServiceProvider.Instance.GetRequiredService<IAccountEditViewModel>();
            // TODO: Account and Transaction VMs set the ID on sub-VMs, where Budgets use a Setup() routine
            // https://github.com/JonnyRivers/Financier/issues/9
            accountEditViewModel.AccountId = accountId;
            var accountEditWindow = new AccountEditWindow(accountEditViewModel);
            bool? result = accountEditWindow.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }

        public bool OpenBudgetCreateView()
        {
            var budgetEditWindow = new BudgetEditWindow(
                IoC.ServiceProvider.Instance.GetRequiredService<IBudgetEditViewModel>());
            bool? result = budgetEditWindow.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }

        public bool OpenBudgetDeleteConfirmationView()
        {
            return OpenDeleteConfirmationView("budget");
        }

        public bool OpenBudgetEditView(int budgetId)
        {
            IBudgetEditViewModel budgetEditViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<IBudgetEditViewModel>();
            // TODO: Account and Transaction VMs set the ID on sub-VMs, where Budgets use a Setup() routine
            // https://github.com/JonnyRivers/Financier/issues/9
            budgetEditViewModel.BudgetId = budgetId;
            var budgetEditWindow = new BudgetEditWindow(budgetEditViewModel);
            bool? result = budgetEditWindow.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }

        public bool OpenBudgetTransactionDeleteConfirmationView()
        {
            return OpenDeleteConfirmationView("budget transaction");
        }

        public bool OpenTransactionCreateView()
        {
            ITransactionEditViewModel transactionEditViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<ITransactionEditViewModel>();
            var transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);
            bool? result = transactionEditWindow.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }

        public bool OpenTransactionDeleteConfirmationView()
        {
            return OpenDeleteConfirmationView("transaction");
        }

        public bool OpenTransactionEditView(int transactionId)
        {
            ITransactionEditViewModel transactionEditViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<ITransactionEditViewModel>();
            // TODO: Account and Transaction VMs set the ID on sub-VMs, where Budgets use a Setup() routine
            // https://github.com/JonnyRivers/Financier/issues/9
            transactionEditViewModel.TransactionId = transactionId;
            var transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);
            bool? result = transactionEditWindow.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }

        private bool OpenDeleteConfirmationView(string context)
        {
            MessageBoxResult confirmResult = MessageBox.Show(
               $"Are you sure you want to delete this {context}?  This cannot be undone.",
               $"Really delete {context}?",
               MessageBoxButton.YesNo
           );

            return (confirmResult == MessageBoxResult.Yes);
        }
    }
}
