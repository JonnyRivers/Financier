using Financier.Data;
using Financier.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class DatabaseSerializationServiceTests
    {
        [TestMethod]
        public void TestLoadValid()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            ILogger<DatabaseSerializationService> logger = loggerFactory.CreateLogger<DatabaseSerializationService>();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationService(logger, sqliteMemoryWrapper.DbContext);
                service.Load("TestData/Valid.xml");

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(3, accounts.Count);
                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(2, currencies.Count);
                Assert.AreEqual(2, transactions.Count);
            }
        }
    }
}
