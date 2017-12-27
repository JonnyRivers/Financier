using Financier.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                    DebitAmount = 10m
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
    }
}
