using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Financier.Web.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Financier.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesController : ControllerBase
    {
        private readonly ILogger<ExpensesController> m_logger;
        private readonly Entities.FinancierDbContext m_dbContext;

        // TODO: put this back!
        private readonly DateTime m_minTransactionAt = new DateTime(2000, 2, 13, 8, 48, 0, DateTimeKind.Utc);

        public ExpensesController(ILoggerFactory loggerFactory, Entities.FinancierDbContext dbContext)
        {
            m_logger = loggerFactory.CreateLogger<ExpensesController>();
            m_dbContext = dbContext;
        }

        // GET api/expenses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExpenseAccount>>> Get()
        {
            List<Entities.Account> accountEntities = await m_dbContext.Accounts.ToListAsync();
            Dictionary<int, Entities.Account> accountEntitiesById = accountEntities.ToDictionary(a => a.AccountId, a => a);

            List<Entities.AccountRelationship> accountRelationships = await m_dbContext.AccountRelationships
                .ToListAsync();
            List<Entities.AccountRelationship> prepaymentExpenseRelationships = accountRelationships
                .Where(ar => ar.Type == AccountRelationshipType.PrepaymentToExpense)
                .ToList();
            Dictionary<int, int> expenseAccountIdByPrepaymentAccountId =
                prepaymentExpenseRelationships.ToDictionary(p => p.SourceAccountId, p => p.DestinationAccountId);

            List<Entities.AccountRelationship> physicalToLogicalRelationships = accountRelationships
                .Where(ar => ar.Type == AccountRelationshipType.PhysicalToLogical)
                .ToList();
            Dictionary<int, int> physicalAccountIdByLogicalAccountId =
                physicalToLogicalRelationships.ToDictionary(p => p.DestinationAccountId, p => p.SourceAccountId);

            // cache all prepayment transactions that were credit card payoff transactions
            List<Entities.TransactionRelationship> creditCardPaymentRelationships = await m_dbContext.TransactionRelationships
                .Where(tr => tr.Type == TransactionRelationshipType.CreditCardPayment)
                .ToListAsync();
            Dictionary<int, int> expenseTransactionIdsByPrepaymentTransactionId =
                creditCardPaymentRelationships.ToDictionary(p => p.DestinationTransactionId, p => p.SourceTransactionId);

            var interestingAccountIds = new List<int>();
            interestingAccountIds.AddRange(expenseAccountIdByPrepaymentAccountId.Keys);
            interestingAccountIds.AddRange(expenseAccountIdByPrepaymentAccountId.Values);

            List<Entities.Transaction> transactionEntities = await m_dbContext.Transactions
                .Include(t => t.CreditAccount)
                .Include(t => t.DebitAccount)
                .Where(t => interestingAccountIds.Contains(t.CreditAccountId) || interestingAccountIds.Contains(t.DebitAccountId))
                .ToListAsync();

            var apiResponse = new List<ExpenseAccount>();
            foreach (Entities.Account accountEntity in accountEntities.Where(a => expenseAccountIdByPrepaymentAccountId.ContainsKey(a.AccountId)))
            {
                int prepaymentAccountId = accountEntity.AccountId;
                int expenseAccountId = expenseAccountIdByPrepaymentAccountId[prepaymentAccountId];
                int physicalAccountId = physicalAccountIdByLogicalAccountId[prepaymentAccountId];

                List<Entities.Transaction> prepaymentAccountTransactionEntities = transactionEntities
                    .Where(t => t.CreditAccountId == prepaymentAccountId || t.DebitAccountId == prepaymentAccountId)
                    .ToList();

                // Process prepayment transactions
                
                var expenseTransactions = new List<ExpenseTransaction>();
                foreach (Entities.Transaction accountTransactionEntity in prepaymentAccountTransactionEntities)
                {
                    int id = accountTransactionEntity.TransactionId;
                    DateTime at = accountTransactionEntity.At;
                    decimal amount = 0;
                    int otherAccountId = 0;
                    
                    if (accountTransactionEntity.DebitAccountId == expenseAccountId &&
                       accountTransactionEntity.CreditAccountId == prepaymentAccountId)
                    {
                        // direct payment from prepayment to expense
                        otherAccountId = physicalAccountId;
                        amount = -accountTransactionEntity.Amount;
                    }
                    else if (accountTransactionEntity.CreditAccountId == expenseAccountId &&
                       accountTransactionEntity.DebitAccountId == prepaymentAccountId)
                    {
                        // direct payment from expense to prepayment (rare but must be handled)

                        otherAccountId = physicalAccountId;
                        amount = accountTransactionEntity.Amount;
                    }
                    else if (accountTransactionEntity.DebitAccountId == prepaymentAccountId)
                    {
                        // a prepament account transaction
                        otherAccountId = accountTransactionEntity.CreditAccountId;
                        amount = accountTransactionEntity.Amount;
                    }
                    else if (accountTransactionEntity.CreditAccountId == prepaymentAccountId)
                    {
                        // a prepament account transaction
                        otherAccountId = accountTransactionEntity.DebitAccountId;
                        amount = -accountTransactionEntity.Amount;
                    }

                    // lookup matching expense transaction & substitute details (as they are typically more accurate)
                    if(expenseTransactionIdsByPrepaymentTransactionId.ContainsKey(accountTransactionEntity.TransactionId))
                    {
                        Entities.Transaction expenseTransaction = transactionEntities
                            .Single(e => e.TransactionId == expenseTransactionIdsByPrepaymentTransactionId[accountTransactionEntity.TransactionId]);

                        // TODO: update At & update 'other account'
                        if (expenseTransaction.DebitAccountId == expenseAccountId)
                        {
                            id = expenseTransaction.TransactionId;
                            at = expenseTransaction.At;
                            otherAccountId = expenseTransaction.CreditAccountId;
                        }
                        else if (accountTransactionEntity.CreditAccountId == expenseAccountId)
                        {
                            id = expenseTransaction.TransactionId;
                            at = expenseTransaction.At;
                            otherAccountId = expenseTransaction.DebitAccountId;
                        }
                        else
                        {
                            // Something very strange happened...
                        }
                    }

                    expenseTransactions.Add(
                        new ExpenseTransaction
                        {
                            Id = id,
                            At = at,
                            Amount = amount,
                            Balance = -666,// to make it clear this is wrong
                            OtherAccountName = accountEntitiesById[otherAccountId].Name
                        }
                    );
                }

                // TODO: look for recent expense only transactions

                IEnumerable<Entities.Transaction> orderedPrepaymentAccountTransactionEntities =
                    prepaymentAccountTransactionEntities.OrderBy(t => t.At).ThenBy(t => t.TransactionId);

                // TODO: calculate balance
                decimal balance = 0;
                foreach(ExpenseTransaction expenseTransaction in expenseTransactions.OrderBy(t => t.At).ThenBy(t => t.Id))
                {
                    balance += expenseTransaction.Amount;
                    expenseTransaction.Balance = balance;
                }

                if (balance == 0 && expenseTransactions.Count == 0)
                    continue;

                List<ExpenseTransaction> recentTransactions = expenseTransactions.Where(et => et.At >= m_minTransactionAt).ToList();

                apiResponse.Add(new ExpenseAccount
                {
                    Id = accountEntity.AccountId,
                    Name = accountEntity.Name,
                    Balance = balance,
                    RecentTransactions = recentTransactions
                });
            }

            return apiResponse;
        }
    }
}