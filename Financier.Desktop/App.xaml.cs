using Financier.Data;
using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace Financier.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IServiceProvider serviceProvider = BuildServiceProvider();
            Window mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            // Framework services
            ILoggerFactory loggerFactory = new LoggerFactory().AddDebug();
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            string connectionString =
                @"Server=(localdb)\mssqllocaldb;" +
                $"Database=Financier_FamilyFortunesTest;" +
                "Trusted_Connection=True;";
            serviceCollection.AddDbContext<FinancierDbContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            // View models
            serviceCollection.AddTransient<ITransactionsViewModel, TransactionsViewModel>();
            serviceCollection.AddTransient<IMainWindowViewModel, MainWindowViewModel>();

            // Views
            serviceCollection.AddTransient<TransactionsControl>();
            serviceCollection.AddTransient<MainWindow>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
