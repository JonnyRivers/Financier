using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class ViewModelFactoryTests
    {
        [TestMethod]
        public void TestAccountViewModelCreation()
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

            Assert.IsNotNull(accountEditViewModel);
            Assert.AreEqual(checkingAccountEntity.AccountId, accountEditViewModel.AccountId);

            Assert.IsNotNull(accountItemViewModel);
            Assert.AreEqual(checkingAccountEntity.AccountId, accountItemViewModel.AccountId);

            Assert.IsNotNull(accountLinkViewModel);
            Assert.AreEqual(checkingAccountEntity.AccountId, accountLinkViewModel.AccountId);

            Assert.IsNotNull(accountListViewModel);
            Assert.AreEqual(dbContext.Accounts.Count(), accountListViewModel.Accounts.Count);
        }

        [TestMethod]
        public void TestTransactionViewModelCreation()
        {
            IServiceProvider serviceProvider = BuildServiceProvider();
            IViewModelFactory viewModelFactory = serviceProvider.GetRequiredService<IViewModelFactory>();

            Entities.FinancierDbContext dbContext =
                serviceProvider.GetRequiredService<Entities.FinancierDbContext>();
            dbContext.Database.EnsureCreated();

            var currencyFactory = new DbSetup.CurrencyFactory();
            Entities.Currency usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
            currencyFactory.Add(dbContext, usdCurrencyEntity);

            var accountFactory = new DbSetup.AccountFactory();
            Entities.Account incomeAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
            accountFactory.Add(dbContext, incomeAccountEntity);
            Entities.Account checkingAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
            accountFactory.Add(dbContext, checkingAccountEntity);

            dbContext.Transactions.Add(
                new Entities.Transaction
                {
                    CreditAccount = incomeAccountEntity,
                    DebitAccount = checkingAccountEntity,
                    Amount = 100,
                    At = new DateTime(2018,1,1)
                }
            );
            dbContext.SaveChanges();

            IAccountService accountService = serviceProvider.GetRequiredService<IAccountService>();
            AccountLink incomeAccountLink = accountService.GetAsLink(incomeAccountEntity.AccountId);
            AccountLink checkingAccountLink = accountService.GetAsLink(checkingAccountEntity.AccountId);

            ITransactionService transactionService = serviceProvider.GetRequiredService<ITransactionService>();
            Transaction firstTransaction = transactionService.GetAll().First();

            IEnumerable<Transaction> transactions = new Transaction[1]
            {
                new Transaction
                {
                    CreditAccount = incomeAccountLink,
                    DebitAccount = checkingAccountLink,
                    Amount = 30m,
                    At = new DateTime(2018, 1, 2)
                }
            };
            ITransactionBatchCreateConfirmViewModel transactionBatchCreateConfirmViewModel = 
                viewModelFactory.CreateTransactionBatchCreateConfirmViewModel(transactions);

            var hint = new Transaction
            {
                CreditAccount = incomeAccountLink,
                DebitAccount = checkingAccountLink,
                Amount = 50m,
                At = new DateTime(2018, 1, 3)
            };
            ITransactionEditViewModel transactionCreateViewModel = 
                viewModelFactory.CreateTransactionCreateViewModel(hint);

            int transactionId = firstTransaction.TransactionId;
            ITransactionEditViewModel transactionEditViewModel = 
                viewModelFactory.CreateTransactionEditViewModel(transactionId);

            Transaction transaction = firstTransaction;
            ITransactionItemViewModel transactionItemViewModel = 
                viewModelFactory.CreateTransactionItemViewModel(transaction);

            ITransactionListViewModel transactionListViewModel = 
                viewModelFactory.CreateTransactionListViewModel();

            Assert.IsNotNull(transactionBatchCreateConfirmViewModel);

            Assert.IsNotNull(transactionCreateViewModel);

            Assert.IsNotNull(transactionEditViewModel);

            Assert.IsNotNull(transactionItemViewModel);

            Assert.IsNotNull(transactionListViewModel);
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
