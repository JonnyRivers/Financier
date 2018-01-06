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

        private static XAttribute GetRequiredAttribute(XElement element, string attributeName)
        {
            XAttribute attribute = element.Attribute(XName.Get(attributeName));
            if (attribute == null)
            {
                throw new ArgumentException(
                    $"Required attribute {attributeName} is missing",
                    nameof(element)
                );
            }

            return attribute;
        }

        private static Currency CurrencyFromElement(XElement element)
        {
            var currency = new Currency
            {
                Name = GetRequiredAttribute(element, "name").Value,
                ShortName = GetRequiredAttribute(element, "shortName").Value,
                Symbol = GetRequiredAttribute(element, "symbol").Value
            };

            return currency;
        }

        private static Account AccountFromElement(XElement element, Dictionary<string, Currency> currenciesByShortName)
        {
            string currencyShortName = GetRequiredAttribute(element, "currency").Value;
            var account = new Account
            {
                Currency = currenciesByShortName[currencyShortName],
                Name = GetRequiredAttribute(element, "name").Value
            };

            return account;
        }

        private static AccountRelationship AccountRelationshipFromElement(XElement element, Dictionary<string, Account> accountsByShortName)
        {
            string destinationAccountName = GetRequiredAttribute(element, "destination").Value;
            string sourceAcountName = GetRequiredAttribute(element, "source").Value;
            string type = GetRequiredAttribute(element, "type").Value;

            var accountRelationship = new AccountRelationship
            {
                DestinationAccount = accountsByShortName[destinationAccountName],
                SourceAccount = accountsByShortName[sourceAcountName],
                Type = (AccountRelationshipType)Enum.Parse(typeof(AccountRelationshipType), type)
            };

            return accountRelationship;
        }

        private static Transaction TransactionFromElement(XElement element, Dictionary<string, Account> accountsByShortName)
        {
            string at = GetRequiredAttribute(element, "at").Value;
            string creditAccountName = GetRequiredAttribute(element, "credit").Value;
            string creditAmount = GetRequiredAttribute(element, "creditAmount").Value;
            string debitAccountName = GetRequiredAttribute(element, "debit").Value;
            string debitAmount = GetRequiredAttribute(element, "debitAmount").Value;

            var transaction = new Transaction
            {
                At = DateTime.Parse(at),
                CreditAccount = accountsByShortName[creditAccountName],
                CreditAmount = Decimal.Parse(creditAmount),
                DebitAccount = accountsByShortName[debitAccountName],
                DebitAmount = Decimal.Parse(debitAmount)
            };

            return transaction;
        }
    }
}
