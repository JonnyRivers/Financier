using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Financier.Services
{
    public class AccountService : IAccountService
    {
        private ILogger<AccountService> m_logger;
        private Entities.FinancierDbContext m_dbContext;

        public AccountService(ILogger<AccountService> logger, Entities.FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Create(Account account)
        {
            var accountEntity = new Entities.Account
            {
                Name = account.Name,
                Type = (Entities.AccountType)account.Type,
                CurrencyId = account.Currency.CurrencyId
            };
            m_dbContext.Accounts.Add(accountEntity);
            m_dbContext.SaveChanges();

            account.AccountId = accountEntity.AccountId;
        }

        public Account Get(int accountId)
        {
            Entities.Account accountEntity = m_dbContext.Accounts.Single(a => a.AccountId == accountId);
            return FromEntity(accountEntity);
        }

        public IEnumerable<Account> GetAll()
        {
            return m_dbContext.Accounts.Select(FromEntity);
        }

        public void Update(Account account)
        {
            Entities.Account accountEntity = m_dbContext.Accounts
                .Single(a => a.AccountId == account.AccountId);

            accountEntity.Name = account.Name;
            accountEntity.Type = (Entities.AccountType)account.Type;
            accountEntity.CurrencyId = account.Currency.CurrencyId;

            m_dbContext.SaveChanges();
        }

        private Account FromEntity(Entities.Account accountEntity)
        {
            // Get all logical accounts
            List<int> logicalAccountIds = m_dbContext.AccountRelationships
                .Where(r => r.SourceAccountId == accountEntity.AccountId && 
                            r.Type == Entities.AccountRelationshipType.PhysicalToLogical)
                .Select(r => r.DestinationAccountId)
                .ToList();
            List<Account> logicalAccounts = logicalAccountIds.Select(id => Get(id)).ToList();

            IEnumerable<Entities.Transaction> creditTransactions =
                m_dbContext.Transactions
                    .Where(t => t.CreditAccountId == accountEntity.AccountId);
            IEnumerable<Entities.Transaction> debitTransactions =
                m_dbContext.Transactions
                    .Where(t => t.DebitAccountId == accountEntity.AccountId);

            decimal creditBalance = creditTransactions.Sum(t => t.Amount);
            decimal debitBalance = debitTransactions.Sum(t => t.Amount);
            decimal balance = debitBalance - creditBalance;

            decimal totalBalance = balance + logicalAccounts.Sum(a => a.Balance);

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
                Type = (AccountType)accountEntity.Type,
                Currency = currency,
                LogicalAccounts = logicalAccounts,
                Balance = totalBalance
            };
        }
    }
}
