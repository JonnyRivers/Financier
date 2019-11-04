using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Financier.Web.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Financier.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditCardController : ControllerBase
    {
        private ILogger<CreditCardController> m_logger;
        private Entities.FinancierDbContext m_dbContext;

        public CreditCardController(ILoggerFactory loggerFactory, Entities.FinancierDbContext dbContext)
        {
            m_logger = loggerFactory.CreateLogger<CreditCardController>();
            m_dbContext = dbContext;
        }

        // GET api/creditcard
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreditCard>>> Get()
        {
            List<Entities.Account> accountEntities = await m_dbContext.Accounts
                .Where(a => a.SubType == AccountSubType.CreditCard)
                .OrderBy(a => a.AccountId)
                .ToListAsync();

            List<int> accountIds = accountEntities.Select(a => a.AccountId).ToList();

            List<Entities.Transaction> transactionEntities = await m_dbContext.Transactions
                .Include(t => t.CreditAccount)
                .Include(t => t.DebitAccount)
                .Where(t => accountIds.Contains(t.CreditAccountId) || accountIds.Contains(t.DebitAccountId))
                .ToListAsync();

            List<CreditCard> apiResponse = new List<CreditCard>();
            foreach (Entities.Account accountEntity in accountEntities)
            {
                List<Entities.Transaction> accountTransactionEntities = transactionEntities
                    .Where(t => t.CreditAccountId == accountEntity.AccountId || t.DebitAccountId == accountEntity.AccountId)
                    .ToList();

                decimal balance = 0;
                foreach (Entities.Transaction accountTransactionEntity in accountTransactionEntities.OrderBy(t => t.At))
                {
                    if (accountTransactionEntity.CreditAccountId == accountEntity.AccountId)
                        balance += accountTransactionEntity.Amount;
                    else if (accountTransactionEntity.DebitAccountId == accountEntity.AccountId)
                        balance -= accountTransactionEntity.Amount;
                }

                apiResponse.Add(new CreditCard
                {
                    Id = accountEntity.AccountId,
                    Name = accountEntity.Name,
                    Balance = balance
                });
            }

            return apiResponse;
        }

        // GET api/creditcard/{id}/transactions
        [HttpGet("{id}/transactions")]
        public async Task<ActionResult<IEnumerable<AccountTransaction>>> GetTransactions(int id)
        {
            List<Entities.Transaction> transactionEntities = await m_dbContext.Transactions
                .Include(t => t.CreditAccount)
                .Include(t => t.DebitAccount)
                .Where(t => t.CreditAccountId == id || t.DebitAccountId == id)
                .ToListAsync();

            decimal balance = 0;
            List<AccountTransaction> accountTransactions = new List<AccountTransaction>();
            foreach (Entities.Transaction transactionEntity in transactionEntities.OrderBy(t => t.At))
            {
                if (transactionEntity.CreditAccountId == id)
                    balance += transactionEntity.Amount;
                else if (transactionEntity.DebitAccountId == id)
                    balance -= transactionEntity.Amount;

                accountTransactions.Add(new AccountTransaction
                {
                    DebitAccountName = transactionEntity.DebitAccount.Name,
                    CreditAccountName = transactionEntity.CreditAccount.Name,
                    At = transactionEntity.At,
                    Amount = transactionEntity.Amount,
                    Balance = balance
                });
            }

            return accountTransactions.OrderByDescending(t => t.At).ToList();
        }
    }
}