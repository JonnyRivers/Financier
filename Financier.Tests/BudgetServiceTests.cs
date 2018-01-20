using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class BudgetServiceTests
    {
        [TestMethod]
        public void TestBudgetCreateFailNullInitialTransaction()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BudgetService> logger = loggerFactory.CreateLogger<BudgetService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(logger, sqliteMemoryWrapper.DbContext);

                var budget = new Budget
                {
                    Name = "Budget",
                    InitialTransaction = null,
                    Transactions = new BudgetTransaction[0],
                    SurplusTransaction = new BudgetTransaction
                    {
                        CreditAccount = null,
                        DebitAccount = null
                    }
                };

                budgetService.Create(budget);
            }
        }

        [TestMethod]
        public void TestBudgetCreateFailNullSurplusTransaction()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BudgetService> logger = loggerFactory.CreateLogger<BudgetService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(logger, sqliteMemoryWrapper.DbContext);

                var budget = new Budget
                {
                    Name = "Budget",
                    InitialTransaction = new BudgetTransaction
                    {
                        CreditAccount = null,
                        DebitAccount = null
                    },
                    Transactions = new BudgetTransaction[0],
                    SurplusTransaction = null
                };

                budgetService.Create(budget);
            }
        }

        [TestMethod]
        public void TestBudgetCreateNewTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BudgetService> logger = loggerFactory.CreateLogger<BudgetService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(logger, sqliteMemoryWrapper.DbContext);

                var budget = new Budget
                {
                    Name = "Budget",
                    InitialTransaction = new BudgetTransaction
                    {
                        CreditAccount = null,
                        DebitAccount = null
                    },
                    Transactions = new BudgetTransaction[1] {
                        new BudgetTransaction
                        {
                            CreditAccount = null,
                            DebitAccount = null
                        }
                    },
                    SurplusTransaction = new BudgetTransaction
                    {
                        CreditAccount = null,
                        DebitAccount = null
                    }
                };

                budgetService.Create(budget);
            }
        }

        [TestMethod]
        public void TestBudgetGetAllEmpty()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BudgetService> logger = loggerFactory.CreateLogger<BudgetService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(logger, sqliteMemoryWrapper.DbContext);

                IEnumerable<Budget> budgets = budgetService.GetAll();

                Assert.AreEqual(0, budgets.Count());
            }
        }

        [TestMethod]
        public void TestBudgetGetAllMany()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BudgetService> logger = loggerFactory.CreateLogger<BudgetService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(logger, sqliteMemoryWrapper.DbContext);

                IEnumerable<Budget> budgets = budgetService.GetAll();

                Assert.AreEqual(2, budgets.Count());
            }
        }

        [TestMethod]
        public void TestBudgetGetFailInvalidId()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BudgetService> logger = loggerFactory.CreateLogger<BudgetService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(logger, sqliteMemoryWrapper.DbContext);

                Budget budgets = budgetService.Get(666);
            }
        }

        [TestMethod]
        public void TestBudgetGet()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BudgetService> logger = loggerFactory.CreateLogger<BudgetService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(logger, sqliteMemoryWrapper.DbContext);

                Budget budgets = budgetService.Get(1);
            }
        }

        [TestMethod]
        public void TestBudgetUpdate()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BudgetService> logger = loggerFactory.CreateLogger<BudgetService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(logger, sqliteMemoryWrapper.DbContext);

                Budget budget = null;
                budgetService.Update(budget);
            }
        }

        [TestMethod]
        public void TestBudgetUpdateRemoveTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BudgetService> logger = loggerFactory.CreateLogger<BudgetService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(logger, sqliteMemoryWrapper.DbContext);

                Budget budget = null;
                budgetService.Update(budget);
            }
        }

        [TestMethod]
        public void TestBudgetUpdateAddTransactions()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<BudgetService> logger = loggerFactory.CreateLogger<BudgetService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var budgetService = new BudgetService(logger, sqliteMemoryWrapper.DbContext);

                Budget budget = null;
                budgetService.Update(budget);
            }
        }
    }
}
