using Financier.Services;
using Financier.UnitTesting;
using Financier.UnitTesting.DbSetup;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Financier.Core.Tests
{
    [TestClass]
    public class DatabaseSerializationXmlServiceTests
    {
        [TestMethod]
        public void TestLoadValid()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Load("TestData/Valid.xml");

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Budget> budgets = sqliteMemoryWrapper.DbContext.Budgets.ToList();
                List<Entities.BudgetTransaction> budgetTransactions = sqliteMemoryWrapper.DbContext.BudgetTransactions.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();
                List<Entities.TransactionRelationship> transactionRelationships = sqliteMemoryWrapper.DbContext.TransactionRelationships.ToList();

                Assert.AreEqual(6, accounts.Count);
                Assert.AreEqual(2, accountRelationships.Count);
                Assert.AreEqual(2, budgets.Count);
                Assert.AreEqual(5, budgetTransactions.Count);
                Assert.AreEqual(2, currencies.Count);
                Assert.AreEqual(4, transactions.Count);
                Assert.AreEqual(1, transactionRelationships.Count);

                Entities.Currency usdCurrency = currencies.Single(c => c.Name == "US Dollar");
                Assert.AreEqual("US Dollar", usdCurrency.Name);
                Assert.AreEqual("USD", usdCurrency.ShortName);
                Assert.AreEqual("$", usdCurrency.Symbol);

                Entities.Currency gbpCurrency = currencies.Single(c => c.Name == "UK Sterling");
                Assert.AreEqual("UK Sterling", gbpCurrency.Name);
                Assert.AreEqual("GBP", gbpCurrency.ShortName);
                Assert.AreEqual("£", gbpCurrency.Symbol);

                Entities.Account checkingAccount = accounts.Single(c => c.Name == "Checking");
                Assert.AreEqual("Checking", checkingAccount.Name);
                Assert.AreEqual("USD", checkingAccount.Currency.ShortName);
                Assert.AreEqual(AccountType.Asset, checkingAccount.Type);
                Assert.AreEqual(AccountSubType.Checking, checkingAccount.SubType);

                Entities.Account savingsAccount = accounts.Single(c => c.Name == "Savings");
                Assert.AreEqual("Savings", savingsAccount.Name);
                Assert.AreEqual("USD", savingsAccount.Currency.ShortName);
                Assert.AreEqual(AccountType.Asset, savingsAccount.Type);
                Assert.AreEqual(AccountSubType.None, savingsAccount.SubType);

                Entities.Account incomeAccount = accounts.Single(c => c.Name == "Income");
                Assert.AreEqual("Income", incomeAccount.Name);
                Assert.AreEqual("USD", incomeAccount.Currency.ShortName);
                Assert.AreEqual(AccountType.Income, incomeAccount.Type);
                Assert.AreEqual(AccountSubType.None, incomeAccount.SubType);

                Entities.Account creditCardAccount = accounts.Single(c => c.Name == "Credit Card");
                Assert.AreEqual("Credit Card", creditCardAccount.Name);
                Assert.AreEqual("USD", creditCardAccount.Currency.ShortName);
                Assert.AreEqual(AccountType.Liability, creditCardAccount.Type);
                Assert.AreEqual(AccountSubType.CreditCard, creditCardAccount.SubType);

                Entities.Account rentPrepaymentAccount = accounts.Single(c => c.Name == "Rent Prepayment");
                Assert.AreEqual("Rent Prepayment", rentPrepaymentAccount.Name);
                Assert.AreEqual("USD", rentPrepaymentAccount.Currency.ShortName);
                Assert.AreEqual(AccountType.Asset, rentPrepaymentAccount.Type);
                Assert.AreEqual(AccountSubType.None, rentPrepaymentAccount.SubType);

                Entities.Account rentExpenseAccount = accounts.Single(c => c.Name == "Rent Expense");
                Assert.AreEqual("Rent Expense", rentExpenseAccount.Name);
                Assert.AreEqual("USD", rentExpenseAccount.Currency.ShortName);
                Assert.AreEqual(AccountType.Expense, rentExpenseAccount.Type);
                Assert.AreEqual(AccountSubType.None, rentExpenseAccount.SubType);

                // TODO: check the data in a way that is not order dependent

                //Assert.AreEqual("Checking", accountRelationships[0].SourceAccount.Name);
                //Assert.AreEqual("Rent Prepayment", accountRelationships[0].DestinationAccount.Name);
                //Assert.AreEqual(AccountRelationshipType.PhysicalToLogical, accountRelationships[0].Type);

                //Assert.AreEqual("Rent Prepayment", accountRelationships[1].SourceAccount.Name);
                //Assert.AreEqual("Rent Expense", accountRelationships[1].DestinationAccount.Name);
                //Assert.AreEqual(AccountRelationshipType.PrepaymentToExpense, accountRelationships[1].Type);

                //Assert.AreEqual("Income", transactions[0].CreditAccount.Name);
                //Assert.AreEqual(100m, transactions[0].Amount);
                //Assert.AreEqual("Checking", transactions[0].DebitAccount.Name);
                //Assert.AreEqual(new DateTime(2018, 1, 1, 9, 0, 0), transactions[0].At);

                //Assert.AreEqual("Checking", transactions[1].CreditAccount.Name);
                //Assert.AreEqual(50m, transactions[1].Amount);
                //Assert.AreEqual("Rent Prepayment", transactions[1].DebitAccount.Name);
                //Assert.AreEqual(new DateTime(2018, 1, 2, 8, 30, 0), transactions[1].At);

                //Assert.AreEqual("Credit Card", transactions[2].CreditAccount.Name);
                //Assert.AreEqual(30m, transactions[2].Amount);
                //Assert.AreEqual("Rent Expense", transactions[2].DebitAccount.Name);
                //Assert.AreEqual(new DateTime(2018, 1, 3, 8, 30, 0), transactions[2].At);

                //Assert.AreEqual("Rent Prepayment", transactions[3].CreditAccount.Name);
                //Assert.AreEqual(30m, transactions[3].Amount);
                //Assert.AreEqual("Credit Card", transactions[3].DebitAccount.Name);
                //Assert.AreEqual(new DateTime(2018, 1, 4, 8, 30, 0), transactions[3].At);

                //Assert.AreEqual(transactions[2].TransactionId, transactionRelationships[0].SourceTransactionId);
                //Assert.AreEqual(transactions[3].TransactionId, transactionRelationships[0].DestinationTransactionId);
                //Assert.AreEqual(TransactionRelationshipType.CreditCardPayment, transactionRelationships[0].Type);

                //Assert.AreEqual("The Budget", budgets[0].Name);
                //Assert.AreEqual(BudgetPeriod.Fortnightly, budgets[0].Period);

                //Assert.AreEqual("Income", budgetTransactions[0].CreditAccount.Name);
                //Assert.AreEqual(100m, budgetTransactions[0].Amount);
                //Assert.AreEqual("Checking", budgetTransactions[0].DebitAccount.Name);
                //Assert.AreEqual(true, budgetTransactions[0].IsInitial);
                //Assert.AreEqual(false, budgetTransactions[0].IsSurplus);
                //Assert.AreEqual(budgets[0].BudgetId, budgetTransactions[0].BudgetId);
                //Assert.AreEqual("Checking", budgetTransactions[1].CreditAccount.Name);
                //Assert.AreEqual(80m, budgetTransactions[1].Amount);
                //Assert.AreEqual("Rent Prepayment", budgetTransactions[1].DebitAccount.Name);
                //Assert.AreEqual(false, budgetTransactions[1].IsInitial);
                //Assert.AreEqual(false, budgetTransactions[1].IsSurplus);
                //Assert.AreEqual(budgets[0].BudgetId, budgetTransactions[1].BudgetId);
                //Assert.AreEqual("Checking", budgetTransactions[2].CreditAccount.Name);
                //Assert.AreEqual(0m, budgetTransactions[2].Amount);
                //Assert.AreEqual("Savings", budgetTransactions[2].DebitAccount.Name);
                //Assert.AreEqual(false, budgetTransactions[2].IsInitial);
                //Assert.AreEqual(true, budgetTransactions[2].IsSurplus);
                //Assert.AreEqual(budgets[0].BudgetId, budgetTransactions[2].BudgetId);

                //Assert.AreEqual("Another Budget", budgets[1].Name);
                //Assert.AreEqual(BudgetPeriod.Weekly, budgets[1].Period);

                //Assert.AreEqual("Income", budgetTransactions[3].CreditAccount.Name);
                //Assert.AreEqual(50m, budgetTransactions[3].Amount);
                //Assert.AreEqual("Checking", budgetTransactions[3].DebitAccount.Name);
                //Assert.AreEqual(true, budgetTransactions[3].IsInitial);
                //Assert.AreEqual(false, budgetTransactions[3].IsSurplus);
                //Assert.AreEqual(budgets[1].BudgetId, budgetTransactions[3].BudgetId);
                //Assert.AreEqual("Checking", budgetTransactions[4].CreditAccount.Name);
                //Assert.AreEqual(0m, budgetTransactions[4].Amount);
                //Assert.AreEqual("Savings", budgetTransactions[4].DebitAccount.Name);
                //Assert.AreEqual(false, budgetTransactions[4].IsInitial);
                //Assert.AreEqual(true, budgetTransactions[4].IsSurplus);
                //Assert.AreEqual(budgets[1].BudgetId, budgetTransactions[4].BudgetId);
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

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
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

            var currencyFactory = new CurrencyFactory();
            Entities.Currency usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Load(path);

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(0, accounts.Count);
                Assert.AreEqual(0, accountRelationships.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);

                Assert.AreEqual(usdCurrencyEntity.Name, currencies[0].Name);
                Assert.AreEqual(usdCurrencyEntity.ShortName, currencies[0].ShortName);
                Assert.AreEqual(usdCurrencyEntity.Symbol, currencies[0].Symbol);
            }
        }

        [TestMethod]
        public void TestReloadAccount()
        {
            const string path = "TestData/AccountReload.xml";
            ILoggerFactory loggerFactory = new LoggerFactory();

            var currencyFactory = new CurrencyFactory();
            Entities.Currency usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);

            var accountFactory = new AccountFactory();
            Entities.Account checkingAccountEntity =
                accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);

                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Load(path);

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(1, accounts.Count);
                Assert.AreEqual(0, accountRelationships.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);

                Assert.AreEqual(checkingAccountEntity.Name, accounts[0].Name);
                Assert.AreEqual(usdCurrencyEntity.ShortName, accounts[0].Currency.ShortName);
            }
        }

        [TestMethod]
        public void TestReloadAccountRelationship()
        {
            const string path = "TestData/AccountRelationshipReload.xml";
            ILoggerFactory loggerFactory = new LoggerFactory();

            var currencyFactory = new CurrencyFactory();
            var usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);

            var accountFactory = new AccountFactory();
            Entities.Account checkingAccountEntity = 
                accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
            Entities.Account rentPrepaymentAccountEntity = 
                accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);

            var accountRelationship = new Entities.AccountRelationship
            {
                SourceAccount = checkingAccountEntity,
                DestinationAccount = rentPrepaymentAccountEntity,
                Type = AccountRelationshipType.PhysicalToLogical
            };

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(accountRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Load(path);

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(2, accounts.Count);
                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);

                Assert.AreEqual(checkingAccountEntity.Name, accountRelationships[0].SourceAccount.Name);
                Assert.AreEqual(rentPrepaymentAccountEntity.Name, accountRelationships[0].DestinationAccount.Name);
                Assert.AreEqual(accountRelationship.Type, accountRelationships[0].Type);
            }
        }

        [TestMethod]
        public void TestReloadTransaction()
        {
            const string path = "TestData/TransactionReload.xml";
            ILoggerFactory loggerFactory = new LoggerFactory();

            var currencyFactory = new CurrencyFactory();
            Entities.Currency usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);

            var accountFactory = new AccountFactory();
            Entities.Account checkingAccountEntity =
                accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
            Entities.Account rentPrepaymentAccountEntity =
                accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);

            var accountRelationship = new Entities.AccountRelationship
            {
                SourceAccount = checkingAccountEntity,
                DestinationAccount = rentPrepaymentAccountEntity,
                Type = AccountRelationshipType.PhysicalToLogical
            };
            var transaction = new Entities.Transaction
            {
                CreditAccount = checkingAccountEntity,
                Amount = 10m,
                DebitAccount = rentPrepaymentAccountEntity,
                At = new DateTime(2018, 1, 1, 8, 30, 0)
            };

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(accountRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.Transactions.Add(transaction);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Load(path);

                List<Entities.Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Entities.AccountRelationship> accountRelationships = sqliteMemoryWrapper.DbContext.AccountRelationships.ToList();
                List<Entities.Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Entities.Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(2, accounts.Count);
                Assert.AreEqual(1, accountRelationships.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(1, transactions.Count);

                Assert.AreEqual(checkingAccountEntity.Name, transactions[0].CreditAccount.Name);
                Assert.AreEqual(transaction.Amount, transactions[0].Amount);
                Assert.AreEqual(rentPrepaymentAccountEntity.Name, transactions[0].DebitAccount.Name);
                Assert.AreEqual(transaction.At, transactions[0].At);
            }
        }

        [TestMethod]
        public void TestReloadBudget()
        {
            const string path = "TestData/BudgetReload.xml";
            ILoggerFactory loggerFactory = new LoggerFactory();

            var currencyFactory = new CurrencyFactory();
            Entities.Currency usdCurrencyEntity = currencyFactory.Create(CurrencyPrefab.Usd, true);

            var accountFactory = new AccountFactory();
            Entities.Account incomeAccountEntity =
                accountFactory.Create(AccountPrefab.Income, usdCurrencyEntity);
            Entities.Account checkingAccountEntity =
                accountFactory.Create(AccountPrefab.Checking, usdCurrencyEntity);
            Entities.Account rentPrepaymentAccountEntity =
                accountFactory.Create(AccountPrefab.RentPrepayment, usdCurrencyEntity);
            Entities.Account savingsAccountEntity =
                accountFactory.Create(AccountPrefab.Savings, usdCurrencyEntity);

            var accountRelationship = new Entities.AccountRelationship
            {
                SourceAccount = checkingAccountEntity,
                DestinationAccount = rentPrepaymentAccountEntity,
                Type = AccountRelationshipType.PhysicalToLogical
            };
            var budget = new Entities.Budget
            {
                Name = "The Budget",
                Period = BudgetPeriod.Fortnightly,
                Transactions = new List<Entities.BudgetTransaction>
                {
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = incomeAccountEntity,
                        DebitAccount = checkingAccountEntity,
                        Amount = 100m,
                        IsInitial = true
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = rentPrepaymentAccountEntity,
                        Amount = 80m,
                    },
                    new Entities.BudgetTransaction
                    {
                        CreditAccount = checkingAccountEntity,
                        DebitAccount = savingsAccountEntity,
                        Amount = 100m,
                        IsSurplus = true
                    }
                }
            };

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                currencyFactory.Add(sqliteMemoryWrapper.DbContext, usdCurrencyEntity);

                accountFactory.Add(sqliteMemoryWrapper.DbContext, incomeAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, checkingAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, rentPrepaymentAccountEntity);
                accountFactory.Add(sqliteMemoryWrapper.DbContext, savingsAccountEntity);

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(accountRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                sqliteMemoryWrapper.DbContext.Budgets.Add(budget);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Save(path);
            }

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
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

                Entities.BudgetTransaction initialTransaction = budgetTransactions.First(bt => bt.IsInitial && !bt.IsSurplus);
                Entities.BudgetTransaction regularTransaction = budgetTransactions.First(bt => !bt.IsInitial && !bt.IsSurplus);
                Entities.BudgetTransaction surplusTransaction = budgetTransactions.First(bt => !bt.IsInitial && bt.IsSurplus);
                Assert.AreEqual(budget.Transactions[0].DebitAccount.Name, initialTransaction.DebitAccount.Name);
                Assert.AreEqual(budget.Transactions[0].CreditAccount.Name, initialTransaction.CreditAccount.Name);
                Assert.AreEqual(budget.Transactions[0].Amount, initialTransaction.Amount);
                Assert.AreEqual(budget.Transactions[0].IsInitial, initialTransaction.IsInitial);
                Assert.AreEqual(budget.Transactions[0].IsSurplus, initialTransaction.IsSurplus);
                Assert.AreEqual(budget.Transactions[1].DebitAccount.Name, regularTransaction.DebitAccount.Name);
                Assert.AreEqual(budget.Transactions[1].CreditAccount.Name, regularTransaction.CreditAccount.Name);
                Assert.AreEqual(budget.Transactions[1].Amount, regularTransaction.Amount);
                Assert.AreEqual(budget.Transactions[1].IsInitial, regularTransaction.IsInitial);
                Assert.AreEqual(budget.Transactions[1].IsSurplus, regularTransaction.IsSurplus);
                Assert.AreEqual(budget.Transactions[2].DebitAccount.Name, surplusTransaction.DebitAccount.Name);
                Assert.AreEqual(budget.Transactions[2].CreditAccount.Name, surplusTransaction.CreditAccount.Name);
                Assert.AreEqual(budget.Transactions[2].Amount, surplusTransaction.Amount);
                Assert.AreEqual(budget.Transactions[2].IsInitial, surplusTransaction.IsInitial);
                Assert.AreEqual(budget.Transactions[2].IsSurplus, surplusTransaction.IsSurplus);
            }
        }

        private void TestLoadFailure(string path)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var service = new DatabaseSerializationXmlService(loggerFactory, sqliteMemoryWrapper.DbContext);
                service.Load(path);
            }
        }
    }
}
