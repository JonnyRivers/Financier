using Financier.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Financier.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        ILogger<AccountBalanceService> m_logger;
        FinancierDbContext m_dbContext;

        public AccountBalanceService(ILogger<AccountBalanceService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public decimal GetBalance(int accountId)
        {
            IEnumerable<int> logicalAccountIds = m_dbContext.AccountRelationships.Where(r => r.SourceAccountId == accountId && r.Type == AccountRelationshipType.PhysicalToLogical).Select(r => r.DestinationAccountId);
            var relevantAccountIds = new HashSet<int>(logicalAccountIds);
            relevantAccountIds.Add(accountId);

            IEnumerable<Transaction> creditTransactions = m_dbContext.Transactions.Where(t => relevantAccountIds.Contains(t.CreditAccountId));
            IEnumerable<Transaction> debitTransactions = m_dbContext.Transactions.Where(t => relevantAccountIds.Contains(t.DebitAccountId));

            decimal creditBalance = creditTransactions.Sum(t => t.Amount);
            decimal debitBalance = debitTransactions.Sum(t => t.Amount);
            decimal balance = creditBalance - debitBalance;

            return balance;
        }
    }
}
