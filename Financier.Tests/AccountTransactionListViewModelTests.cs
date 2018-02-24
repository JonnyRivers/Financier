using System;
using Financier.Services;
using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Tests.Moq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace Financier.Tests
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
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(), 
                    sqliteMemoryWrapper.DbContext);

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext);

                var viewModelFactory = new Concrete.FakeViewModelFactory();

                var viewService = new Concrete.FakeViewService();

                var viewModel = new AccountTransactionListViewModel(
                    logger,
                    accountService,
                    transactionService,
                    viewModelFactory,
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
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
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

                var viewModelFactory = new Concrete.FakeViewModelFactory();

                var viewService = new Concrete.FakeViewService();

                var viewModel = new AccountTransactionListViewModel(
                    logger,
                    accountService,
                    transactionService,
                    viewModelFactory,
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
        public void TestAccountTransactionListViewModelPostCreateBalanceRefresh()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountTransactionListViewModel> logger = loggerFactory.CreateLogger<AccountTransactionListViewModel>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentExpense, usdCurrencyEntity);
                
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

                var viewModelFactory = new Concrete.FakeViewModelFactory();

                AccountLink checkingAccountLink = accountService.GetAsLink(checkingAccountEntity.AccountId);
                AccountLink rentExpenseAccountLink = accountService.GetAsLink(rentExpenseAccountEntity.AccountId);
                var newTransactionAmount = 18m;
                var newTransactionAt = new DateTime(2018, 1, 1, 8, 33, 0);

                var mockViewService = new Mock<IViewService>();
                Transaction newTransaction;
                mockViewService
                    .Setup(viewService => viewService.OpenTransactionCreateView(
                        It.IsAny<Transaction>(),
                        out newTransaction))
                    .OutCallback((Transaction hint, out Transaction t) => 
                    {
                        t = new Transaction
                        {
                            CreditAccount = checkingAccountLink,
                            DebitAccount = rentExpenseAccountLink,
                            Amount = newTransactionAmount,
                            At = newTransactionAt
                        };

                        transactionService.Create(t);
                    })
                    .Returns(true);
                
                var viewModel = new AccountTransactionListViewModel(
                    logger,
                    accountService,
                    transactionService,
                    viewModelFactory,
                    mockViewService.Object,
                    checkingAccountEntity.AccountId
                );
                viewModel.CreateCommand.Execute(this);

                var transactionViewModels = new List<IAccountTransactionItemViewModel>(viewModel.Transactions.OrderBy(t => t.At));

                Assert.AreEqual(4, transactionViewModels.Count);
                Assert.AreEqual(100m, transactionViewModels[0].Balance);
                Assert.AreEqual(40m, transactionViewModels[1].Balance);
                Assert.AreEqual(30m, transactionViewModels[2].Balance);
                Assert.AreEqual(checkingAccountLink.AccountId, transactionViewModels[3].CreditAccount.AccountId);
                Assert.AreEqual(rentExpenseAccountLink.AccountId, transactionViewModels[3].DebitAccount.AccountId);
                Assert.AreEqual(newTransactionAmount, transactionViewModels[3].Amount);
                Assert.AreEqual(new DateTime(2018, 1, 1, 8, 33, 0), transactionViewModels[3].At);
                Assert.AreEqual(12m, transactionViewModels[3].Balance);
            }
        }
    }
}
