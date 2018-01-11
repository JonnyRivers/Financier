using Financier.Data;
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

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(3, accounts.Count);
                Assert.AreEqual(1, accountRelationships.Count);
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

                Assert.AreEqual("Income", accounts[1].Name);
                Assert.AreEqual("USD", accounts[1].Currency.ShortName);

                Assert.AreEqual("Rent Prepayment", accounts[2].Name);
                Assert.AreEqual("USD", accounts[2].Currency.ShortName);

                Assert.AreEqual("Checking", accountRelationships[0].SourceAccount.Name);
                Assert.AreEqual("Rent Prepayment", accountRelationships[0].DestinationAccount.Name);
                Assert.AreEqual(AccountRelationshipType.PhysicalToLogical, accountRelationships[0].Type);

                Assert.AreEqual("Income", transactions[0].CreditAccount.Name);
                Assert.AreEqual(100m, transactions[0].CreditAmount);
                Assert.AreEqual("Checking", transactions[0].DebitAccount.Name);
                Assert.AreEqual(100m, transactions[0].DebitAmount);
                Assert.AreEqual(new DateTime(2018, 1, 1, 9, 0, 0), transactions[0].At);

                Assert.AreEqual("Checking", transactions[1].CreditAccount.Name);
                Assert.AreEqual(50m, transactions[1].CreditAmount);
                Assert.AreEqual("Rent Prepayment", transactions[1].DebitAccount.Name);
                Assert.AreEqual(50m, transactions[1].DebitAmount);
                Assert.AreEqual(new DateTime(2018, 1, 2, 8, 30, 0), transactions[1].At);
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

            var usdCurrency = new Currency
            {
                Name = "US Dollar",
                ShortName = "USD",
                Symbol = "$"
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

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

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

            var usdCurrency = new Currency
            {
                Name = "US Dollar",
                ShortName = "USD",
                Symbol = "$"
            };
            var checkingAccount = new Account
            {
                Name = "Checking",
                Currency = usdCurrency
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

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

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

            var usdCurrency = new Currency
            {
                Name = "US Dollar",
                ShortName = "USD",
                Symbol = "$"
            };
            var checkingAccount = new Account
            {
                Name = "Checking",
                Currency = usdCurrency
            };
            var rentPrepaymentAccount = new Account
            {
                Name = "Rent Prepayment",
                Currency = usdCurrency
            };
            var accountRelationship = new AccountRelationship
            {
                SourceAccount = checkingAccount,
                DestinationAccount = rentPrepaymentAccount,
                Type = AccountRelationshipType.PhysicalToLogical
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

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

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

            var usdCurrency = new Currency
            {
                Name = "US Dollar",
                ShortName = "USD",
                Symbol = "$"
            };
            var checkingAccount = new Account
            {
                Name = "Checking",
                Currency = usdCurrency
            };
            var rentPrepaymentAccount = new Account
            {
                Name = "Rent Prepayment",
                Currency = usdCurrency
            };
            var accountRelationship = new AccountRelationship
            {
                SourceAccount = checkingAccount,
                DestinationAccount = rentPrepaymentAccount,
                Type = AccountRelationshipType.PhysicalToLogical
            };
            var transaction = new Transaction
            {
                CreditAccount = checkingAccount,
                CreditAmount = 10m,
                DebitAccount = rentPrepaymentAccount,
                DebitAmount = 10m,
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

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(2, accounts.Count);
                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(1, transactions.Count);

                Assert.AreEqual(checkingAccount.Name, transactions[0].CreditAccount.Name);
                Assert.AreEqual(transaction.CreditAmount, transactions[0].CreditAmount);
                Assert.AreEqual(rentPrepaymentAccount.Name, transactions[0].DebitAccount.Name);
                Assert.AreEqual(transaction.DebitAmount, transactions[0].DebitAmount);
                Assert.AreEqual(transaction.At, transactions[0].At);
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
