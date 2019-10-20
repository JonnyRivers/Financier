using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Financier.UnitTesting;
using Financier.UnitTesting.DbSetup;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Tests
{
    [TestClass]
    public class BudgetCreateViewModelTests
    {
        [TestMethod]
        public void TestBudgetCreateViewModelCancel()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var budgetService = new BudgetService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var viewModel = new BudgetCreateViewModel(
                    loggerFactory,
                    budgetService,
                    new Mock<IBudgetTransactionListViewModelFactory>().Object
                );

                viewModel.Name = "My First Budget";
                viewModel.CancelCommand.Execute(this);

                List<Budget> budgets = budgetService.GetAll().ToList();

                Assert.AreEqual(0, budgets.Count);
            }
        }

        [TestMethod]
        public void TestBudgetCreateViewModelOK()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

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
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);

                var accountService = new AccountService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var budgetService = new BudgetService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var mockAccountLinkViewModelFactory = new Mock<IAccountLinkViewModelFactory>();
                mockAccountLinkViewModelFactory
                    .Setup(f => f.Create(It.IsAny<AccountLink>()))
                    .Returns((AccountLink accountLink) =>
                    {
                        return new AccountLinkViewModel(
                            loggerFactory,
                            accountLink);
                    });

                var mockBudgetTransactionItemViewModelFactory = new Mock<IBudgetTransactionItemViewModelFactory>();
                mockBudgetTransactionItemViewModelFactory
                    .Setup(f => f.Create(
                        It.IsAny<ObservableCollection<IAccountLinkViewModel>>(),
                        It.IsAny<BudgetTransaction>(),
                        It.IsAny<BudgetTransactionType>()))
                    .Returns(
                        (ObservableCollection<IAccountLinkViewModel> accountLinks, 
                        BudgetTransaction budgetTransaction, 
                        BudgetTransactionType type) =>
                    {
                        return new BudgetTransactionItemViewModel(
                            loggerFactory,
                            accountLinks,
                            budgetTransaction,
                            type);
                    });

                var mockTransactionListViewModelFactory = new Mock<IBudgetTransactionListViewModelFactory>();
                mockTransactionListViewModelFactory
                    .Setup(f => f.Create(It.IsAny<int>()))
                    .Returns((int budgetId) =>
                    {
                        return new BudgetTransactionListViewModel(
                            loggerFactory,
                            accountService,
                            budgetService,
                            mockAccountLinkViewModelFactory.Object,
                            mockBudgetTransactionItemViewModelFactory.Object,
                            new Mock<IDeleteConfirmationViewService>().Object,
                            budgetId
                        );
                    });

                var viewModel = new BudgetCreateViewModel(
                    loggerFactory,
                    budgetService,
                    mockTransactionListViewModelFactory.Object
                );

                viewModel.Name = "My First Budget";
                viewModel.SelectedPeriod = BudgetPeriod.Monthly;
                viewModel.OKCommand.Execute(this);

                List<Budget> budgets = budgetService.GetAll().ToList();

                Assert.AreEqual(1, budgets.Count);
                Assert.AreEqual(viewModel.Name, budgets[0].Name);
                Assert.AreEqual(viewModel.SelectedPeriod, budgets[0].Period);
                Assert.AreEqual(AccountType.Income, budgets[0].InitialTransaction.CreditAccount.Type);
                Assert.AreEqual(AccountType.Asset, budgets[0].InitialTransaction.DebitAccount.Type);
                Assert.AreEqual(AccountType.Asset, budgets[0].SurplusTransaction.CreditAccount.Type);
                Assert.AreEqual(AccountType.Asset, budgets[0].SurplusTransaction.DebitAccount.Type);
                Assert.AreEqual(0, budgets[0].Transactions.Count());
            }
        }
    }
}
