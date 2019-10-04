using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Financier.UnitTesting;
using Financier.UnitTesting.DbSetup;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Tests
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

            var currencyFactory = new CurrencyFactory();
            Entities.Currency currencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
            currencyFactory.Add(dbContext, currencyEntity);

            var accountFactory = new AccountFactory();
            Entities.Account checkingAccountEntity = accountFactory.Create(AccountPrefab.Checking, currencyEntity);
            accountFactory.Add(dbContext, checkingAccountEntity);

            IAccountService accountService = serviceProvider.GetRequiredService<IAccountService>();
            Account checkingAccount = accountService.Get(checkingAccountEntity.AccountId);
            AccountLink checkingAccountLink = accountService.GetAsLink(checkingAccountEntity.AccountId);

            IAccountDetailsViewModel accountDetailsViewModel = 
                viewModelFactory.CreateAccountEditViewModel(checkingAccountEntity.AccountId);

            IAccountItemViewModel accountItemViewModel =
                viewModelFactory.CreateAccountItemViewModel(checkingAccount);

            IAccountLinkViewModel accountLinkViewModel =
                viewModelFactory.CreateAccountLinkViewModel(checkingAccountLink);

            IAccountListViewModel accountListViewModel =
                viewModelFactory.CreateAccountListViewModel();

            Assert.IsNotNull(accountDetailsViewModel);
            Assert.AreEqual(checkingAccountEntity.AccountId, accountDetailsViewModel.ToAccount().AccountId);

            Assert.IsNotNull(accountItemViewModel);
            Assert.AreEqual(checkingAccountEntity.AccountId, accountItemViewModel.AccountId);

            Assert.IsNotNull(accountLinkViewModel);
            Assert.AreEqual(checkingAccountEntity.AccountId, accountLinkViewModel.AccountId);

            Assert.IsNotNull(accountListViewModel);
            Assert.AreEqual(dbContext.Accounts.Count(), accountListViewModel.Accounts.Count);
        }

        [TestMethod]
        public void TestBudgetViewModelCreation()
        {
            IServiceProvider serviceProvider = BuildServiceProvider();
            IViewModelFactory viewModelFactory = serviceProvider.GetRequiredService<IViewModelFactory>();

            Entities.FinancierDbContext dbContext =
                serviceProvider.GetRequiredService<Entities.FinancierDbContext>();
            dbContext.Database.EnsureCreated();

            var currencyFactory = new CurrencyFactory();
            Entities.Currency usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
            currencyFactory.Add(dbContext, usdCurrencyEntity);

            var accountFactory = new AccountFactory();
            Entities.Account incomeAccountEntity = accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
            accountFactory.Add(dbContext, incomeAccountEntity);
            Entities.Account checkingAccountEntity = accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
            accountFactory.Add(dbContext, checkingAccountEntity);
            Entities.Account rentPrepaymentAccountEntity = accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
            accountFactory.Add(dbContext, rentPrepaymentAccountEntity);
            Entities.Account savingsAccountEntity = accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
            accountFactory.Add(dbContext, savingsAccountEntity);

            var budgetEntity = new Entities.Budget
            {
                Name = "Budget",
                Period = BudgetPeriod.Fortnightly
            };
            dbContext.Budgets.Add(budgetEntity);
            dbContext.SaveChanges();

            var initialTransactionEntity = new Entities.BudgetTransaction
            {
                CreditAccount = incomeAccountEntity,
                DebitAccount = checkingAccountEntity,
                Amount = 200m,
                IsInitial = true,
                Budget = budgetEntity
            };
            var rentTransactionEntity = new Entities.BudgetTransaction
            {
                CreditAccount = checkingAccountEntity,
                DebitAccount = rentPrepaymentAccountEntity,
                Amount = 100m,
                Budget = budgetEntity
            };
            var surplusTransactionEntity = new Entities.BudgetTransaction
            {
                CreditAccount = checkingAccountEntity,
                DebitAccount = savingsAccountEntity,
                IsSurplus = true,
                Budget = budgetEntity
            };
            dbContext.BudgetTransactions.Add(initialTransactionEntity);
            dbContext.BudgetTransactions.Add(rentTransactionEntity);
            dbContext.BudgetTransactions.Add(surplusTransactionEntity);
            dbContext.SaveChanges();

            IAccountService accountService = serviceProvider.GetRequiredService<IAccountService>();
            IEnumerable<AccountLink> accountLinks = accountService.GetAllAsLinks();
            IEnumerable<IAccountLinkViewModel> accountLinkViewModels = 
                accountLinks
                    .Select(al => viewModelFactory.CreateAccountLinkViewModel(al));

            IBudgetService budgetService = serviceProvider.GetRequiredService<IBudgetService>();
            Budget budget = budgetService.Get(budgetEntity.BudgetId);

            ICurrencyService currencyService = serviceProvider.GetRequiredService<ICurrencyService>();
            Currency primaryCurrency = currencyService.GetPrimary();

            IBudgetDetailsViewModel budgetDetailsViewModel =
                viewModelFactory.CreateBudgetEditViewModel(budgetEntity.BudgetId);

            IBudgetItemViewModel budgetItemViewModel = 
                viewModelFactory.CreateBudgetItemViewModel(budget, primaryCurrency);

            IBudgetListViewModel budgetListViewModel = 
                viewModelFactory.CreateBudgetListViewModel();

            IBudgetTransactionItemViewModel budgetTransactionItemViewModel = 
                viewModelFactory.CreateBudgetTransactionItemViewModel(
                    new ObservableCollection<IAccountLinkViewModel>(accountLinkViewModels),
                    budget.InitialTransaction, 
                    BudgetTransactionType.Initial
            );

            IBudgetTransactionListViewModel budgetTransactionListViewModel = 
                viewModelFactory.CreateBudgetTransactionListViewModel(budgetEntity.BudgetId);

            IPaydayEventStartViewModel paydayEventStartViewModel = 
                viewModelFactory.CreatePaydayEventStartViewModel(budgetEntity.BudgetId);

            Assert.IsNotNull(budgetDetailsViewModel);

            Assert.IsNotNull(budgetItemViewModel);

            Assert.IsNotNull(budgetListViewModel);

            Assert.IsNotNull(budgetTransactionItemViewModel);

            Assert.IsNotNull(budgetTransactionListViewModel);

            Assert.IsNotNull(paydayEventStartViewModel);
        }

        [TestMethod]
        public void TestTransactionViewModelCreation()
        {
            IServiceProvider serviceProvider = BuildServiceProvider();
            IViewModelFactory viewModelFactory = serviceProvider.GetRequiredService<IViewModelFactory>();

            Entities.FinancierDbContext dbContext =
                serviceProvider.GetRequiredService<Entities.FinancierDbContext>();
            dbContext.Database.EnsureCreated();

            var currencyFactory = new CurrencyFactory();
            Entities.Currency usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
            currencyFactory.Add(dbContext, usdCurrencyEntity);

            var accountFactory = new AccountFactory();
            Entities.Account incomeAccountEntity = accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
            accountFactory.Add(dbContext, incomeAccountEntity);
            Entities.Account checkingAccountEntity = accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
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
            ITransactionDetailsViewModel transactionCreateViewModel = 
                viewModelFactory.CreateTransactionCreateViewModel(hint);

            int transactionId = firstTransaction.TransactionId;
            ITransactionDetailsViewModel transactionEditViewModel = 
                viewModelFactory.CreateTransactionEditViewModel(transactionId);

            Transaction transaction = firstTransaction;
            ITransactionItemViewModel transactionItemViewModel = 
                viewModelFactory.CreateTransactionItemViewModel(transaction);

            ITransactionListViewModel transactionListViewModel = 
                viewModelFactory.CreateTransactionListViewModel();

            IAccountTransactionListViewModel accountTransactionListViewModel =
                viewModelFactory.CreateAccountTransactionListViewModel(checkingAccountEntity.AccountId);

            IAccountTransactionItemViewModel accountTransactionItemViewModel = 
                viewModelFactory.CreateAccountTransactionItemViewModel(transaction);

            Assert.IsNotNull(transactionBatchCreateConfirmViewModel);

            Assert.IsNotNull(transactionCreateViewModel);

            Assert.IsNotNull(transactionEditViewModel);

            Assert.IsNotNull(transactionItemViewModel);

            Assert.IsNotNull(transactionListViewModel);

            Assert.IsNotNull(accountTransactionListViewModel);
            Assert.AreEqual(dbContext.Transactions.Count(), accountTransactionListViewModel.Transactions.Count);

            Assert.IsNotNull(accountTransactionItemViewModel);
            Assert.AreEqual(transaction.Amount, accountTransactionItemViewModel.Amount);
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
            serviceCollection.AddSingleton<IEnvironmentService, EnvironmentService>();
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
