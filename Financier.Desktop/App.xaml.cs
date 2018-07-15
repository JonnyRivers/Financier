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

            m_serviceProvider = Build();
            IViewService viewService = m_serviceProvider.GetRequiredService<IViewService>();
            viewService.OpenMainView();
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

        private IServiceProvider Build()
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

            // We have to build a temporary service provider to get the connection string for the DbContext.
            // Perhaps there is a better way.
            serviceCollection.AddSingleton<IEnvironmentService, EnvironmentService>();
            IEnvironmentService environmentService = 
                serviceCollection.BuildServiceProvider().GetRequiredService<IEnvironmentService>();
            string connectionString = environmentService.GetConnectionString();

            serviceCollection.AddDbContext<FinancierDbContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            // Financier.Core services
            serviceCollection.AddSingleton<IAccountService, AccountService>();
            serviceCollection.AddSingleton<IAccountRelationshipService, AccountRelationshipService>();
            serviceCollection.AddSingleton<IBudgetService, BudgetService>();
            serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
            serviceCollection.AddSingleton<ICurrencyExchangeService, FixerIOCurrencyExchangeService>();
            serviceCollection.AddSingleton<IHttpClientFactory, HttpClientFactory>();
            serviceCollection.AddSingleton<ITransactionService, TransactionService>();
            serviceCollection.AddSingleton<ITransactionRelationshipService, TransactionRelationshipService>();
            
            // Financier.Desktop services
            serviceCollection.AddSingleton<IMessageService, MessageService>();
            serviceCollection.AddSingleton<IViewModelFactory, ViewModelFactory>();
            serviceCollection.AddSingleton<IViewService, ViewService>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
