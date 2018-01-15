using Financier.Data;
using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
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
            ITransactionListViewModel transactionListViewModel = new TransactionListViewModel(dbContext);
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(transactionListViewModel);
            Window mainWindow = new MainWindow(mainWindowLogger, mainWindowViewModel);

            return mainWindow;
        }

        public Window CreateTransactionEditWindow(ITransactionItemViewModel itemViewModel)
        {
            ITransactionEditViewModel transactionEditViewModel = new TransactionEditViewModel(itemViewModel);
            Window transactionEditWindow = new TransactionEditWindow(transactionEditViewModel);

            return transactionEditWindow;
        }
    }
}
