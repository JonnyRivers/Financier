﻿using System;
using Financier.Services;
using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Financier.Tests
{
    [TestClass]
    public class AccountTransactionListViewModelTests
    {
        [TestMethod]
        public void TestNoTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountTransactionListViewModel> logger = loggerFactory.CreateLogger<AccountTransactionListViewModel>();

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

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(), 
                    sqliteMemoryWrapper.DbContext);

                var conversionService = new ConversionService(loggerFactory.CreateLogger<ConversionService>());

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext);

                var viewService = new Concrete.FakeViewService();

                var viewModel = new AccountTransactionListViewModel(
                    logger,
                    accountService,
                    conversionService,
                    transactionService,
                    viewService);

                viewModel.Setup(checkingAccountEntity.AccountId);

                Assert.AreEqual(false, viewModel.HasLogicalAcounts);
                Assert.AreEqual(0, viewModel.Transactions.Count);
            }
        }

        [TestMethod]
        public void TestAccountWithLogicalsAndTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountTransactionListViewModel> logger = loggerFactory.CreateLogger<AccountTransactionListViewModel>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var currencyFactory = new DbSetup.CurrencyFactory();
                var usdCurrencyEntity = currencyFactory.Create(DbSetup.CurrencyPrefab.Usd, true);
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var accountFactory = new DbSetup.AccountFactory();
                Entities.Account incomeAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Income, usdCurrencyEntity);
                Entities.Account checkingAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.Checking, usdCurrencyEntity);
                Entities.Account rentPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.RentPrepayment, usdCurrencyEntity);
                Entities.Account groceriesPrepaymentAccountEntity =
                    accountFactory.Create(DbSetup.AccountPrefab.GroceriesPrepayment, usdCurrencyEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, groceriesPrepaymentAccountEntity);

                var transactionEntities = new Entities.Transaction[]
                {
                    new Entities.Transaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        At = new DateTime(2018, 1, 1, 8, 30, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 60m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    },
                    new Entities.Transaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = groceriesPrepaymentAccountEntity,
                        Amount = 10m,
                        At = new DateTime(2018, 1, 1, 8, 31, 0)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionEntities);
                sqliteMemoryWrapper.DbContext.SaveChanges();

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

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToGroceriesPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var accountService = new AccountService(
                    loggerFactory.CreateLogger<AccountService>(),
                    sqliteMemoryWrapper.DbContext);

                var conversionService = new ConversionService(loggerFactory.CreateLogger<ConversionService>());

                var transactionService = new TransactionService(
                    loggerFactory.CreateLogger<TransactionService>(),
                    sqliteMemoryWrapper.DbContext);

                var viewService = new Concrete.FakeViewService();

                var viewModel = new AccountTransactionListViewModel(
                    logger,
                    accountService,
                    conversionService,
                    transactionService,
                    viewService);

                viewModel.Setup(checkingAccountEntity.AccountId);

                Assert.AreEqual(true, viewModel.HasLogicalAcounts);
                Assert.AreEqual(3, viewModel.Transactions.Count);
            }
        }
    }
}
