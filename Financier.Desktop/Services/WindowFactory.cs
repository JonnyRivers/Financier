using Financier.Data;
using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace Financier.Desktop.Services
{
    public class WindowFactory : IWindowFactory
    {
        public Window CreateMainWindow()
        {
            ILogger<MainWindow> mainWindowLogger = IoC.ServiceProvider.Instance.GetRequiredService<ILogger<MainWindow>>();
            FinancierDbContext dbContext = IoC.ServiceProvider.Instance.GetRequiredService<FinancierDbContext>();
            IAccountBalanceService accountBalanceService = IoC.ServiceProvider.Instance.GetRequiredService<IAccountBalanceService>();
            var accountListViewModel = new AccountListViewModel(dbContext, accountBalanceService);
            var transactionListViewModel = new TransactionListViewModel(dbContext);
            var mainWindowViewModel = new MainWindowViewModel(accountListViewModel, transactionListViewModel);
            var mainWindow = new MainWindow(mainWindowLogger, mainWindowViewModel);

            return mainWindow;
        }

        public Window CreateAccountCreateWindow()
        {
            FinancierDbContext dbContext = IoC.ServiceProvider.Instance.GetRequiredService<FinancierDbContext>();
            var accountCreateViewModel = new AccountCreateViewModel(dbContext);
            var transactionEditWindow = new AccountCreateWindow(accountCreateViewModel);

            return transactionEditWindow;
        }

        public Window CreateAccountEditWindow(int accountId)
        {
            FinancierDbContext dbContext = IoC.ServiceProvider.Instance.GetRequiredService<FinancierDbContext>();
            var accountOverviewViewModel = new AccountOverviewViewModel(dbContext, accountId);
            var accountEditViewModel = new AccountEditViewModel(accountOverviewViewModel);
            var accountEditWindow = new AccountEditWindow(accountEditViewModel);

            return accountEditWindow;
        }

        public Window CreateTransactionCreateWindow()
        {
            FinancierDbContext dbContext = IoC.ServiceProvider.Instance.GetRequiredService<FinancierDbContext>();
            var transactionEditViewModel = new TransactionEditViewModel(dbContext);
            var transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);

            return transactionEditWindow;
        }

        public Window CreateTransactionEditWindow(int transactionId)
        {
            FinancierDbContext dbContext = IoC.ServiceProvider.Instance.GetRequiredService<FinancierDbContext>();
            var transactionEditViewModel = new TransactionEditViewModel(dbContext, transactionId);
            var transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);

            return transactionEditWindow;
        }
    }
}
