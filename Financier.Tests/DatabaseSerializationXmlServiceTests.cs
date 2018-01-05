using Financier.Data;
using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

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
                Assert.AreEqual(new DateTime(2018, 1, 1), transactions[0].At);

                Assert.AreEqual("Checking", transactions[1].CreditAccount.Name);
                Assert.AreEqual(50m, transactions[1].CreditAmount);
                Assert.AreEqual("Rent Prepayment", transactions[1].DebitAccount.Name);
                Assert.AreEqual(50m, transactions[1].DebitAmount);
                Assert.AreEqual(new DateTime(2018, 1, 2), transactions[1].At);
            }
        }
    }
}
