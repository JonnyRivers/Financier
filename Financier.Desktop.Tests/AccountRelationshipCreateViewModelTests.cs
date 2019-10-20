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
    public class AccountRelationshipCreateViewModelTests
    {
        [TestMethod]
        public void TestAccountRelationshipCreateViewModelCancel()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);

                var accountService = new AccountService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var accountRelationshipService = new AccountRelationshipService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var hint = new AccountRelationship
                {
                    SourceAccount = accountService.GetAsLink(checkingAccountEntity.AccountId),
                    DestinationAccount = accountService.GetAsLink(rentPrepaymentAccountEntity.AccountId),
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var viewModel = new AccountRelationshipCreateViewModel(
                    loggerFactory,
                    accountService,
                    accountRelationshipService,
                    hint
                );

                viewModel.SelectedType = AccountRelationshipType.PrepaymentToExpense;
                viewModel.CancelCommand.Execute(this);

                List<AccountRelationship> accountRelationships = accountRelationshipService.GetAll().ToList();

                Assert.AreEqual(0, accountRelationships.Count);
            }
        }

        [TestMethod]
        public void TestAccountRelationshipCreateViewModelOK()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new AccountFactory();
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);

                var accountService = new AccountService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var accountRelationshipService = new AccountRelationshipService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var hint = new AccountRelationship
                {
                    SourceAccount = accountService.GetAsLink(checkingAccountEntity.AccountId),
                    DestinationAccount = accountService.GetAsLink(rentPrepaymentAccountEntity.AccountId),
                    Type = AccountRelationshipType.PrepaymentToExpense
                };
                var viewModel = new AccountRelationshipCreateViewModel(
                    loggerFactory,
                    accountService,
                    accountRelationshipService,
                    hint
                );

                viewModel.SelectedType = AccountRelationshipType.PhysicalToLogical;
                viewModel.OKCommand.Execute(this);

                List<AccountRelationship> accountRelationships = accountRelationshipService.GetAll().ToList();

                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(checkingAccountEntity.AccountId, accountRelationships[0].SourceAccount.AccountId);
                Assert.AreEqual(rentPrepaymentAccountEntity.AccountId, accountRelationships[0].DestinationAccount.AccountId);
                Assert.AreEqual(viewModel.SelectedType, accountRelationships[0].Type);
            }
        }
    }
}
