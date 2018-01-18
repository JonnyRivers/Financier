using System.Collections.Generic;
using System.Linq;
using Financier.Data;
using Microsoft.Extensions.Logging;

namespace Financier.Services
{
    public class AccountService : IAccountService
    {
        private ILogger<AccountService> m_logger;
        private FinancierDbContext m_dbContext;

        public AccountService(ILogger<AccountService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Create(Account account)
        {
            m_dbContext.Accounts.Add(account);
            m_dbContext.SaveChanges();
        }

        public Account Get(int accountId)
        {
            return m_dbContext.Accounts.Single(a => a.AccountId == accountId);
        }

        public decimal GetBalance(int accountId)
        {
            IEnumerable<int> logicalAccountIds = m_dbContext.AccountRelationships
                .Where(r => r.SourceAccountId == accountId && r.Type == AccountRelationshipType.PhysicalToLogical)
                .Select(r => r.DestinationAccountId);
            var relevantAccountIds = new HashSet<int>(logicalAccountIds);
            relevantAccountIds.Add(accountId);

            IEnumerable<Transaction> creditTransactions = m_dbContext.Transactions.Where(t => relevantAccountIds.Contains(t.CreditAccountId));
            IEnumerable<Transaction> debitTransactions = m_dbContext.Transactions.Where(t => relevantAccountIds.Contains(t.DebitAccountId));

            decimal creditBalance = creditTransactions.Sum(t => t.Amount);
            decimal debitBalance = debitTransactions.Sum(t => t.Amount);
            decimal balance = debitBalance - creditBalance;

            return balance;
        }

        public IEnumerable<Account> GetAll()
        {
            return m_dbContext.Accounts;
        }

        public void Update(Account account)
        {
            // TODO: this is problematic as other changes related to retrieved records will be saved

            m_dbContext.SaveChanges();
        }
    }
}
