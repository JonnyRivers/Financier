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

            serviceCollection.AddSingleton<IConversionService, ConversionService>();
            serviceCollection.AddSingleton<IMessageService, MessageService>();
            serviceCollection.AddSingleton<IViewService, ViewService>();

            serviceCollection.AddTransient<IAccountEditViewModel, AccountEditViewModel>();
            serviceCollection.AddTransient<IAccountItemViewModel, AccountItemViewModel>();
            serviceCollection.AddTransient<IAccountLinkViewModel, AccountLinkViewModel>();
            serviceCollection.AddTransient<IAccountListViewModel, AccountListViewModel>();
            serviceCollection.AddTransient<IBudgetEditViewModel, BudgetEditViewModel>();
            serviceCollection.AddTransient<IBudgetItemViewModel, BudgetItemViewModel>();
            serviceCollection.AddTransient<IBudgetListViewModel, BudgetListViewModel>();
            serviceCollection.AddTransient<IBudgetTransactionItemViewModel, BudgetTransactionItemViewModel>();
            serviceCollection.AddTransient<IBudgetTransactionListViewModel, BudgetTransactionListViewModel>();
            serviceCollection.AddTransient<IPaydayEventStartViewModel, PaydayEventStartViewModel>();
            serviceCollection.AddTransient<ITransactionEditViewModel, TransactionEditViewModel>();
            serviceCollection.AddTransient<ITransactionItemViewModel, TransactionItemViewModel>();
            serviceCollection.AddTransient<ITransactionListViewModel, TransactionListViewModel>();
            serviceCollection.AddTransient<IMainWindowViewModel, MainWindowViewModel>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
