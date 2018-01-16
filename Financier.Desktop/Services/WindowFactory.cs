using Financier.Data;
using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            IAccountListViewModel accountListViewModel = new AccountListViewModel(dbContext, accountBalanceService);
            ITransactionListViewModel transactionListViewModel = new TransactionListViewModel(dbContext);
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(accountListViewModel, transactionListViewModel);
            Window mainWindow = new MainWindow(mainWindowLogger, mainWindowViewModel);

            return mainWindow;
        }

        public Window CreateAccountCreateWindow()
        {
            FinancierDbContext dbContext = IoC.ServiceProvider.Instance.GetRequiredService<FinancierDbContext>();
            IAccountCreateViewModel accountCreateViewModel = new AccountCreateViewModel(dbContext);
            Window transactionEditWindow = new AccountCreateWindow(accountCreateViewModel);

            return transactionEditWindow;
        }

        public Window CreateTransactionCreateWindow()
        {
            FinancierDbContext dbContext = IoC.ServiceProvider.Instance.GetRequiredService<FinancierDbContext>();
            ITransactionEditViewModel transactionEditViewModel = new TransactionEditViewModel(dbContext);
            Window transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);

            return transactionEditWindow;
        }

        public Window CreateTransactionEditWindow(int transactionId)
        {
            FinancierDbContext dbContext = IoC.ServiceProvider.Instance.GetRequiredService<FinancierDbContext>();
            ITransactionEditViewModel transactionEditViewModel = new TransactionEditViewModel(dbContext, transactionId);
            Window transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);

            return transactionEditWindow;
        }
    }
}
