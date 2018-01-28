using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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

        public int OpenAccountCreateView()
        {
            IAccountEditViewModel accountEditViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<IAccountEditViewModel>();
            accountEditViewModel.SetupForCreate();
            var accountEditWindow = new AccountEditWindow(
                accountEditViewModel);
            bool? result = accountEditWindow.ShowDialog();

            if (result.HasValue)
                return accountEditViewModel.AccountId;

            return 0;
        }

        public bool OpenAccountEditView(int accountId)
        {
            IAccountEditViewModel accountEditViewModel = 
                IoC.ServiceProvider.Instance.GetRequiredService<IAccountEditViewModel>();
            accountEditViewModel.SetupForEdit(accountId);
            var accountEditWindow = new AccountEditWindow(accountEditViewModel);
            bool? result = accountEditWindow.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }

        public int OpenBudgetCreateView()
        {
            IBudgetEditViewModel budgetEditViewModel = 
                IoC.ServiceProvider.Instance.GetRequiredService<IBudgetEditViewModel>();
            budgetEditViewModel.SetupForCreate();
            var budgetEditWindow = new BudgetEditWindow(budgetEditViewModel);
            bool? result = budgetEditWindow.ShowDialog();

            if (result.HasValue)
                return budgetEditViewModel.BudgetId;

            return 0;
        }

        public bool OpenBudgetDeleteConfirmationView()
        {
            return OpenDeleteConfirmationView("budget");
        }

        public bool OpenBudgetEditView(int budgetId)
        {
            IBudgetEditViewModel budgetEditViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<IBudgetEditViewModel>();
            budgetEditViewModel.SetupForEdit(budgetId);
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

        public bool OpenPaydayEventStartView(int budgetId, out PaydayStart paydayStart)
        {
            paydayStart = null;

            IPaydayEventStartViewModel paydayEventStartViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<IPaydayEventStartViewModel>();
            paydayEventStartViewModel.Setup(budgetId);
            var paydayEventStartWindow = new PaydayEventStartWindow(paydayEventStartViewModel);
            bool? result = paydayEventStartWindow.ShowDialog();

            if (result.HasValue)
            {
                paydayStart = paydayEventStartViewModel.ToPaydayStart();
                return result.Value;
            }
            
            return false;
        }

        public bool OpenTransactionBatchCreateConfirmView(IEnumerable<Transaction> transactions)
        {
            throw new System.NotImplementedException();
        }

        public int OpenTransactionCreateView()
        {
            ITransactionEditViewModel transactionEditViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<ITransactionEditViewModel>();
            transactionEditViewModel.SetupForCreate();
            var transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);
            bool? result = transactionEditWindow.ShowDialog();

            if (result.HasValue)
                return transactionEditViewModel.TransactionId;

            return 0;
        }

        public bool OpenTransactionDeleteConfirmationView()
        {
            return OpenDeleteConfirmationView("transaction");
        }

        public bool OpenTransactionEditView(int transactionId)
        {
            ITransactionEditViewModel transactionEditViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<ITransactionEditViewModel>();
            transactionEditViewModel.SetupForEdit(transactionId);
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
