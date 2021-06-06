using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Financier.Services
{
    public class AccountService : IAccountService
    {
        private ILogger<AccountService> m_logger;
        private Entities.FinancierDbContext m_dbContext;

        public AccountService(ILoggerFactory loggerFactory, Entities.FinancierDbContext dbContext)
        {
            m_logger = loggerFactory.CreateLogger<AccountService>();
            m_dbContext = dbContext;
        }

        public void Create(Account account)
        {
            var accountEntity = new Entities.Account
            {
                Name = account.Name,
                Type = account.Type,
                SubType = account.SubType,
                CurrencyId = account.Currency.CurrencyId
            };
            m_dbContext.Accounts.Add(accountEntity);
            m_dbContext.SaveChanges();

            account.AccountId = accountEntity.AccountId;
        }

        public Account Get(int accountId)
        {
            Entities.Account accountEntity = m_dbContext.Accounts
                .Include(a => a.Currency)
                .SingleOrDefault(a => a.AccountId == accountId);

            if (accountEntity == null)
                throw new ArgumentException($"No Account exists with AccountId {accountId}", nameof(accountId));

            return FromEntity(accountEntity);
        }

        public AccountLink GetAsLink(int accountId)
        {
            Entities.Account accountEntity = m_dbContext.Accounts
                .SingleOrDefault(a => a.AccountId == accountId);

            if (accountEntity == null)
                throw new ArgumentException($"No Account exists with AccountId {accountId}", nameof(accountId));

            return FromEntityToAccountLink(accountEntity);
        }

        public IEnumerable<Account> GetAll()
        {
            return m_dbContext.Accounts
                .Include(a => a.Currency)
                .Select(FromEntity)
                .ToList();
        }

        public IEnumerable<AccountLink> GetAllAsLinks()
        {
            return m_dbContext.Accounts
                .Select(FromEntityToAccountLink)
                .ToList();
        }

        public IEnumerable<Account> GetAllPhysical()
        {
            var logicalAccountIds = new HashSet<int>(
                m_dbContext.AccountRelationships
                    .Where(ar => ar.Type == AccountRelationshipType.PhysicalToLogical)
                    .Select(ar => ar.DestinationAccountId)
            );

            return m_dbContext.Accounts
                .Include(a => a.Currency)
                .Where(a => a.Type == AccountType.Asset || a.Type == AccountType.Liability)
                .Where(a => !logicalAccountIds.ToList().Contains(a.AccountId))
                .Select(FromEntity)
                .ToList();
        }

        public IEnumerable<int> GetLogicalAccountIds(int accountId)
        {
            return m_dbContext.AccountRelationships
                .Where(ar => ar.SourceAccountId == accountId &&
                                ar.Type == AccountRelationshipType.PhysicalToLogical)
                .Select(ar => ar.DestinationAccountId)
                .ToList();
        }

        public void Update(Account account)
        {
            Entities.Account accountEntity = m_dbContext.Accounts
                .Single(a => a.AccountId == account.AccountId);

            accountEntity.Name = account.Name;
            accountEntity.Type = account.Type;
            accountEntity.SubType = account.SubType;
            accountEntity.CurrencyId = account.Currency.CurrencyId;

            m_dbContext.SaveChanges();
        }

        public decimal GetBalance(int accountId, bool includeLogical)
        {
            return GetBalanceAt(accountId, DateTime.MaxValue, includeLogical);
        }

        public decimal GetBalanceAt(int accountId, DateTime at, bool includeLogical)
        {
            var allAccountIds = new HashSet<int>();
            allAccountIds.Add(accountId);
            if (includeLogical)
            {
                IEnumerable<int> logicalAccountIds = m_dbContext.AccountRelationships
                    .Where(r => r.SourceAccountId == accountId &&
                                r.Type == AccountRelationshipType.PhysicalToLogical)
                    .Select(r => r.DestinationAccountId);
                foreach(int logicalAccountId in logicalAccountIds)
                {
                    allAccountIds.Add(logicalAccountId);
                }
            }

            IEnumerable<Entities.Transaction> creditTransactions =
                m_dbContext.Transactions
                    .Where(t => allAccountIds.ToList().Contains(t.CreditAccountId) &&
                                t.At <= at);
            IEnumerable<Entities.Transaction> debitTransactions =
                m_dbContext.Transactions
                    .Where(t => allAccountIds.ToList().Contains(t.DebitAccountId) &&
                                t.At <= at);

            decimal creditBalance = creditTransactions.Sum(t => t.Amount);
            decimal debitBalance = debitTransactions.Sum(t => t.Amount);
            decimal balance = debitBalance - creditBalance;

            return balance;
        }

        private Account FromEntity(Entities.Account accountEntity)
        {
            var currency = new Currency
            {
                CurrencyId = accountEntity.Currency.CurrencyId,
                Name = accountEntity.Currency.Name,
                ShortName = accountEntity.Currency.ShortName,
                Symbol = accountEntity.Currency.Symbol,
                IsPrimary = accountEntity.Currency.IsPrimary
            };

            return new Account
            {
                AccountId = accountEntity.AccountId,
                Name = accountEntity.Name,
                Type = accountEntity.Type,
                SubType = accountEntity.SubType,
                Currency = currency
            };
        }

        // TODO: duplicated code
        private AccountLink FromEntityToAccountLink(
            Entities.Account accountEntity)
        {
            return new AccountLink
            {
                AccountId = accountEntity.AccountId,
                Name = accountEntity.Name,
                Type = accountEntity.Type,
                SubType = accountEntity.SubType
            };
        }
    }
}
