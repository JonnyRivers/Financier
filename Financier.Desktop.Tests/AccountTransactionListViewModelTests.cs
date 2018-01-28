using System;
using Financier.Services;
using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Financier.Desktop.Tests
{
    [TestClass]
    public class AccountTransactionListViewModelTests
    {
        [TestMethod]
        public void TestRegister()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<AccountTransactionListViewModel> logger = loggerFactory.CreateLogger<AccountTransactionListViewModel>();

            AccountService accountService = new AccountService(
                loggerFactory.CreateLogger<AccountService>(),
                dbContext)

            var viewModel = new AccountTransactionListViewModel(
                logger,
                accountService,
                conversionService,
                transactionService,
                viewService
                )
        }
    }
}
