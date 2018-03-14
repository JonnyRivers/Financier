﻿using Financier.Desktop.Services;
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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IServiceProvider serviceProvider = Build();
            IViewService viewService = serviceProvider.GetRequiredService<IViewService>();
            viewService.OpenMainView();
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

            // TODO: how can we use a service that is required for DbContext creation?
            IEnvironmentService environmentService = new EnvironmentService();
            string connectionString = environmentService.GetConnectionString();

            serviceCollection.AddDbContext<FinancierDbContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            serviceCollection.AddSingleton<IAccountService, AccountService>();
            serviceCollection.AddSingleton<IAccountRelationshipService, AccountRelationshipService>();
            serviceCollection.AddSingleton<IBudgetService, BudgetService>();
            serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
            serviceCollection.AddSingleton<ICurrencyExchangeService, FixerIOCurrencyExchangeService>();
            serviceCollection.AddSingleton<IEnvironmentService, EnvironmentService>();
            serviceCollection.AddSingleton<IHttpClientFactory, HttpClientFactory>();
            serviceCollection.AddSingleton<ITransactionService, TransactionService>();
            serviceCollection.AddSingleton<ITransactionRelationshipService, TransactionRelationshipService>();
            
            serviceCollection.AddSingleton<IMessageService, MessageService>();
            serviceCollection.AddSingleton<IViewModelFactory, ViewModelFactory>();
            serviceCollection.AddSingleton<IViewService, ViewService>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
