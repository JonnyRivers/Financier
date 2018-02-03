using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class ViewModelFactoryTests
    {
        [TestMethod]
        public void TestAllViewModelCreation()
        {
            IServiceProvider serviceProvider = BuildServiceProvider();
            IViewModelFactory viewModelFactory = serviceProvider.GetRequiredService<IViewModelFactory>();

            Entities.FinancierDbContext dbContext = 
                serviceProvider.GetRequiredService<Entities.FinancierDbContext>();
            dbContext.Database.EnsureCreated();

            var currencyFactory = new DbSetup.CurrencyFactory();
            Entities.Currency currencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
            currencyFactory.Add(dbContext, currencyEntity);

            var accountFactory = new DbSetup.AccountFactory();
            Entities.Account checkingAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Checking, currencyEntity);
            accountFactory.Add(dbContext, checkingAccountEntity);

            IAccountService accountService = serviceProvider.GetRequiredService<IAccountService>();
            Account checkingAccount = accountService.Get(checkingAccountEntity.AccountId);
            AccountLink checkingAccountLink = accountService.GetAsLink(checkingAccountEntity.AccountId);

            IAccountEditViewModel accountEditViewModel = 
                viewModelFactory.CreateAccountEditViewModel(checkingAccountEntity.AccountId);

            IAccountItemViewModel accountItemViewModel =
                viewModelFactory.CreateAccountItemViewModel(checkingAccount);

            IAccountLinkViewModel accountLinkViewModel =
                viewModelFactory.CreateAccountLinkViewModel(checkingAccountLink);

            IAccountListViewModel accountListViewModel =
                viewModelFactory.CreateAccountListViewModel();

            Assert.AreEqual(checkingAccountEntity.AccountId, accountEditViewModel.AccountId);
            Assert.AreEqual(checkingAccountEntity.AccountId, accountItemViewModel.AccountId);
            Assert.AreEqual(checkingAccountEntity.AccountId, accountLinkViewModel.AccountId);
            Assert.AreEqual(dbContext.Accounts.Count(), accountListViewModel.Accounts.Count);
        }

        private IServiceProvider BuildServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            // Framework services
            ILoggerFactory loggerFactory = new LoggerFactory();
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            var dbConnection = new SqliteConnection("DataSource=:memory:");
            dbConnection.Open();
            serviceCollection.AddDbContext<Entities.FinancierDbContext>(
                options => options.UseSqlite(dbConnection),
                ServiceLifetime.Transient);

            serviceCollection.AddSingleton<IAccountService, AccountService>();
            serviceCollection.AddSingleton<IAccountRelationshipService, AccountRelationshipService>();
            serviceCollection.AddSingleton<IBudgetService, BudgetService>();
            serviceCollection.AddSingleton<ICurrencyService, CurrencyService>();
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
