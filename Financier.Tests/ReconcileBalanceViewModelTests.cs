using System;
using Financier.Services;
using Financier.Desktop.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Financier.Tests
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
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity = accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
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
