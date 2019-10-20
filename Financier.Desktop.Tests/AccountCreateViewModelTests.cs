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
    public class AccountCreateViewModelTests
    {
        [TestMethod]
        public void TestAccountCreateViewModelCancel()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountService = new AccountService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var currencyService = new CurrencyService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var viewModel = new AccountCreateViewModel(
                    loggerFactory,
                    accountService,
                    currencyService
                );

                viewModel.Name = "MyAccount";
                viewModel.SelectedAccountType = AccountType.Asset;
                viewModel.SelectedAccountSubType = AccountSubType.Investment;
                viewModel.CancelCommand.Execute(this);

                List<Account> accounts = accountService.GetAll().ToList();

                Assert.AreEqual(0, accounts.Count);
            }
        }

        [TestMethod]
        public void TestAccountCreateViewModelOK()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountService = new AccountService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var currencyService = new CurrencyService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var viewModel = new AccountCreateViewModel(
                    loggerFactory,
                    accountService,
                    currencyService
                );

                viewModel.Name = "MyAccount";
                viewModel.SelectedAccountType = AccountType.Asset;
                viewModel.SelectedAccountSubType = AccountSubType.Investment;
                viewModel.OKCommand.Execute(this);

                List<Account> accounts = accountService.GetAll().ToList();

                Assert.AreEqual(1, accounts.Count);
                Assert.AreEqual(usdCurrencyEntity.CurrencyId, accounts[0].Currency.CurrencyId);
                Assert.AreEqual(viewModel.Name, accounts[0].Name);
                Assert.AreEqual(viewModel.SelectedAccountType, accounts[0].Type);
                Assert.AreEqual(viewModel.SelectedAccountSubType, accounts[0].SubType);
            }
        }
    }
}
