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
            m_dbContext.Database.EnsureDeleted();
            m_logger.LogInformation("Database is obliterated");

            m_dbContext.Database.EnsureCreated();
            m_logger.LogInformation("Database is created");

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

            List<XElement> transactionElements = rootElement.Elements(XName.Get("Transaction")).ToList();
            List<Entities.Transaction> transactions = transactionElements.Select(e => TransactionFromElement(e, accountsByName)).ToList();
            m_dbContext.Transactions.AddRange(transactions);
            m_dbContext.SaveChanges();

            // TODO: Clean up Budget->XML by using the Transactions nav property of Entities.Budget
            // https://github.com/JonnyRivers/Financier/issues/13
            List<XElement> budgetElements = rootElement.Elements(XName.Get("Budget")).ToList();
            List<Entities.Budget> budgets = budgetElements.Select(BudgetFromElement).ToList();
            m_dbContext.Budgets.AddRange(budgets);
            m_dbContext.SaveChanges();
            Dictionary<string, Entities.Budget> budgetsByName = budgets.ToDictionary(b => b.Name, b => b);

            List<Entities.BudgetTransaction> budgetTransactions = budgetElements
                .SelectMany(e => BudgetTransactionsFromElement(e, accountsByName, budgetsByName))
                .ToList();
            m_dbContext.BudgetTransactions.AddRange(budgetTransactions);
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
                Type = (Entities.AccountType)Enum.Parse(typeof(Entities.AccountType), GetRequiredAttribute(element, "type").Value)
            };

            return account;
        }

        private static XElement ElementFromAccount(Entities.Account account)
        {
            var element = new XElement(XName.Get("Account"),
                new XAttribute(XName.Get("name"), account.Name),
                new XAttribute(XName.Get("currency"), account.Currency.ShortName),
                new XAttribute(XName.Get("type"), account.Type)
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
                Type = (Entities.AccountRelationshipType)Enum.Parse(typeof(Entities.AccountRelationshipType), type)
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

        private static Entities.Transaction TransactionFromElement(XElement element, Dictionary<string, Entities.Account> accountsByShortName)
        {
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

            return transaction;
        }

        private static XElement ElementFromTransaction(Entities.Transaction transaction)
        {
            var element = new XElement(XName.Get("Transaction"),
                new XAttribute(XName.Get("at"), transaction.At),
                new XAttribute(XName.Get("credit"), transaction.CreditAccount.Name),
                new XAttribute(XName.Get("amount"), transaction.Amount),
                new XAttribute(XName.Get("debit"), transaction.DebitAccount.Name)
            );

            return element;
        }

        private static Entities.Budget BudgetFromElement(XElement element)
        {
            string name = GetRequiredAttribute(element, "name").Value;
            string period = GetRequiredAttribute(element, "period").Value;

            var budget = new Entities.Budget
            {
                Name = name,
                Period = (Entities.BudgetPeriod)Enum.Parse(typeof(Entities.BudgetPeriod), period)
            };

            return budget;
        }

        private static List<Entities.BudgetTransaction> BudgetTransactionsFromElement(
            XElement element,
            Dictionary<string, Entities.Account> accountsByName,
            Dictionary<string, Entities.Budget> budgetsByName)
        {
            string budgetName = GetRequiredAttribute(element, "name").Value;
            int budgetId = budgetsByName[budgetName].BudgetId;

            bool initialTransactionFound = false;
            bool surplusTransactionFound = false;

            var budgetTransactions = new List<Entities.BudgetTransaction>();
            foreach (XElement budgetTransactionElement in element.Elements(XName.Get(nameof(BudgetTransaction))))
            {
                string creditAccountName = GetRequiredAttribute(budgetTransactionElement, "credit").Value;
                string debitAccountName = GetRequiredAttribute(budgetTransactionElement, "debit").Value;
                string amount = GetRequiredAttribute(budgetTransactionElement, "amount").Value;
                bool isInitial = AttributeExistsAndIsTrue(budgetTransactionElement, "isInitial");
                bool isSurplus = AttributeExistsAndIsTrue(budgetTransactionElement, "isSurplus");

                var newBudgetTransaction = new Entities.BudgetTransaction
                {
                    CreditAccountId = accountsByName[creditAccountName].AccountId,
                    DebitAccountId = accountsByName[debitAccountName].AccountId,
                    Amount = Decimal.Parse(amount),
                    IsInitial = isInitial,
                    IsSurplus = isSurplus,
                    BudgetId = budgetId
                };

                if(newBudgetTransaction.IsInitial && newBudgetTransaction.IsSurplus)
                    throw new ArgumentException(
                        nameof(element), 
                        "Budget contains transaction with initial and surplus flags set");

                initialTransactionFound |= newBudgetTransaction.IsInitial;
                surplusTransactionFound |= newBudgetTransaction.IsSurplus;

                budgetTransactions.Add(newBudgetTransaction);
            }

            if (!initialTransactionFound)
                throw new ArgumentException(nameof(element), "Budget element has no initial transaction");
            if (!surplusTransactionFound)
                throw new ArgumentException(nameof(element), "Budget element has no surplus transaction");

            return budgetTransactions;
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
