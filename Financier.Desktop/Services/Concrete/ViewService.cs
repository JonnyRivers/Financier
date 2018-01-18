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
            accountEditViewModel.AccountId = accountId;// TODO: can we omit this initialization step?
            var accountEditWindow = new AccountEditWindow(accountEditViewModel);
            bool? result = accountEditWindow.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
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
            MessageBoxResult confirmResult = MessageBox.Show(
               "Are you sure you want to delete this transaction?  This cannot be undone.",
               "Really delete transaction?",
               MessageBoxButton.YesNo
           );

            return (confirmResult == MessageBoxResult.Yes);
        }

        public bool OpenTransactionEditView(int transactionId)
        {
            ITransactionEditViewModel transactionEditViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<ITransactionEditViewModel>();
            transactionEditViewModel.TransactionId = transactionId;// TODO: can we omit this initialization step?
            var transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);
            bool? result = transactionEditWindow.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }
    }
}
