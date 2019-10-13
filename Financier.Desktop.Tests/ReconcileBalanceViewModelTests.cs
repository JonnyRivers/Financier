using System;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Financier.UnitTesting;
using Financier.UnitTesting.DbSetup;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Financier.Desktop.Tests
{
    [TestClass]
    public class ReconcileBalanceViewModelTests
    {
        [TestMethod]
        public void TestReconcileBalance()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

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

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext);

                var currencyService = new CurrencyService(
                    loggerFactory.CreateLogger<CurrencyService>(),
                    sqliteMemoryWrapper.DbContext);

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext);

                var viewModelFactory = new Concrete.FakeViewModelFactory();

                var viewService = new Concrete.FakeViewService();

                var viewModel = new ReconcileBalanceViewModel(
                    loggerFactory.CreateLogger<ReconcileBalanceViewModel>(),
                    accountService,
                    currencyService,
                    transactionService,
                    new Concrete.StubAccountLinkViewModelFactory(),
                    viewModelFactory,
                    viewService,
                    checkingAccountEntity.AccountId);
                viewModel.Balance = 140m;
                viewModel.At = new DateTime(2018, 1, 2, 7, 0, 0);
                viewModel.OKCommand.Execute(null);
                Transaction newTransaction = viewModel.ToTransaction();

                Assert.AreEqual(1, viewModel.Accounts.Count);
                Assert.AreEqual(40m, newTransaction.Amount);
                Assert.AreEqual(incomeAccountEntity.AccountId, newTransaction.CreditAccount.AccountId);
                Assert.AreEqual(checkingAccountEntity.AccountId, newTransaction.DebitAccount.AccountId);
                Assert.AreEqual(viewModel.At, newTransaction.At);
            }
        }
    }
}
