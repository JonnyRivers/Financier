using Financier.Entities;
using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.IoC
{
    public static class ServiceProvider
    {
        private static IServiceProvider m_instance;

        public static IServiceProvider Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = Build();

                return m_instance;
            }
        }

        private static IServiceProvider Build()
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

            serviceCollection.AddSingleton<IAccountService, AccountService>();
            serviceCollection.AddSingleton<IAccountRelationshipService, AccountRelationshipService>();
            serviceCollection.AddSingleton<IBudgetService, BudgetService>();
            serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
            serviceCollection.AddSingleton<ITransactionService, TransactionService>();

            serviceCollection.AddSingleton<IViewService, ViewService>();

            // TODO: For consistency, all view models should be constructed with IoC
            // https://github.com/JonnyRivers/Financier/issues/15
            serviceCollection.AddTransient<IAccountEditViewModel, AccountEditViewModel>();
            serviceCollection.AddTransient<IAccountLinkViewModel, AccountLinkViewModel>();
            serviceCollection.AddTransient<IAccountListViewModel, AccountListViewModel>();
            serviceCollection.AddTransient<IBudgetEditViewModel, BudgetEditViewModel>();
            serviceCollection.AddTransient<IBudgetListViewModel, BudgetListViewModel>();
            serviceCollection.AddTransient<IBudgetTransactionListViewModel, BudgetTransactionListViewModel>();
            serviceCollection.AddTransient<IBudgetTransactionItemViewModel, BudgetTransactionItemViewModel>();
            serviceCollection.AddTransient<ITransactionEditViewModel, TransactionEditViewModel>();
            serviceCollection.AddTransient<ITransactionListViewModel, TransactionListViewModel>();
            serviceCollection.AddTransient<IMainWindowViewModel, MainWindowViewModel>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
