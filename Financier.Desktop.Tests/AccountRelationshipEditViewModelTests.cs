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
    public class AccountRelationshipEditViewModelTests
    {
        [TestMethod]
        public void TestAccountRelationshipEditViewModelCancel()
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

                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var accountRelationshipService = new AccountRelationshipService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var viewModel = new AccountRelationshipEditViewModel(
                    loggerFactory,
                    accountService,
                    accountRelationshipService,
                    checkingToRentPrepaymentRelationship.AccountRelationshipId
                );

                viewModel.SelectedType = AccountRelationshipType.PrepaymentToExpense;
                viewModel.CancelCommand.Execute(this);

                List<AccountRelationship> accountRelationships = accountRelationshipService.GetAll().ToList();

                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(checkingToRentPrepaymentRelationship.SourceAccountId, 
                    accountRelationships[0].SourceAccount.AccountId);
                Assert.AreEqual(checkingToRentPrepaymentRelationship.DestinationAccountId, 
                    accountRelationships[0].DestinationAccount.AccountId);
                Assert.AreEqual(checkingToRentPrepaymentRelationship.Type, 
                    accountRelationships[0].Type);
            }
        }

        [TestMethod]
        public void TestAccountRelationshipEditViewModelOK()
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

                var checkingToRentPrepaymentRelationship = new Entities.AccountRelationship
                {
                    SourceAccount = checkingAccountEntity,
                    DestinationAccount = rentPrepaymentAccountEntity,
                    Type = AccountRelationshipType.PhysicalToLogical
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var accountRelationshipService = new AccountRelationshipService(
                    loggerFactory,
                    sqliteMemoryWrapper.DbContext);

                var viewModel = new AccountRelationshipEditViewModel(
                    loggerFactory,
                    accountService,
                    accountRelationshipService,
                    checkingToRentPrepaymentRelationship.AccountRelationshipId
                );

                viewModel.SelectedType = AccountRelationshipType.PrepaymentToExpense;
                viewModel.OKCommand.Execute(this);

                List<AccountRelationship> accountRelationships = accountRelationshipService.GetAll().ToList();

                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(checkingToRentPrepaymentRelationship.SourceAccountId,
                    accountRelationships[0].SourceAccount.AccountId);
                Assert.AreEqual(checkingToRentPrepaymentRelationship.DestinationAccountId,
                    accountRelationships[0].DestinationAccount.AccountId);
                Assert.AreEqual(viewModel.SelectedType,
                    accountRelationships[0].Type);
            }
        }
    }
}
