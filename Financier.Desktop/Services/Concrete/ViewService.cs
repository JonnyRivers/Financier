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

        public bool OpenAccountCreateView(out Account account)
        {
            account = null;

            IAccountEditViewModel viewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<IAccountEditViewModel>();
            viewModel.SetupForCreate();
            var window = new AccountEditWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                account = viewModel.ToAccount();
                return true;
            }

            return false;
        }

        public bool OpenAccountEditView(int accountId, out Account account)
        {
            account = null;

            IAccountEditViewModel viewModel = 
                IoC.ServiceProvider.Instance.GetRequiredService<IAccountEditViewModel>();
            viewModel.SetupForEdit(accountId);
            var window = new AccountEditWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                account = viewModel.ToAccount();
                return true;
            }

            return false;
        }

        public void OpenAccountListView()
        {
            IAccountListViewModel viewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<IAccountListViewModel>();
            var window = new AccountListWindow(viewModel);
            window.ShowDialog();
        }

        public bool OpenAccountTransactionsEditView(int accountId)
        {
            IAccountTransactionListViewModel viewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<IAccountTransactionListViewModel>();
            viewModel.Setup(accountId);
            var window = new AccountTransactionListWindow(viewModel);
            bool? result = window.ShowDialog();

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

        public void OpenBudgetListView()
        {
            IBudgetListViewModel viewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<IBudgetListViewModel>();
            var window = new BudgetListWindow(viewModel);
            window.ShowDialog();
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

            if (result.HasValue && result.Value)
            {
                paydayStart = paydayEventStartViewModel.ToPaydayStart();
                return true;
            }
            
            return false;
        }

        public bool OpenTransactionBatchCreateConfirmView(IEnumerable<Transaction> transactions)
        {
            ITransactionBatchCreateConfirmViewModel viewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<ITransactionBatchCreateConfirmViewModel>();
            viewModel.Setup(transactions);
            var window = new TransactionBatchCreateConfirmWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }

        public bool OpenTransactionCreateView(out Transaction transaction)
        {
            transaction = null;

            ITransactionEditViewModel transactionEditViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<ITransactionEditViewModel>();
            transactionEditViewModel.SetupForCreate();
            var transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);
            bool? result = transactionEditWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                transaction = transactionEditViewModel.ToTransaction();
                return true;
            }

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
            transactionEditViewModel.SetupForEdit(transactionId);
            var transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);
            bool? result = transactionEditWindow.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }

        public void OpenTransactionListView()
        {
            ITransactionListViewModel viewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<ITransactionListViewModel>();
            var window = new TransactionListWindow(viewModel);
            window.ShowDialog();
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
