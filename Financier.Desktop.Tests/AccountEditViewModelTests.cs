using Financier.Desktop.ViewModels;
using Financier.Services;
using Financier.UnitTesting;
using Financier.UnitTesting.DbSetup;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Desktop.Tests
{
    [TestClass]
    public class AccountEditViewModelTests
    {
        [TestMethod]
        public void TestAccountEditViewModelCancel()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext);

                var currencyService = new CurrencyService(
                    loggerFactory.CreateLogger<CurrencyService>(),
                    sqliteMemoryWrapper.DbContext);

                Currency primaryCurrency = currencyService.GetPrimary();
                var account = new Account
                {
                    Name = "My First Account",
                    Type = AccountType.Asset,
                    SubType = AccountSubType.Checking,
                    Currency = primaryCurrency,
                };
                accountService.Create(account);

                var viewModel = new AccountEditViewModel(
                    loggerFactory,
                    accountService,
                    currencyService,
                    account.AccountId
                );

                viewModel.Name = "Updated Account";
                viewModel.SelectedAccountType = AccountType.Liability;
                viewModel.SelectedAccountSubType = AccountSubType.CreditCard;
                viewModel.CancelCommand.Execute(this);

                List<Account> accounts = accountService.GetAll().ToList();

                Assert.AreEqual(1, accounts.Count);
                Assert.AreEqual(account.Name, accounts[0].Name);
                Assert.AreEqual(account.Type, accounts[0].Type);
                Assert.AreEqual(account.SubType, accounts[0].SubType);
                Assert.AreEqual(account.Currency.CurrencyId, accounts[0].Currency.CurrencyId);
            }
        }

        [TestMethod]
        public void TestAccountEditViewModelOK()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext);

                var currencyService = new CurrencyService(
                    loggerFactory.CreateLogger<CurrencyService>(),
                    sqliteMemoryWrapper.DbContext);

                Currency primaryCurrency = currencyService.GetPrimary();
                var account = new Account
                {
                    Name = "My First Account",
                    Type = AccountType.Asset,
                    SubType = AccountSubType.Checking,
                    Currency = primaryCurrency,
                };
                accountService.Create(account);

                var viewModel = new AccountEditViewModel(
                    loggerFactory,
                    accountService,
                    currencyService,
                    account.AccountId
                );

                viewModel.Name = "Updated Account";
                viewModel.SelectedAccountType = AccountType.Liability;
                viewModel.SelectedAccountSubType = AccountSubType.CreditCard;
                viewModel.OKCommand.Execute(this);

                List<Account> accounts = accountService.GetAll().ToList();

                Assert.AreEqual(1, accounts.Count);
                Assert.AreEqual(viewModel.Name, accounts[0].Name);
                Assert.AreEqual(viewModel.SelectedAccountType, accounts[0].Type);
                Assert.AreEqual(viewModel.SelectedAccountSubType, accounts[0].SubType);
                Assert.AreEqual(account.Currency.CurrencyId, accounts[0].Currency.CurrencyId);
            }
        }
    }
}
