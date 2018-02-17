using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Financier.Services
{
    public class CashflowService : ICashflowService
    {
        ILogger<CashflowService> m_logger;
        IAccountRelationshipService m_accountRelationshipService;
        IAccountService m_accountService;
        ICurrencyService m_currencyService;
        ITransactionService m_transactionService;

        public CashflowService(
            ILogger<CashflowService> logger,
            IAccountRelationshipService accountRelationshipService,
            IAccountService accountService,
            ICurrencyService currencyService,
            ITransactionService transactionService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;
            m_currencyService = currencyService;
            m_transactionService = transactionService;
        }

        public CashflowStatement Generate(DateTime startAt, DateTime endAt)
        {
            Currency primaryCurrency = m_currencyService.GetPrimary();

            List<AccountRelationship> prepaymentToExpenseRelationships = 
                m_accountRelationshipService
                    .GetAll()
                    .Where(ar => ar.Type == AccountRelationshipType.PrepaymentToExpense)
                    .ToList();

            var relevantAccountIds = new HashSet<int>(
                prepaymentToExpenseRelationships.SelectMany(
                    r => new int[2] {
                        r.SourceAccount.AccountId,
                        r.DestinationAccount.AccountId
                    }
                )
            );

            List<Transaction> relevantTransactions = m_transactionService.GetAll(relevantAccountIds).ToList();

            var cashflowStatementItems = new List<CashflowStatementItem>();
            foreach (AccountRelationship prepaymentToExpenseRelationship in prepaymentToExpenseRelationships)
            {
                
            }

            return new CashflowStatement(
                startAt, 
                endAt, 
                primaryCurrency.Symbol, 
                cashflowStatementItems
            );
        }
    }
}
