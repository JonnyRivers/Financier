using Financier.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Financier.Services
{
    public class DatabaseSerializationXmlService : IDatabaseSerializationService
    {
        ILogger<DatabaseSerializationXmlService> m_logger;
        FinancierDbContext m_dbContext;

        public DatabaseSerializationXmlService(ILogger<DatabaseSerializationXmlService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Load(string path)
        {
            m_dbContext.Database.EnsureDeleted();
            m_logger.LogInformation("Database is obliterated");

            m_dbContext.Database.EnsureCreated();
            m_logger.LogInformation("Database is created");

            XDocument document = XDocument.Load(path);
            XElement rootElement = document.Root;

            List<XElement> currencyElements = rootElement.Elements(XName.Get("Currency")).ToList();
            List<Currency> currencies = currencyElements.Select(CurrencyFromElement).ToList();
            m_dbContext.Currencies.AddRange(currencies);
            m_dbContext.SaveChanges();
            Dictionary<string, Currency> currenciesByShortName = currencies.ToDictionary(c => c.ShortName, c => c);

            List<XElement> accountElements = rootElement.Elements(XName.Get("Account")).ToList();
            List<Account> accounts = accountElements.Select(e => AccountFromElement(e, currenciesByShortName)).ToList();
            m_dbContext.Accounts.AddRange(accounts);
            m_dbContext.SaveChanges();
            Dictionary<string, Account> accountsByName = accounts.ToDictionary(a => a.Name, a => a);

            List<XElement> accountRelationshipElements = rootElement.Elements(XName.Get("AccountRelationship")).ToList();
            List<AccountRelationship> accountRelationships = accountRelationshipElements.Select(e => AccountRelationshipFromElement(e, accountsByName)).ToList();
            m_dbContext.AccountRelationships.AddRange(accountRelationships);
            m_dbContext.SaveChanges();

            List<XElement> transactionElements = rootElement.Elements(XName.Get("Transaction")).ToList();
            List<Transaction> transactions = transactionElements.Select(e => TransactionFromElement(e, accountsByName)).ToList();
            m_dbContext.Transactions.AddRange(transactions);
            m_dbContext.SaveChanges();
        }

        public void Save(string path)
        {
            throw new System.NotImplementedException();
        }

        private static Currency CurrencyFromElement(XElement element)
        {
            var currency = new Currency
            {
                Name = element.Attribute(XName.Get("name")).Value,
                ShortName = element.Attribute(XName.Get("shortName")).Value,
                Symbol = element.Attribute(XName.Get("symbol")).Value
            };

            return currency;
        }

        private static Account AccountFromElement(XElement element, Dictionary<string, Currency> currenciesByShortName)
        {
            var account = new Account
            {
                Currency = currenciesByShortName[element.Attribute(XName.Get("currency")).Value],
                Name = element.Attribute(XName.Get("name")).Value
            };

            return account;
        }

        private static AccountRelationship AccountRelationshipFromElement(XElement element, Dictionary<string, Account> accountsByShortName)
        {
            var accountRelationship = new AccountRelationship
            {
                DestinationAccount = accountsByShortName[element.Attribute(XName.Get("destination")).Value],
                SourceAccount = accountsByShortName[element.Attribute(XName.Get("source")).Value],
                Type = (AccountRelationshipType)Enum.Parse(typeof(AccountRelationshipType), element.Attribute(XName.Get("type")).Value)
            };

            return accountRelationship;
        }

        private static Transaction TransactionFromElement(XElement element, Dictionary<string, Account> accountsByShortName)
        {
            var transaction = new Transaction
            {
                At = DateTime.Parse(element.Attribute(XName.Get("at")).Value),
                CreditAccount = accountsByShortName[element.Attribute(XName.Get("credit")).Value],
                CreditAmount = Decimal.Parse(element.Attribute(XName.Get("creditAmount")).Value),
                DebitAccount = accountsByShortName[element.Attribute(XName.Get("debit")).Value],
                DebitAmount = Decimal.Parse(element.Attribute(XName.Get("debitAmount")).Value)
            };

            return transaction;
        }
    }
}
