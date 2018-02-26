using Financier.Desktop.ViewModels;
using Financier.Services;
using Financier.UnitTesting;
using Financier.UnitTesting.DbSetup;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Desktop.Tests
{
    [TestClass]
    public class TransactionCreateViewModelTests
    {
        [TestMethod]
        public void TestTransactionCreateViewModelCancel()
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

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext);

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext);

                var mockViewModelFactory = new Mock<Services.IViewModelFactory>();
                mockViewModelFactory
                    .Setup(f => f.CreateAccountLinkViewModel(It.IsAny<AccountLink>()))
                    .Returns((AccountLink accountLink) =>
                    {
                        return new AccountLinkViewModel(
                            loggerFactory.CreateLogger<AccountLinkViewModel>(),
                            accountLink);
                    });

                var hint = new Transaction
                {
                    CreditAccount = accountService.GetAsLink(incomeAccountEntity.AccountId),
                    DebitAccount = accountService.GetAsLink(checkingAccountEntity.AccountId),
                    Amount = 10m,
                    At = new DateTime(2018, 1, 1, 9, 0, 0)
                };

                var viewModel = new TransactionCreateViewModel(
                    loggerFactory.CreateLogger<TransactionCreateViewModel>(),
                    accountService,
                    transactionService,
                    mockViewModelFactory.Object,
                    hint
                );

                viewModel.CancelCommand.Execute(this);

                List<Transaction> transactions = transactionService.GetAll().ToList();

                Assert.AreEqual(0, transactions.Count);
            }
        }

        [TestMethod]
        public void TestTransactionCreateViewModelOK()
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

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext);

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext);

                var mockViewModelFactory = new Mock<Services.IViewModelFactory>();
                mockViewModelFactory
                    .Setup(f => f.CreateAccountLinkViewModel(It.IsAny<AccountLink>()))
                    .Returns((AccountLink accountLink) =>
                    {
                        return new AccountLinkViewModel(
                            loggerFactory.CreateLogger<AccountLinkViewModel>(),
                            accountLink);
                    });

                var hint = new Transaction
                {
                    CreditAccount = accountService.GetAsLink(incomeAccountEntity.AccountId),
                    DebitAccount = accountService.GetAsLink(checkingAccountEntity.AccountId),
                    Amount = 10m,
                    At = new DateTime(2018, 1, 1, 9, 0, 0)
                };

                var viewModel = new TransactionCreateViewModel(
                    loggerFactory.CreateLogger<TransactionCreateViewModel>(),
                    accountService,
                    transactionService,
                    mockViewModelFactory.Object,
                    hint
                );

                viewModel.Amount = 20m;
                viewModel.At = new DateTime(2018, 1, 1, 9, 25, 0);
                viewModel.OKCommand.Execute(this);

                List<Transaction> transactions = transactionService.GetAll().ToList();

                Assert.AreEqual(1, transactions.Count);
                Assert.AreEqual(incomeAccountEntity.AccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(checkingAccountEntity.AccountId, transactions[0].DebitAccount.AccountId);
                Assert.AreEqual(viewModel.Amount, transactions[0].Amount);
                Assert.AreEqual(viewModel.At, transactions[0].At);
            }
        }
    }
}
