using Financier.Desktop.Services;
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
                IViewService viewService = m_serviceProvider.GetRequiredService<IViewService>();
                this.ShutdownMode = ShutdownMode.OnLastWindowClose;
                viewService.OpenMainView();
            }
        }

        private void App_DispatcherUnhandledException(
            object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (m_serviceProvider != null)
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
            serviceCollection.AddSingleton<IMessageService, MessageService>();// unused
            serviceCollection.AddSingleton<IDatabaseConnectionViewModelFactory, DatabaseConnectionViewModelFactory>();
            serviceCollection.AddSingleton<IDatabaseConnectionViewService, DatabaseConnectionViewService>();

            // This exposes a major flaw.  We need IViewService to be registered for exception handling, 
            // but as the ViewModelFactory takes an IServiceProvider, there will be missing depedencies
            // at this point.  Also, any missing dependencies are encountered very late.
            serviceCollection.AddSingleton<IViewModelFactory, ViewModelFactory>();
            serviceCollection.AddSingleton<IViewService, ViewService>();

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
