using Financier.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Tests
{
    [TestClass]
    public class FinancierDbContextTests
    {
        [TestMethod]
        public void TestCreateInMemory()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                Assert.AreEqual(0, sqliteMemoryWrapper.DbContext.Accounts.Count());
                Assert.AreEqual(0, sqliteMemoryWrapper.DbContext.Currencies.Count());
                Assert.AreEqual(0, sqliteMemoryWrapper.DbContext.Transactions.Count());
            }
        }

        [TestMethod]
        public void TestCreateCurrency()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrency = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$"
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(0, accounts.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);
            }
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrency = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$"
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingAccount = new Account
                {
                    Name = "Checking",
                    Currency = usdCurrency
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(1, accounts.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(0, transactions.Count);
                Assert.AreEqual(checkingAccount.Currency.CurrencyId, accounts[0].Currency.CurrencyId);
            }
        }

        [TestMethod]
        public void TestCreateTransaction()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrency = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$"
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var incomeAccount = new Account
                {
                    Name = "Income",
                    Currency = usdCurrency
                };
                var checkingAccount = new Account
                {
                    Name = "Checking",
                    Currency = usdCurrency
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transaction = new Transaction
                {
                    CreditAccount = incomeAccount,
                    CreditAmount = 10m,
                    DebitAccount = checkingAccount,
                    DebitAmount = 10m,
                    At = DateTime.Now
                };

                sqliteMemoryWrapper.DbContext.Transactions.Add(transaction);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(2, accounts.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(1, transactions.Count);
                Assert.AreEqual(transaction.CreditAccount.AccountId, transactions[0].CreditAccount.AccountId);
                Assert.AreEqual(transaction.CreditAmount, transactions[0].CreditAmount);
                Assert.AreEqual(transaction.DebitAccount.AccountId, transactions[0].DebitAccount.AccountId);
                Assert.AreEqual(transaction.DebitAmount, transactions[0].DebitAmount);
            }
        }

        [TestMethod]
        public void TestCreateAcountRelationships()
        {
            using (var sqliteMemoryWrapper = new SqliteMemoryWrapper())
            {
                var usdCurrency = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$"
                };

                sqliteMemoryWrapper.DbContext.Currencies.Add(usdCurrency);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var incomeAccount = new Account
                {
                    Name = "Income",
                    Currency = usdCurrency
                };
                var checkingAccount = new Account
                {
                    Name = "Checking",
                    Currency = usdCurrency
                };
                var creditCardAccount = new Account
                {
                    Name = "Credit Card",
                    Currency = usdCurrency
                };
                var rentPrepaymentAccount = new Account
                {
                    Name = "Rent Prepayment",
                    Currency = usdCurrency
                };
                var rentExpenseAccount = new Account
                {
                    Name = "Rent Expense",
                    Currency = usdCurrency
                };

                sqliteMemoryWrapper.DbContext.Accounts.Add(incomeAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(checkingAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(creditCardAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentPrepaymentAccount);
                sqliteMemoryWrapper.DbContext.Accounts.Add(rentExpenseAccount);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var checkingToRentPrepaymentRelationship = new AccountRelationship
                {
                    SourceAcount = checkingAccount,
                    DestinationAccount = rentPrepaymentAccount,
                    Type = AccountRelationshipType.PhysicalToLogical
                };
                var rentPrepaymentToExpenseRelationship = new AccountRelationship
                {
                    SourceAcount = rentPrepaymentAccount,
                    DestinationAccount = rentExpenseAccount,
                    Type = AccountRelationshipType.PrepaymentToExpense
                };

                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(checkingToRentPrepaymentRelationship);
                sqliteMemoryWrapper.DbContext.AccountRelationships.Add(rentPrepaymentToExpenseRelationship);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                var transactionsToAdd = new Transaction[]
                {
                    new Transaction
                    {
                        CreditAccount = incomeAccount,
                        CreditAmount = 10m,
                        DebitAccount = checkingAccount,
                        DebitAmount = 10m,
                        At = new DateTime(2018, 1, 1)
                    },
                    new Transaction
                    {
                        CreditAccount = checkingAccount,
                        CreditAmount = 5m,
                        DebitAccount = rentPrepaymentAccount,
                        DebitAmount = 5m,
                        At = new DateTime(2018, 1, 1)
                    },
                    new Transaction
                    {
                        CreditAccount = incomeAccount,
                        CreditAmount = 10m,
                        DebitAccount = checkingAccount,
                        DebitAmount = 10m,
                        At = new DateTime(2018, 1, 15)
                    },
                    new Transaction
                    {
                        CreditAccount = checkingAccount,
                        CreditAmount = 5m,
                        DebitAccount = rentPrepaymentAccount,
                        DebitAmount = 5m,
                        At = new DateTime(2018, 1, 15)
                    },
                    new Transaction
                    {
                        CreditAccount = rentPrepaymentAccount,
                        CreditAmount = 10m,
                        DebitAccount = rentExpenseAccount,
                        DebitAmount = 10m,
                        At = new DateTime(2018, 1, 25)
                    }
                };

                sqliteMemoryWrapper.DbContext.Transactions.AddRange(transactionsToAdd);
                sqliteMemoryWrapper.DbContext.SaveChanges();

                List<Account> accounts = sqliteMemoryWrapper.DbContext.Accounts.ToList();
                List<Currency> currencies = sqliteMemoryWrapper.DbContext.Currencies.ToList();
                List<Transaction> transactions = sqliteMemoryWrapper.DbContext.Transactions.ToList();

                Assert.AreEqual(5, accounts.Count);
                Assert.AreEqual(1, currencies.Count);
                Assert.AreEqual(5, transactions.Count);

                decimal incomeBalance = GetAccountBalance(sqliteMemoryWrapper.DbContext, incomeAccount.AccountId);
                decimal checkingAccountBalance = GetAccountBalance(sqliteMemoryWrapper.DbContext, checkingAccount.AccountId);
                decimal creditCardBalance = GetAccountBalance(sqliteMemoryWrapper.DbContext, creditCardAccount.AccountId);
                decimal rentPrepaymentBalance = GetAccountBalance(sqliteMemoryWrapper.DbContext, rentPrepaymentAccount.AccountId);
                decimal rentExpenseBalance = GetAccountBalance(sqliteMemoryWrapper.DbContext, rentExpenseAccount.AccountId);

                Assert.AreEqual(20m, incomeBalance);
                Assert.AreEqual(-10m, checkingAccountBalance);
                Assert.AreEqual(0m, creditCardBalance);
                Assert.AreEqual(0m, rentPrepaymentBalance);
                Assert.AreEqual(-10m, rentExpenseBalance);
            }
        }

        private static decimal GetAccountBalance(FinancierDbContext dbContext, int accountId)
        {
            IEnumerable<Transaction> creditTransactions = dbContext.Transactions.Where(t => t.CreditAccountId == accountId);
            IEnumerable<Transaction> debitTransactions = dbContext.Transactions.Where(t => t.DebitAccountId == accountId);

            decimal creditBalance = creditTransactions.Sum(t => t.CreditAmount);
            decimal debitBalance = debitTransactions.Sum(t => t.DebitAmount);
            decimal balance = creditBalance - debitBalance;

            return balance;
        }
    }
}
