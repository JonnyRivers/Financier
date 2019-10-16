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
    public class BudgetEditViewModelTests
    {
        [TestMethod]
        public void TestBudgetEditViewModelCancel()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                Entities.Currency usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                Entities.Account groceriesExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesExpenseAccountEntity);

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
                var rentPrepaymentToExpenseRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };
                var groceriesPrepaymentToExpenseRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = groceriesPrepaymentAccountEntity,
                    DestinationAccount = groceriesExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(groceriesPrepaymentToExpenseRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budget = new Entities.Budget
                {
                    Name = "Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budget);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var initialTransaction = new Entities.BudgetTransaction
                {
                    CreditAccount = incomeAccountEntity,
                    DebitAccount = checkingAccountEntity,
                    Amount = 200m,
                    IsInitial = true,
                    Budget = budget
                };
                var rentTransaction = new Entities.BudgetTransaction
                {
                    CreditAccount = checkingAccountEntity,
                    DebitAccount = rentPrepaymentAccountEntity,
                    Amount = 100m,
                    Budget = budget
                };
                var groceriesTransaction = new Entities.BudgetTransaction
                {
                    CreditAccount = checkingAccountEntity,
                    DebitAccount = groceriesPrepaymentAccountEntity,
                    Amount = 50m,
                    Budget = budget
                };
                var surplusTransaction = new Entities.BudgetTransaction
                {
                    CreditAccount = checkingAccountEntity,
                    DebitAccount = groceriesPrepaymentAccountEntity,
                    IsSurplus = true,
                    Budget = budget
                };
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(initialTransaction);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(rentTransaction);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(groceriesTransaction);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(surplusTransaction);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext);

                var budgetService = new BudgetService(
                    loggerFactory.CreateLogger<BudgetService>(),
                    sqliteMemoryWrapper.DbContext);

                var mockTransactionItemViewModelFactory = new Mock<Services.IViewModelFactory>();
                mockTransactionItemViewModelFactory
                    .Setup(f => f.CreateBudgetTransactionItemViewModel(
                        It.IsAny<ObservableCollection<IAccountLinkViewModel>>(),
                        It.IsAny<BudgetTransaction>(),
                        It.IsAny<BudgetTransactionType>()))
                    .Returns(
                        (ObservableCollection<IAccountLinkViewModel> accountLinks,
                        BudgetTransaction budgetTransaction,
                        BudgetTransactionType type) =>
                        {
                            return new BudgetTransactionItemViewModel(
                                loggerFactory.CreateLogger<BudgetTransactionItemViewModel>(),
                                accountLinks,
                                budgetTransaction,
                                type);
                        });

                var mockTransactionListViewModelFactory = new Mock<Services.IViewModelFactory>();
                mockTransactionListViewModelFactory
                    .Setup(f => f.CreateBudgetTransactionListViewModel(It.IsAny<int>()))
                    .Returns((int budgetId) =>
                    {
                        return new BudgetTransactionListViewModel(
                            loggerFactory.CreateLogger<BudgetTransactionListViewModel>(),
                            accountService,
                            budgetService,
                            new Concrete.StubAccountLinkViewModelFactory(),
                            new Mock<IDeleteConfirmationViewService>().Object,
                            mockTransactionItemViewModelFactory.Object,
                            budgetId
                        );
                    });

                var viewModel = new BudgetEditViewModel(
                    loggerFactory.CreateLogger<BudgetEditViewModel>(),
                    budgetService,
                    mockTransactionListViewModelFactory.Object,
                    budget.BudgetId
                );

                viewModel.Name = "My First Budget";
                viewModel.SelectedPeriod = BudgetPeriod.Monthly;
                viewModel.CancelCommand.Execute(this);

                List<Budget> budgets = budgetService.GetAll().ToList();

                Assert.AreEqual(1, budgets.Count);
                Assert.AreEqual(budget.Name, budgets[0].Name);
                Assert.AreEqual(budget.Period, budgets[0].Period);
                Assert.AreEqual(initialTransaction.CreditAccountId, budgets[0].InitialTransaction.CreditAccount.AccountId);
                Assert.AreEqual(initialTransaction.DebitAccountId, budgets[0].InitialTransaction.DebitAccount.AccountId);
                Assert.AreEqual(initialTransaction.Amount, budgets[0].InitialTransaction.Amount);
                Assert.AreEqual(surplusTransaction.CreditAccountId, budgets[0].SurplusTransaction.CreditAccount.AccountId);
                Assert.AreEqual(surplusTransaction.DebitAccountId, budgets[0].SurplusTransaction.DebitAccount.AccountId);
                Assert.AreEqual(surplusTransaction.Amount, budgets[0].SurplusTransaction.Amount);
                Assert.AreEqual(2, budgets[0].Transactions.Count());
            }
        }

        [TestMethod]
        public void TestBudgetEditViewModelOK()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                Entities.Currency usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account savingsAccountEntity =
                    accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account rentExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.RentExpense, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                Entities.Account groceriesExpenseAccountEntity =
                    accountFactory.Create(AccountPrefab.GroceriesExpense, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentExpenseAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesExpenseAccountEntity);

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
                var rentPrepaymentToExpenseRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = rentPrepaymentAccountEntity,
                    DestinationAccount = rentExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };
                var groceriesPrepaymentToExpenseRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = groceriesPrepaymentAccountEntity,
                    DestinationAccount = groceriesExpenseAccountEntity,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(groceriesPrepaymentToExpenseRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var budget = new Entities.Budget
                {
                    Name = "Budget",
                    Period = BudgetPeriod.Fortnightly
                };
                sqliteMemoryWrapper.DbContext.Budgets.Add(budget);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var initialTransaction = new Entities.BudgetTransaction
                {
                    CreditAccount = incomeAccountEntity,
                    DebitAccount = checkingAccountEntity,
                    Amount = 200m,
                    IsInitial = true,
                    Budget = budget
                };
                var rentTransaction = new Entities.BudgetTransaction
                {
                    CreditAccount = checkingAccountEntity,
                    DebitAccount = rentPrepaymentAccountEntity,
                    Amount = 100m,
                    Budget = budget
                };
                var groceriesTransaction = new Entities.BudgetTransaction
                {
                    CreditAccount = checkingAccountEntity,
                    DebitAccount = groceriesPrepaymentAccountEntity,
                    Amount = 50m,
                    Budget = budget
                };
                var surplusTransaction = new Entities.BudgetTransaction
                {
                    CreditAccount = checkingAccountEntity,
                    DebitAccount = groceriesPrepaymentAccountEntity,
                    IsSurplus = true,
                    Budget = budget
                };
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(initialTransaction);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(rentTransaction);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(groceriesTransaction);
                sqliteMemoryWrapper.DbContext.BudgetTransactions.Add(surplusTransaction);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext);

                var budgetService = new BudgetService(
                    loggerFactory.CreateLogger<BudgetService>(),
                    sqliteMemoryWrapper.DbContext);

                var mockTransactionItemViewModelFactory = new Mock<Services.IViewModelFactory>();
                mockTransactionItemViewModelFactory
                    .Setup(f => f.CreateBudgetTransactionItemViewModel(
                        It.IsAny<ObservableCollection<IAccountLinkViewModel>>(),
                        It.IsAny<BudgetTransaction>(),
                        It.IsAny<BudgetTransactionType>()))
                    .Returns(
                        (ObservableCollection<IAccountLinkViewModel> accountLinks,
                        BudgetTransaction budgetTransaction,
                        BudgetTransactionType type) =>
                        {
                            return new BudgetTransactionItemViewModel(
                                loggerFactory.CreateLogger<BudgetTransactionItemViewModel>(),
                                accountLinks,
                                budgetTransaction,
                                type);
                        });

                var mockTransactionListViewModelFactory = new Mock<Services.IViewModelFactory>();
                mockTransactionListViewModelFactory
                    .Setup(f => f.CreateBudgetTransactionListViewModel(It.IsAny<int>()))
                    .Returns((int budgetId) =>
                    {
                        return new BudgetTransactionListViewModel(
                            loggerFactory.CreateLogger<BudgetTransactionListViewModel>(),
                            accountService,
                            budgetService,
                            new Concrete.StubAccountLinkViewModelFactory(),
                            new Mock<IDeleteConfirmationViewService>().Object,
                            mockTransactionItemViewModelFactory.Object,
                            budgetId
                        );
                    });

                var viewModel = new BudgetEditViewModel(
                    loggerFactory.CreateLogger<BudgetEditViewModel>(),
                    budgetService,
                    mockTransactionListViewModelFactory.Object,
                    budget.BudgetId
                );

                viewModel.Name = "My First Budget";
                viewModel.SelectedPeriod = BudgetPeriod.Monthly;
                viewModel.OKCommand.Execute(this);

                List<Budget> budgets = budgetService.GetAll().ToList();

                Assert.AreEqual(1, budgets.Count);
                Assert.AreEqual(viewModel.Name, budgets[0].Name);
                Assert.AreEqual(viewModel.SelectedPeriod, budgets[0].Period);
                Assert.AreEqual(initialTransaction.CreditAccountId, budgets[0].InitialTransaction.CreditAccount.AccountId);
                Assert.AreEqual(initialTransaction.DebitAccountId, budgets[0].InitialTransaction.DebitAccount.AccountId);
                Assert.AreEqual(initialTransaction.Amount, budgets[0].InitialTransaction.Amount);
                Assert.AreEqual(surplusTransaction.CreditAccountId, budgets[0].SurplusTransaction.CreditAccount.AccountId);
                Assert.AreEqual(surplusTransaction.DebitAccountId, budgets[0].SurplusTransaction.DebitAccount.AccountId);
                Assert.AreEqual(surplusTransaction.Amount, budgets[0].SurplusTransaction.Amount);
                Assert.AreEqual(2, budgets[0].Transactions.Count());
            }
        }
    }
}
