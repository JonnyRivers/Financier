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
                IEnumerable<Entities.Transaction> accountTransactionEntities = transactionEntities
                    .Where(t => t.CreditAccountId == accountEntity.AccountId || t.DebitAccountId == accountEntity.AccountId);

                var creditCardTrasactions = new List<CreditCardTransaction>();
                decimal balance = 0;
                foreach (Entities.Transaction accountTransactionEntity in accountTransactionEntities.OrderBy(t => t.At).ThenBy(t => t.TransactionId))
                {
                    decimal amount = 0;
                    string otherAccountName = String.Empty;
                    if (accountTransactionEntity.CreditAccountId == accountEntity.AccountId)
                    {
                        amount = accountTransactionEntity.Amount;
                        otherAccountName = accountTransactionEntity.DebitAccount.Name;
                    }
                    else if (accountTransactionEntity.DebitAccountId == accountEntity.AccountId)
                    {
                        amount = -accountTransactionEntity.Amount; 
                        otherAccountName = accountTransactionEntity.CreditAccount.Name;
                    }

                    balance += amount;

                    creditCardTrasactions.Add(
                        new CreditCardTransaction
                        {
                            Id = accountTransactionEntity.TransactionId,
                            At = accountTransactionEntity.At,
                            OtherAccountName = otherAccountName,
                            Amount = accountTransactionEntity.Amount,
                            RunningBalance = balance
                        }
                    );
                }

                apiResponse.Add(new CreditCard
                {
                    Id = accountEntity.AccountId,
                    Name = accountEntity.Name,
                    Balance = balance,
                    Transactions = creditCardTrasactions.OrderByDescending(t => t.At).ThenByDescending(t => t.Id)
                });
            }

            return apiResponse;
        }
    }
}