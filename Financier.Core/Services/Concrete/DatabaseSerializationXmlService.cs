using Financier.Entities;
using Microsoft.EntityFrameworkCore;
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
            //TODO: clean database on Load()
            //https://github.com/JonnyRivers/Financier/issues/51

            m_dbContext.Database.EnsureCreated();
            m_logger.LogInformation("Database is created");

            m_dbContext.BudgetTransactions.RemoveRange(m_dbContext.BudgetTransactions);
            m_dbContext.SaveChanges();
            m_dbContext.Budgets.RemoveRange(m_dbContext.Budgets);
            m_dbContext.SaveChanges();
            m_dbContext.TransactionRelationships.RemoveRange(m_dbContext.TransactionRelationships);
            m_dbContext.SaveChanges();
            m_dbContext.Transactions.RemoveRange(m_dbContext.Transactions);
            m_dbContext.SaveChanges();
            m_dbContext.AccountRelationships.RemoveRange(m_dbContext.AccountRelationships);
            m_dbContext.SaveChanges();
            m_dbContext.Accounts.RemoveRange(m_dbContext.Accounts);
            m_dbContext.SaveChanges();
            m_dbContext.Currencies.RemoveRange(m_dbContext.Currencies);
            m_dbContext.SaveChanges();

            XDocument document = XDocument.Load(path);
            XElement rootElement = document.Root;

            List<XElement> currencyElements = rootElement.Elements(XName.Get("Currency")).ToList();
            List<Entities.Currency> currencies = currencyElements.Select(CurrencyFromElement).ToList();
            m_dbContext.Currencies.AddRange(currencies);
            m_dbContext.SaveChanges();
            Dictionary<string, Entities.Currency> currenciesByShortName = currencies.ToDictionary(c => c.ShortName, c => c);

            List<XElement> accountElements = rootElement.Elements(XName.Get("Account")).ToList();
            List<Entities.Account> accounts = accountElements.Select(e => AccountFromElement(e, currenciesByShortName)).ToList();
            m_dbContext.Accounts.AddRange(accounts);
            m_dbContext.SaveChanges();
            Dictionary<string, Entities.Account> accountsByName = accounts.ToDictionary(a => a.Name, a => a);

            List<XElement> accountRelationshipElements = rootElement.Elements(XName.Get("AccountRelationship")).ToList();
            List<Entities.AccountRelationship> accountRelationships = accountRelationshipElements.Select(e => AccountRelationshipFromElement(e, accountsByName)).ToList();
            m_dbContext.AccountRelationships.AddRange(accountRelationships);
            m_dbContext.SaveChanges();

            var transactionsByIncomingId = new Dictionary<int, Entities.Transaction>();
            List<XElement> transactionElements = rootElement.Elements(XName.Get("Transaction")).ToList();
            List<Entities.Transaction> transactions = 
                transactionElements
                    .Select(e => TransactionFromElement(e, accountsByName, transactionsByIncomingId))
                    .ToList();
            m_dbContext.Transactions.AddRange(transactions);
            m_dbContext.SaveChanges();

            List<XElement> transactionRelationshipElements = rootElement.Elements(XName.Get("TransactionRelationship")).ToList();
            List<Entities.TransactionRelationship> transactionRelationships = 
                transactionRelationshipElements
                    .Select(e => TransactionRelationshipFromElement(e, transactionsByIncomingId)).ToList();
            m_dbContext.TransactionRelationships.AddRange(transactionRelationships);
            m_dbContext.SaveChanges();

            List<XElement> budgetElements = rootElement.Elements(XName.Get("Budget")).ToList();
            List<Entities.Budget> budgets = budgetElements.Select(b => BudgetFromElement(b, accountsByName)).ToList();
            m_dbContext.Budgets.AddRange(budgets);
            m_dbContext.SaveChanges();
        }

        public void Save(string path)
        {
            List<Entities.Currency> currencies = m_dbContext.Currencies.ToList();
            List<XElement> currencyElements = currencies.Select(ElementFromCurrency).ToList();

            List<Entities.Account> accounts = m_dbContext.Accounts.ToList();
            List<XElement> accountElements = accounts.Select(ElementFromAccount).ToList();

            List<Entities.AccountRelationship> accountRelationships = m_dbContext.AccountRelationships.ToList();
            List<XElement> accountRelationshipElements = accountRelationships.Select(ElementFromAccountRelationship).ToList();

            List<Entities.Transaction> transactions = m_dbContext.Transactions.ToList();
            List<XElement> transactionElements = transactions.Select(ElementFromTransaction).ToList();

            List<Entities.TransactionRelationship> transactionRelationships = m_dbContext.TransactionRelationships.ToList();
            List<XElement> transactionRelationshipElements = transactionRelationships.Select(ElementFromTransactionRelationship).ToList();

            List<Entities.Budget> budgets = m_dbContext
                .Budgets
                .Include(b => b.Transactions)
                .ToList();
            List<XElement> budgetsElements = budgets.Select(ElementFromBudget).ToList();

            var document = new XDocument(
                new XElement(XName.Get("Data"),
                    currencyElements,
                    accountElements,
                    accountRelationshipElements,
                    transactionElements,
                    transactionRelationshipElements,
                    budgetsElements)
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

        private static bool AttributeExistsAndIsTrue(XElement element, string attributeName)
        {
            XAttribute attribute = element.Attribute(XName.Get(attributeName));
            if (attribute == null)
            {
                return false;
            }

            bool valueAsBool;
            if (Boolean.TryParse(attribute.Value, out valueAsBool))
            {
                return valueAsBool;
            }

            return false;
        }

        private static Entities.Currency CurrencyFromElement(XElement element)
        {
            var currency = new Entities.Currency
            {
                Name = GetRequiredAttribute(element, "name").Value,
                ShortName = GetRequiredAttribute(element, "shortName").Value,
                Symbol = GetRequiredAttribute(element, "symbol").Value,
                IsPrimary = Boolean.Parse(GetRequiredAttribute(element, "isPrimary").Value)
            };

            return currency;
        }

        private static XElement ElementFromCurrency(Entities.Currency currency)
        {
            var element = new XElement(XName.Get("Currency"),
                new XAttribute(XName.Get("name"), currency.Name),
                new XAttribute(XName.Get("shortName"), currency.ShortName),
                new XAttribute(XName.Get("symbol"), currency.Symbol),
                new XAttribute(XName.Get("isPrimary"), currency.IsPrimary)
            );

            return element;
        }

        private static Entities.Account AccountFromElement(XElement element, Dictionary<string, Entities.Currency> currenciesByShortName)
        {
            string currencyShortName = GetRequiredAttribute(element, "currency").Value;
            var account = new Entities.Account
            {
                Currency = currenciesByShortName[currencyShortName],
                Name = GetRequiredAttribute(element, "name").Value,
                Type = (AccountType)Enum.Parse(typeof(AccountType), GetRequiredAttribute(element, "type").Value),
                SubType = (AccountSubType)Enum.Parse(typeof(AccountSubType), GetRequiredAttribute(element, "subType").Value)
            };

            return account;
        }

        private static XElement ElementFromAccount(Entities.Account account)
        {
            var element = new XElement(XName.Get("Account"),
                new XAttribute(XName.Get("name"), account.Name),
                new XAttribute(XName.Get("currency"), account.Currency.ShortName),
                new XAttribute(XName.Get("type"), account.Type),
                new XAttribute(XName.Get("subType"), account.SubType)
            );

            return element;
        }

        private static Entities.AccountRelationship AccountRelationshipFromElement(XElement element, Dictionary<string, Entities.Account> accountsByShortName)
        {
            string destinationAccountName = GetRequiredAttribute(element, "destination").Value;
            string sourceAcountName = GetRequiredAttribute(element, "source").Value;
            string type = GetRequiredAttribute(element, "type").Value;

            var accountRelationship = new Entities.AccountRelationship
            {
                DestinationAccount = accountsByShortName[destinationAccountName],
                SourceAccount = accountsByShortName[sourceAcountName],
                Type = (AccountRelationshipType)Enum.Parse(typeof(AccountRelationshipType), type)
            };

            return accountRelationship;
        }

        private static XElement ElementFromAccountRelationship(Entities.AccountRelationship accountRelationship)
        {
            var element = new XElement(XName.Get("AccountRelationship"),
                new XAttribute(XName.Get("destination"), accountRelationship.DestinationAccount.Name),
                new XAttribute(XName.Get("source"), accountRelationship.SourceAccount.Name),
                new XAttribute(XName.Get("type"), accountRelationship.Type.ToString())
            );

            return element;
        }

        private static Entities.Transaction TransactionFromElement(
            XElement element, 
            Dictionary<string, Entities.Account> accountsByShortName,
            Dictionary<int, Entities.Transaction> transactionsByIncomingId
            )
        {
            string incomingIdText = GetRequiredAttribute(element, "id").Value;
            int incomingId = Int32.Parse(incomingIdText);

            string at = GetRequiredAttribute(element, "at").Value;
            string creditAccountName = GetRequiredAttribute(element, "credit").Value;
            string amount = GetRequiredAttribute(element, "amount").Value;
            string debitAccountName = GetRequiredAttribute(element, "debit").Value;

            var transaction = new Entities.Transaction
            {
                At = DateTime.Parse(at),
                CreditAccount = accountsByShortName[creditAccountName],
                Amount = Decimal.Parse(amount),
                DebitAccount = accountsByShortName[debitAccountName]
            };

            transactionsByIncomingId.Add(incomingId, transaction);

            return transaction;
        }

        private static XElement ElementFromTransaction(Entities.Transaction transaction)
        {
            var element = new XElement(XName.Get("Transaction"),
                new XAttribute(XName.Get("id"), transaction.TransactionId),
                new XAttribute(XName.Get("at"), transaction.At),
                new XAttribute(XName.Get("credit"), transaction.CreditAccount.Name),
                new XAttribute(XName.Get("amount"), transaction.Amount),
                new XAttribute(XName.Get("debit"), transaction.DebitAccount.Name)
            );

            return element;
        }

        private static Entities.TransactionRelationship TransactionRelationshipFromElement(
            XElement element, 
            Dictionary<int, Entities.Transaction> transactionsById)
        {
            string destinationTransactionIdText = GetRequiredAttribute(element, "destination").Value;
            int destinationTransactionId = Int32.Parse(destinationTransactionIdText);
            string sourceTransactionIdText = GetRequiredAttribute(element, "source").Value;
            int sourceTransactionId = Int32.Parse(sourceTransactionIdText);
            string type = GetRequiredAttribute(element, "type").Value;

            var transactionRelationship = new Entities.TransactionRelationship
            {
                DestinationTransaction = transactionsById[destinationTransactionId],
                SourceTransaction = transactionsById[sourceTransactionId],
                Type = (TransactionRelationshipType)Enum.Parse(typeof(TransactionRelationshipType), type)
            };

            return transactionRelationship;
        }

        private static XElement ElementFromTransactionRelationship(Entities.TransactionRelationship transactionRelationship)
        {
            var element = new XElement(XName.Get("TransactionRelationship"),
                new XAttribute(XName.Get("destination"), transactionRelationship.DestinationTransactionId),
                new XAttribute(XName.Get("source"), transactionRelationship.SourceTransactionId),
                new XAttribute(XName.Get("type"), transactionRelationship.Type.ToString())
            );

            return element;
        }

        private static Entities.Budget BudgetFromElement(
            XElement element,
            Dictionary<string, Entities.Account> accountsByName)
        {
            string name = GetRequiredAttribute(element, "name").Value;
            string period = GetRequiredAttribute(element, "period").Value;

            var budget = new Entities.Budget
            {
                Name = name,
                Period = (BudgetPeriod)Enum.Parse(typeof(BudgetPeriod), period),
                Transactions = element
                    .Elements(XName.Get("BudgetTransaction"))
                    .Select(e => BudgetTransactionFromElement(e, accountsByName)).ToList()
            };

            if (budget.Transactions.Count(t => t.IsInitial) != 1)
                throw new ArgumentException("Budget element must have exactly one initial transaction", nameof(element));
            if (budget.Transactions.Count(t => t.IsSurplus) != 1)
                throw new ArgumentException("Budget element must have exactly one surplus transaction", nameof(element));

            return budget;
        }

        private static Entities.BudgetTransaction BudgetTransactionFromElement(
            XElement element,
            Dictionary<string, Entities.Account> accountsByName)
        {
            string creditAccountName = GetRequiredAttribute(element, "credit").Value;
            string debitAccountName = GetRequiredAttribute(element, "debit").Value;
            string amount = GetRequiredAttribute(element, "amount").Value;
            bool isInitial = AttributeExistsAndIsTrue(element, "isInitial");
            bool isSurplus = AttributeExistsAndIsTrue(element, "isSurplus");

            var newBudgetTransaction = new Entities.BudgetTransaction
            {
                CreditAccountId = accountsByName[creditAccountName].AccountId,
                DebitAccountId = accountsByName[debitAccountName].AccountId,
                Amount = Decimal.Parse(amount),
                IsInitial = isInitial,
                IsSurplus = isSurplus
            };

            if(newBudgetTransaction.IsInitial && newBudgetTransaction.IsSurplus)
                throw new ArgumentException(
                    "Budget contains transaction with initial and surplus flags set.  " + 
                    "Either or neither must be set, never both.",
                    nameof(element));

            return newBudgetTransaction;
        }

        private static XElement ElementFromBudget(Entities.Budget budget)
        {
            var element = new XElement(XName.Get("Budget"),
                new XAttribute(XName.Get("name"), budget.Name),
                new XAttribute(XName.Get("period"), budget.Period),
                budget.Transactions.Select(ElementFromBudgetTransaction)
            );

            return element;
        }

        private static XElement ElementFromBudgetTransaction(Entities.BudgetTransaction budgetTransaction)
        {
            var element = new XElement(XName.Get("BudgetTransaction"),
                new XAttribute(XName.Get("credit"), budgetTransaction.CreditAccount.Name),
                new XAttribute(XName.Get("debit"), budgetTransaction.DebitAccount.Name),
                new XAttribute(XName.Get("amount"), budgetTransaction.Amount),
                new XAttribute(XName.Get("isInitial"), budgetTransaction.IsInitial),
                new XAttribute(XName.Get("isSurplus"), budgetTransaction.IsSurplus)
            );

            return element;
        }
    }
}
