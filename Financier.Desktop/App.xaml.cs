using Financier.Desktop.Services;
using Financier.Entities;
using Financier.Services;
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
        private IServiceProvider m_serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += App_DispatcherUnhandledException;

            ServiceCollection serviceCollection = BuildConnectionServiceProvider();
            IDatabaseConnectionViewService connectionViewService = m_serviceProvider.GetRequiredService<IDatabaseConnectionViewService>();
            DatabaseConnection databaseConnection;
            if(connectionViewService.OpenDatabaseConnectionListView(out databaseConnection))
            {
                BuildFullServiceProvider(serviceCollection, databaseConnection);
                IViewService viewService = m_serviceProvider.GetRequiredService<IViewService>();
                viewService.OpenMainView();
            }
        }

        private void App_DispatcherUnhandledException(
            object sender, 
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if(m_serviceProvider != null)
            {
                IViewService viewService = m_serviceProvider.GetRequiredService<IViewService>();

                viewService.OpenUnhandledExceptionView(e.Exception);
            }
            else
            {
                MessageBox.Show(e.Exception.ToString(), "Unhandled exception");
            }

            e.Handled = true;
        }

        private ServiceCollection BuildConnectionServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            // Framework services
            string localApplicationDataDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            ILoggerFactory loggerFactory =
                new LoggerFactory()
                    .AddDebug()
                    .AddFile($"{localApplicationDataDirectory}/Financier.Desktop/{{Date}}.txt", LogLevel.Trace);
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            // Financier.Core services
            serviceCollection.AddSingleton<IDatabaseConnectionService, LocalDatabaseConnectionService>();

            // Financier.Desktop services
            serviceCollection.AddSingleton<IMessageService, MessageService>();// unused
            serviceCollection.AddSingleton<IDatabaseConnectionViewModelFactory, DatabaseConnectionViewModelFactory>();
            serviceCollection.AddSingleton<IDatabaseConnectionViewService, DatabaseConnectionViewService>();

            // This exposes a major flaw.  We need IViewService tobe registered for exception handling, 
            // but as the ViewModelFactory takes an IServiceProvider, there will be missing depedencies
            // at this point.  Also, any missing dependencies are encountered very late.
            serviceCollection.AddSingleton<IViewModelFactory, ViewModelFactory>();
            serviceCollection.AddSingleton<IViewService, ViewService>();

            m_serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceCollection;
        }

        private void BuildFullServiceProvider(ServiceCollection serviceCollection, DatabaseConnection databaseConnection)
        {
            // TODO - we are assuming SqlServer
            string connectionString = databaseConnection.BuildConnectionString();
            if (databaseConnection.Type == DatabaseConnectionType.SqlServer)
            {
                serviceCollection.AddDbContext<FinancierDbContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);
            }
            else if (databaseConnection.Type == DatabaseConnectionType.SqlLiteFile)
            {
                throw new NotImplementedException();
            }

            // Financier.Core services
            serviceCollection.AddSingleton<IAccountService, AccountService>();
            serviceCollection.AddSingleton<IAccountRelationshipService, AccountRelationshipService>();
            serviceCollection.AddSingleton<IBalanceSheetService, BalanceSheetService>();
            serviceCollection.AddSingleton<IBudgetService, BudgetService>();
            serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
            serviceCollection.AddSingleton<ICurrencyExchangeService, FixerIOCurrencyExchangeService>();
            serviceCollection.AddSingleton<IEnvironmentService, EnvironmentService>();
            serviceCollection.AddSingleton<IHttpClientFactory, HttpClientFactory>();
            serviceCollection.AddSingleton<ITransactionService, TransactionService>();
            serviceCollection.AddSingleton<ITransactionRelationshipService, TransactionRelationshipService>();

            m_serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
