using System;
using Financier.Services;
using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Financier.UnitTesting;
using Financier.UnitTesting.DbSetup;
using Financier.UnitTesting.Moq;
using Financier.Desktop.Tests.Concrete;

namespace Financier.Desktop.Tests
{
    [TestClass]
    public class AccountTransactionListViewModelTests
    {
        [TestMethod]
        public void TestAccountTransactionListViewModelNoTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountTransactionListViewModel> logger = loggerFactory.CreateLogger<AccountTransactionListViewModel>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity = accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity = accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(), 
                    sqliteMemoryWrapper.DbContext);

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext);

                IAccountTransactionItemViewModelFactory accountTransactionViewModelFactory = 
                    new Concrete.StubAccountTransactionItemViewModelFactory();

                var viewService = new Concrete.FakeViewService();

                var viewModel = new AccountTransactionListViewModel(
                    logger,
                    accountService,
                    transactionService,
                    accountTransactionViewModelFactory,
                    new Mock<IDeleteConfirmationViewService>().Object,
                    new Mock<ITransactionCreateViewService>().Object,
                    new Mock<ITransactionEditViewService>().Object,
                    viewService,
                    checkingAccountEntity.AccountId);

                Assert.AreEqual(false, viewModel.HasLogicalAcounts);
                Assert.AreEqual(0, viewModel.Transactions.Count);
            }
        }

        [TestMethod]
        public void TestAccountTransactionListViewModelAccountWithLogicalsAndTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountTransactionListViewModel> logger = loggerFactory.CreateLogger<AccountTransactionListViewModel>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 60m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 8, 32, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var checkingToGroceriesPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = groceriesPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext);

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext);


                IAccountTransactionItemViewModelFactory accountTransactionViewModelFactory =
                    new Concrete.StubAccountTransactionItemViewModelFactory();

                var viewService = new Concrete.FakeViewService();

                var viewModel = new AccountTransactionListViewModel(
                    logger,
                    accountService,
                    transactionService,
                    accountTransactionViewModelFactory,
                    new Mock<IDeleteConfirmationViewService>().Object,
                    new Mock<ITransactionCreateViewService>().Object,
                    new Mock<ITransactionEditViewService>().Object,
                    viewService,
                    checkingAccountEntity.AccountId);

                Assert.AreEqual(true, viewModel.HasLogicalAcounts);
                Assert.AreEqual(3, viewModel.Transactions.Count);
                Assert.AreEqual(100m, viewModel.Transactions[0].Balance);
                Assert.AreEqual(100m, viewModel.Transactions[1].Balance);
                Assert.AreEqual(100m, viewModel.Transactions[2].Balance);
            }
        }

        [TestMethod]
        public void TestAccountTransactionListViewModelPostDeleteBalanceRefresh()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountTransactionListViewModel> logger = loggerFactory.CreateLogger<AccountTransactionListViewModel>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);

                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentExpenseAccountEntity,
                        Amount = 60m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentExpenseAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 8, 32, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext);

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext);

                IAccountTransactionItemViewModelFactory accountTransactionViewModelFactory =
                    new Concrete.StubAccountTransactionItemViewModelFactory();

                // Force transaction deletion to always confirm
                var mockDeleteConfirmationViewService = new Mock<IDeleteConfirmationViewService>();
                mockDeleteConfirmationViewService
                    .Setup(s => s.Show(It.IsAny<string>()))
                    .Returns(true);

                var viewModel = new AccountTransactionListViewModel(
                    logger,
                    accountService,
                    transactionService,
                    accountTransactionViewModelFactory,
                    mockDeleteConfirmationViewService.Object,
                    new Mock<ITransactionCreateViewService>().Object,
                    new Mock<ITransactionEditViewService>().Object,
                    new Mock<IViewService>().Object,
                    checkingAccountEntity.AccountId
                );

                // Delete the first transaction
                viewModel.SelectedTransaction = viewModel.Transactions.OrderBy(t => t.At).First();
                viewModel.DeleteCommand.Execute(this);

                var transactionViewModels = new List<IAccountTransactionItemViewModel>(viewModel.Transactions.OrderBy(t => t.At));

                Assert.AreEqual(2, transactionViewModels.Count);
                Assert.AreEqual(transactionEntities[1].CreditAccount.AccountId, transactionViewModels[0].CreditAccount.AccountId);
                Assert.AreEqual(transactionEntities[1].DebitAccount.AccountId, transactionViewModels[0].DebitAccount.AccountId);
                Assert.AreEqual(transactionEntities[1].Amount, transactionViewModels[0].Amount);
                Assert.AreEqual(transactionEntities[1].At, transactionViewModels[0].At);
                Assert.AreEqual(-60m, transactionViewModels[0].Balance);
                Assert.AreEqual(transactionEntities[2].CreditAccount.AccountId, transactionViewModels[1].CreditAccount.AccountId);
                Assert.AreEqual(transactionEntities[2].DebitAccount.AccountId, transactionViewModels[1].DebitAccount.AccountId);
                Assert.AreEqual(transactionEntities[2].Amount, transactionViewModels[1].Amount);
                Assert.AreEqual(transactionEntities[2].At, transactionViewModels[1].At);
                Assert.AreEqual(-70m, transactionViewModels[1].Balance);
            }
        }
    }
}
