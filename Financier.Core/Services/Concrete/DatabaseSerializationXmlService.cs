using Financier.Entities;
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
            List<Currency> currencies = m_dbContext.Currencies.ToList();
            List<XElement> currencyElements = currencies.Select(ElementFromCurrency).ToList();

            List<Account> accounts = m_dbContext.Accounts.ToList();
            List<XElement> accountElements = accounts.Select(ElementFromAccount).ToList();

            List<AccountRelationship> accountRelationships = m_dbContext.AccountRelationships.ToList();
            List<XElement> accountRelationshipElements = accountRelationships.Select(ElementFromAccountRelationship).ToList();

            List<Transaction> transactions = m_dbContext.Transactions.ToList();
            List<XElement> transactionElements = transactions.Select(ElementFromTransaction).ToList();

            var document = new XDocument(
                new XElement(XName.Get("Data"),
                    currencyElements,
                    accountElements,
                    accountRelationshipElements,
                    transactionElements)
            );
            document.Save(path);
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
                Symbol = GetRequiredAttribute(element, "symbol").Value,
                IsPrimary = Boolean.Parse(GetRequiredAttribute(element, "isPrimary").Value)
            };

            return currency;
        }

        private static XElement ElementFromCurrency(Currency currency)
        {
            var element = new XElement(XName.Get("Currency"),
                new XAttribute(XName.Get("name"), currency.Name),
                new XAttribute(XName.Get("shortName"), currency.ShortName),
                new XAttribute(XName.Get("symbol"), currency.Symbol),
                new XAttribute(XName.Get("isPrimary"), currency.IsPrimary)
            );

            return element;
        }

        private static Account AccountFromElement(XElement element, Dictionary<string, Currency> currenciesByShortName)
        {
            string currencyShortName = GetRequiredAttribute(element, "currency").Value;
            var account = new Account
            {
                Currency = currenciesByShortName[currencyShortName],
                Name = GetRequiredAttribute(element, "name").Value,
                Type = (AccountType)Enum.Parse(typeof(AccountType), GetRequiredAttribute(element, "type").Value)
            };

            return account;
        }

        private static XElement ElementFromAccount(Account account)
        {
            var element = new XElement(XName.Get("Account"),
                new XAttribute(XName.Get("name"), account.Name),
                new XAttribute(XName.Get("currency"), account.Currency.ShortName),
                new XAttribute(XName.Get("type"), account.Type)
            );

            return element;
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

        private static XElement ElementFromAccountRelationship(AccountRelationship accountRelationship)
        {
            var element = new XElement(XName.Get("AccountRelationship"),
                new XAttribute(XName.Get("destination"), accountRelationship.DestinationAccount.Name),
                new XAttribute(XName.Get("source"), accountRelationship.SourceAccount.Name),
                new XAttribute(XName.Get("type"), accountRelationship.Type.ToString())
            );

            return element;
        }

        private static Transaction TransactionFromElement(XElement element, Dictionary<string, Account> accountsByShortName)
        {
            string at = GetRequiredAttribute(element, "at").Value;
            string creditAccountName = GetRequiredAttribute(element, "credit").Value;
            string amount = GetRequiredAttribute(element, "amount").Value;
            string debitAccountName = GetRequiredAttribute(element, "debit").Value;

            var transaction = new Transaction
            {
                At = DateTime.Parse(at),
                CreditAccount = accountsByShortName[creditAccountName],
                Amount = Decimal.Parse(amount),
                DebitAccount = accountsByShortName[debitAccountName]
            };

            return transaction;
        }

        private static XElement ElementFromTransaction(Transaction transaction)
        {
            var element = new XElement(XName.Get("Transaction"),
                new XAttribute(XName.Get("at"), transaction.At),
                new XAttribute(XName.Get("credit"), transaction.CreditAccount.Name),
                new XAttribute(XName.Get("amount"), transaction.Amount),
                new XAttribute(XName.Get("debit"), transaction.DebitAccount.Name)
            );

            return element;
        }
    }
}
