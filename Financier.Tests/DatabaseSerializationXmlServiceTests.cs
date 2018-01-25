using Financier.Entities;
using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class DatabaseSerializationXmlServiceTests
    {
        [TestMethod]
        public void TestLoadValid()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<DatabaseSerializationXmlService> logger = loggerFactory.CreateLogger<DatabaseSerializationXmlService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Load("TestData/Valid.xml");

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Budget> budgets = sqliteMemoryWrapper.DbContext.Budgets.ToList();
                List<Entities.BudgetTransaction> budgetTransactions = sqliteMemoryWrapper.DbContext.BudgetTransactions.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(4, accounts.Count);
                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(2, budgets.Count);
                Assert.AreEqual(5, budgetTransactions.Count);
                Assert.AreEqual(2, currencies.Count);
                Assert.AreEqual(2, transactions.Count);

                Assert.AreEqual("US Dollar", currencies[0].Name);
                Assert.AreEqual("USD", currencies[0].ShortName);
                Assert.AreEqual("$", currencies[0].Symbol);

                Assert.AreEqual("UK Sterling", currencies[1].Name);
                Assert.AreEqual("GBP", currencies[1].ShortName);
                Assert.AreEqual("£", currencies[1].Symbol);

                Assert.AreEqual("Checking", accounts[0].Name);
                Assert.AreEqual("USD", accounts[0].Currency.ShortName);
                Assert.AreEqual(Entities.AccountType.Asset, accounts[0].Type);

                Assert.AreEqual("Savings", accounts[1].Name);
                Assert.AreEqual("USD", accounts[1].Currency.ShortName);
                Assert.AreEqual(Entities.AccountType.Asset, accounts[1].Type);

                Assert.AreEqual("Income", accounts[2].Name);
                Assert.AreEqual("USD", accounts[2].Currency.ShortName);
                Assert.AreEqual(Entities.AccountType.Income, accounts[2].Type);

                Assert.AreEqual("Rent Prepayment", accounts[3].Name);
                Assert.AreEqual("USD", accounts[3].Currency.ShortName);
                Assert.AreEqual(Entities.AccountType.Asset, accounts[3].Type);

                Assert.AreEqual("Checking", accountRelationships[0].SourceAccount.Name);
                Assert.AreEqual("Rent Prepayment", accountRelationships[0].DestinationAccount.Name);
                Assert.AreEqual(Entities.AccountRelationshipType.PhysicalToLogical, accountRelationships[0].Type);

                Assert.AreEqual("Income", transactions[0].CreditAccount.Name);
                Assert.AreEqual(100m, transactions[0].Amount);
                Assert.AreEqual("Checking", transactions[0].DebitAccount.Name);
                Assert.AreEqual(new DateTime(2018, 1, 1, 9, 0, 0), transactions[0].At);

                Assert.AreEqual("Checking", transactions[1].CreditAccount.Name);
                Assert.AreEqual(50m, transactions[1].Amount);
                Assert.AreEqual("Rent Prepayment", transactions[1].DebitAccount.Name);
                Assert.AreEqual(new DateTime(2018, 1, 2, 8, 30, 0), transactions[1].At);

                Assert.AreEqual("The Budget", budgets[0].Name);
                Assert.AreEqual(Entities.BudgetPeriod.Fortnightly, budgets[0].Period);
                
                Assert.AreEqual("Income", budgetTransactions[0].CreditAccount.Name);
                Assert.AreEqual(100m, budgetTransactions[0].Amount);
                Assert.AreEqual("Checking", budgetTransactions[0].DebitAccount.Name);
                Assert.AreEqual(true, budgetTransactions[0].IsInitial);
                Assert.AreEqual(false, budgetTransactions[0].IsSurplus);
                Assert.AreEqual(budgets[0].BudgetId, budgetTransactions[0].BudgetId);
                Assert.AreEqual("Checking", budgetTransactions[1].CreditAccount.Name);
                Assert.AreEqual(80m, budgetTransactions[1].Amount);
                Assert.AreEqual("Rent Prepayment", budgetTransactions[1].DebitAccount.Name);
                Assert.AreEqual(false, budgetTransactions[1].IsInitial);
                Assert.AreEqual(false, budgetTransactions[1].IsSurplus);
                Assert.AreEqual(budgets[0].BudgetId, budgetTransactions[1].BudgetId);
                Assert.AreEqual("Checking", budgetTransactions[2].CreditAccount.Name);
                Assert.AreEqual(0m, budgetTransactions[2].Amount);
                Assert.AreEqual("Savings", budgetTransactions[2].DebitAccount.Name);
                Assert.AreEqual(false, budgetTransactions[2].IsInitial);
                Assert.AreEqual(true, budgetTransactions[2].IsSurplus);
                Assert.AreEqual(budgets[0].BudgetId, budgetTransactions[2].BudgetId);

                Assert.AreEqual("Another Budget", budgets[1].Name);
                Assert.AreEqual(Entities.BudgetPeriod.Weekly, budgets[1].Period);

                Assert.AreEqual("Income", budgetTransactions[3].CreditAccount.Name);
                Assert.AreEqual(50m, budgetTransactions[3].Amount);
                Assert.AreEqual("Checking", budgetTransactions[3].DebitAccount.Name);
                Assert.AreEqual(true, budgetTransactions[3].IsInitial);
                Assert.AreEqual(false, budgetTransactions[3].IsSurplus);
                Assert.AreEqual(budgets[1].BudgetId, budgetTransactions[3].BudgetId);
                Assert.AreEqual("Checking", budgetTransactions[4].CreditAccount.Name);
                Assert.AreEqual(0m, budgetTransactions[4].Amount);
                Assert.AreEqual("Savings", budgetTransactions[4].DebitAccount.Name);
                Assert.AreEqual(false, budgetTransactions[4].IsInitial);
                Assert.AreEqual(true, budgetTransactions[4].IsSurplus);
                Assert.AreEqual(budgets[1].BudgetId, budgetTransactions[4].BudgetId);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestIncompleteAccountElementFailure()
        {
            TestLoadFailure("TestData/IncompleteAccount.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestIncompleteAccountRelationshipElementFailure()
        {
            TestLoadFailure("TestData/IncompleteAccountRelationship.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestIncompleteBudgetElementFailure()
        {
            TestLoadFailure("TestData/IncompleteBudget.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestIncompleteBudgetTransactionElementFailure()
        {
            TestLoadFailure("TestData/IncompleteBudgetTransaction.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestIncompleteCurrencyElementFailure()
        {
            TestLoadFailure("TestData/IncompleteCurrency.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestIncompleteTransactionElementFailure()
        {
            TestLoadFailure("TestData/IncompleteTransaction.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLoadFailureBudgetWithNoInitialTransaction()
        {
            TestLoadFailure("TestData/BudgetNoInitialTransaction.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLoadFailureBudgetWithNoSurplusTransaction()
        {
            TestLoadFailure("TestData/BudgetNoSurplusTransaction.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLoadFailureBudgetInitialSurplusTransaction()
        {
            TestLoadFailure("TestData/BudgetInitialSurplusTransaction.xml");
        }

        [TestMethod]
        public void TestSaveEmpty()
        {
            const string path = "TestData/EmptySaved.xml";
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<DatabaseSerializationXmlService> logger = loggerFactory.CreateLogger<DatabaseSerializationXmlService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            XDocument document = XDocument.Load(path);
            Assert.AreEqual(0, document.Root.DescendantNodes().Count());
        }

        [TestMethod]
        public void TestReloadCurrency()
        {
            const string path = "TestData/CurrencySaved.xml";
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<DatabaseSerializationXmlService> logger = loggerFactory.CreateLogger<DatabaseSerializationXmlService>();

            var usdCurrency = new Entities.Currency
            {
                Name = "US Dollar",
                ShortName = "USD",
                Symbol = "$",
                IsPrimary = true
            };

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Load(path);

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(0, accounts.Count);
                Assert.AreEqual(0, accountRelationships.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);

                Assert.AreEqual(usdCurrency.Name, currencies[0].Name);
                Assert.AreEqual(usdCurrency.ShortName, currencies[0].ShortName);
                Assert.AreEqual(usdCurrency.Symbol, currencies[0].Symbol);
            }
        }

        [TestMethod]
        public void TestReloadAccount()
        {
            const string path = "TestData/AccountReload.xml";
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<DatabaseSerializationXmlService> logger = loggerFactory.CreateLogger<DatabaseSerializationXmlService>();

            var usdCurrency = new Entities.Currency
            {
                Name = "US Dollar",
                ShortName = "USD",
                Symbol = "$",
                IsPrimary = true
            };
            var checkingAccount = new Entities.Account
            {
                Name = "Checking",
                Currency = usdCurrency,
                Type = Entities.AccountType.Asset
            };

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Load(path);

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(1, accounts.Count);
                Assert.AreEqual(0, accountRelationships.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);

                Assert.AreEqual(checkingAccount.Name, accounts[0].Name);
                Assert.AreEqual(usdCurrency.ShortName, accounts[0].Currency.ShortName);
            }
        }

        [TestMethod]
        public void TestReloadAccountRelationship()
        {
            const string path = "TestData/AccountRelationshipReload.xml";
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<DatabaseSerializationXmlService> logger = loggerFactory.CreateLogger<DatabaseSerializationXmlService>();

            var usdCurrency = new Entities.Currency
            {
                Name = "US Dollar",
                ShortName = "USD",
                Symbol = "$",
                IsPrimary = true
            };
            var checkingAccount = new Entities.Account
            {
                Name = "Checking",
                Currency = usdCurrency,
                Type = Entities.AccountType.Asset
            };
            var rentPrepaymentAccount = new Entities.Account
            {
                Name = "Rent Prepayment",
                Currency = usdCurrency,
                Type = Entities.AccountType.Asset
            };
            var accountRelationship = new Entities.AccountRelationship
            {
                SourceAccount = checkingAccount,
                DestinationAccount = rentPrepaymentAccount,
                Type = Entities.AccountRelationshipType.PhysicalToLogical
            };

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentPrepaymentAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(accountRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Load(path);

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(2, accounts.Count);
                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);

                Assert.AreEqual(checkingAccount.Name, accountRelationships[0].SourceAccount.Name);
                Assert.AreEqual(rentPrepaymentAccount.Name, accountRelationships[0].DestinationAccount.Name);
                Assert.AreEqual(accountRelationship.Type, accountRelationships[0].Type);
            }
        }

        [TestMethod]
        public void TestReloadTransaction()
        {
            const string path = "TestData/TransactionReload.xml";
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<DatabaseSerializationXmlService> logger = loggerFactory.CreateLogger<DatabaseSerializationXmlService>();

            var usdCurrency = new Entities.Currency
            {
                Name = "US Dollar",
                ShortName = "USD",
                Symbol = "$",
                IsPrimary = true
            };
            var checkingAccount = new Entities.Account
            {
                Name = "Checking",
                Currency = usdCurrency,
                Type = Entities.AccountType.Asset
            };
            var rentPrepaymentAccount = new Entities.Account
            {
                Name = "Rent Prepayment",
                Currency = usdCurrency,
                Type = Entities.AccountType.Asset
            };
            var accountRelationship = new Entities.AccountRelationship
            {
                SourceAccount = checkingAccount,
                DestinationAccount = rentPrepaymentAccount,
                Type = Entities.AccountRelationshipType.PhysicalToLogical
            };
            var transaction = new Entities.Transaction
            {
                CreditAccount = checkingAccount,
                Amount = 10m,
                DebitAccount = rentPrepaymentAccount,
                At = new DateTime(2018, 1, 1, 8, 30, 0)
            };

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentPrepaymentAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(accountRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.Transactions.Add(transaction);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Load(path);

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(2, accounts.Count);
                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(1, transactions.Count);

                Assert.AreEqual(checkingAccount.Name, transactions[0].CreditAccount.Name);
                Assert.AreEqual(transaction.Amount, transactions[0].Amount);
                Assert.AreEqual(rentPrepaymentAccount.Name, transactions[0].DebitAccount.Name);
                Assert.AreEqual(transaction.At, transactions[0].At);
            }
        }

        [TestMethod]
        public void TestReloadBudget()
        {
            const string path = "TestData/BudgetReload.xml";
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<DatabaseSerializationXmlService> logger = loggerFactory.CreateLogger<DatabaseSerializationXmlService>();

            var usdCurrency = new Entities.Currency
            {
                Name = "US Dollar",
                ShortName = "USD",
                Symbol = "$",
                IsPrimary = true
            };
            var incomeAccount = new Entities.Account
            {
                Name = "Income",
                Currency = usdCurrency,
                Type = Entities.AccountType.Income
            };
            var checkingAccount = new Entities.Account
            {
                Name = "Checking",
                Currency = usdCurrency,
                Type = Entities.AccountType.Asset
            };
            var rentPrepaymentAccount = new Entities.Account
            {
                Name = "Rent Prepayment",
                Currency = usdCurrency,
                Type = Entities.AccountType.Asset
            };
            var savingsAccount = new Entities.Account
            {
                Name = "Savings",
                Currency = usdCurrency,
                Type = Entities.AccountType.Asset
            };
            var accountRelationship = new Entities.AccountRelationship
            {
                SourceAccount = checkingAccount,
                DestinationAccount = rentPrepaymentAccount,
                Type = Entities.AccountRelationshipType.PhysicalToLogical
            };
            var budget = new Entities.Budget
            {
                Name = "The Budget",
                Period = Entities.BudgetPeriod.Fortnightly,
                Transactions = new List<Entities.BudgetTransaction>
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccount,
                        DebitAccount = checkingAccount,
                        Amount = 100m,
                        IsInitial = true
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccount,
                        DebitAccount = rentPrepaymentAccount,
                        Amount = 80m,
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccount,
                        DebitAccount = savingsAccount,
                        Amount = 100m,
                        IsSurplus = true
                    }
                }
            };

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentPrepaymentAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(accountRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.Budgets.Add(budget);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Load(path);

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Budget> budgets = sqliteMemoryWrapper.DbContext.Budgets.ToList();
                List<Entities.BudgetTransaction> budgetTransactions = sqliteMemoryWrapper.DbContext.BudgetTransactions.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(4, accounts.Count);
                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(1, budgets.Count);
                Assert.AreEqual(3, budgetTransactions.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);

                Assert.AreEqual(budget.Name, budgets[0].Name);
                Assert.AreEqual(budget.Period, budgets[0].Period);

                Assert.AreEqual(budget.Transactions[0].DebitAccount.Name, budgetTransactions[0].DebitAccount.Name);
                Assert.AreEqual(budget.Transactions[0].CreditAccount.Name, budgetTransactions[0].CreditAccount.Name);
                Assert.AreEqual(budget.Transactions[0].Amount, budgetTransactions[0].Amount);
                Assert.AreEqual(budget.Transactions[0].IsInitial, budgetTransactions[0].IsInitial);
                Assert.AreEqual(budget.Transactions[0].IsSurplus, budgetTransactions[0].IsSurplus);
                Assert.AreEqual(budget.Transactions[1].DebitAccount.Name, budgetTransactions[1].DebitAccount.Name);
                Assert.AreEqual(budget.Transactions[1].CreditAccount.Name, budgetTransactions[1].CreditAccount.Name);
                Assert.AreEqual(budget.Transactions[1].Amount, budgetTransactions[1].Amount);
                Assert.AreEqual(budget.Transactions[1].IsInitial, budgetTransactions[1].IsInitial);
                Assert.AreEqual(budget.Transactions[1].IsSurplus, budgetTransactions[1].IsSurplus);
                Assert.AreEqual(budget.Transactions[2].DebitAccount.Name, budgetTransactions[2].DebitAccount.Name);
                Assert.AreEqual(budget.Transactions[2].CreditAccount.Name, budgetTransactions[2].CreditAccount.Name);
                Assert.AreEqual(budget.Transactions[2].Amount, budgetTransactions[2].Amount);
                Assert.AreEqual(budget.Transactions[2].IsInitial, budgetTransactions[2].IsInitial);
                Assert.AreEqual(budget.Transactions[2].IsSurplus, budgetTransactions[2].IsSurplus);
            }
        }

        private void TestLoadFailure(string path)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<DatabaseSerializationXmlService> logger = loggerFactory.CreateLogger<DatabaseSerializationXmlService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(logger, sqliteMemoryWrapper.DbContext);
                service.Load(path);
            }
        }
    }
}
