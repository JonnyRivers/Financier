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
    public class TransactionEditViewModelTests
    {
        [TestMethod]
        public void TestTransactionEditViewModelCancel()
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

                var transactionEntity = new Entities.Transaction
                {
                    CreditAccount = incomeAccountEntity,
                    DebitAccount = checkingAccountEntity,
                    Amount = 10m,
                    At = new DateTime(2018, 1, 1, 9, 0, 0)
                };
                sqliteMemoryWrapper.DbContext.Transactions.Add(transactionEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

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

                var viewModel = new TransactionEditViewModel(
                    loggerFactory.CreateLogger<TransactionEditViewModel>(),
                    accountService,
                    transactionService,
                    mockViewModelFactory.Object,
                    transactionEntity.TransactionId
                );

                viewModel.Amount = 20m;
                viewModel.At = new DateTime(2018, 1, 1, 9, 25, 0);
                viewModel.CancelCommand.Execute(this);

                List<Transaction> transactions = transactionService.GetAll().ToList();

                Assert.AreEqual(1, transactions.Count);
                Assert.AreEqual(transactionEntity.CreditAccount.AccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(transactionEntity.DebitAccount.AccountId, transactions[0].DebitAccount.AccountId);
                Assert.AreEqual(transactionEntity.Amount, transactions[0].Amount);
                Assert.AreEqual(transactionEntity.At, transactions[0].At);
            }
        }

        [TestMethod]
        public void TestTransactionEditViewModelOK()
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

                var transactionEntity = new Entities.Transaction
                {
                    CreditAccount = incomeAccountEntity,
                    DebitAccount = checkingAccountEntity,
                    Amount = 10m,
                    At = new DateTime(2018, 1, 1, 9, 0, 0)
                };
                sqliteMemoryWrapper.DbContext.Transactions.Add(transactionEntity);
                sqliteMemoryWrapper.DbContext.SaveChanges();

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

                var viewModel = new TransactionEditViewModel(
                    loggerFactory.CreateLogger<TransactionEditViewModel>(),
                    accountService,
                    transactionService,
                    mockViewModelFactory.Object,
                    transactionEntity.TransactionId
                );

                viewModel.Amount = 20m;
                viewModel.At = new DateTime(2018, 1, 1, 9, 25, 0);
                viewModel.OKCommand.Execute(this);

                List<Transaction> transactions = transactionService.GetAll().ToList();

                Assert.AreEqual(1, transactions.Count);
                Assert.AreEqual(transactionEntity.CreditAccount.AccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(transactionEntity.DebitAccount.AccountId, transactions[0].DebitAccount.AccountId);
                Assert.AreEqual(viewModel.Amount, transactions[0].Amount);
                Assert.AreEqual(viewModel.At, transactions[0].At);
            }
        }
    }
}
