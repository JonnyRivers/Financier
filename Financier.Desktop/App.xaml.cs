using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Entities;
using Financier.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
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

            // TODO - this is a bit goofy
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            ServiceCollection serviceCollection = BuildConnectionServiceProvider();
            IDatabaseConnectionViewService connectionViewService = m_serviceProvider.GetRequiredService<IDatabaseConnectionViewService>();
            DatabaseConnection databaseConnection;
            string password;
            if (connectionViewService.OpenDatabaseConnectionListView(out databaseConnection, out password))
            {
                BuildFullServiceProvider(serviceCollection, databaseConnection, password);
                IMainViewService mainViewService = m_serviceProvider.GetRequiredService<IMainViewService>();
                this.ShutdownMode = ShutdownMode.OnLastWindowClose;
                mainViewService.Show();
            }
        }

        private void App_DispatcherUnhandledException(
            object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (m_serviceProvider != null)
            {
                IExceptionViewService exceptionViewService = m_serviceProvider.GetRequiredService<IExceptionViewService>();

                exceptionViewService.Show(e.Exception);
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
            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddDebug();

                string localApplicationDataDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var serilogFileLogger = new Serilog.LoggerConfiguration()
                    .WriteTo
                    .File(
                        $"{localApplicationDataDirectory}/Financier.Desktop/Financier.Desktop-.txt",
                        rollingInterval: RollingInterval.Day)
                    .CreateLogger();
                loggingBuilder.AddSerilog(serilogFileLogger);
            });

            // Financier.Core services
            serviceCollection.AddSingleton<IDatabaseConnectionService, LocalDatabaseConnectionService>();

            // Financier.Desktop services
            serviceCollection.AddSingleton<IMessageService, MessageService>();
            serviceCollection.AddSingleton<IDatabaseConnectionViewModelFactory, DatabaseConnectionViewModelFactory>();
            serviceCollection.AddSingleton<IDatabaseConnectionViewService, DatabaseConnectionViewService>();
            serviceCollection.AddSingleton<IExceptionViewModelFactory, ExceptionViewModelFactory>();
            serviceCollection.AddSingleton<IExceptionViewService, ExceptionViewService>();

            m_serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceCollection;
        }

        private void BuildFullServiceProvider(
            ServiceCollection serviceCollection,
            DatabaseConnection databaseConnection,
            string password)
        {
            string connectionString = databaseConnection.BuildConnectionString(password);
            // TODO - verify connection
            if (databaseConnection.Type == DatabaseConnectionType.SqlServerAzure ||
                databaseConnection.Type == DatabaseConnectionType.SqlServerLocalDB)
            {
                serviceCollection.AddDbContext<FinancierDbContext>(
                    options => options.UseSqlServer(connectionString),
                    ServiceLifetime.Transient);
            }
            else
            {
                throw new NotImplementedException();
            }

            // Financier.Desktop services
            serviceCollection.AddSingleton<IMainViewModelFactory, MainViewModelFactory>();
            serviceCollection.AddSingleton<IMainViewService, MainViewService>();

            // TODO - break this up
            serviceCollection.AddSingleton<IViewModelFactory, ViewModelFactory>();
            serviceCollection.AddSingleton<IViewService, ViewService>();

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
