using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class TransactionCreateViewModel : TransactionDetailsBaseViewModel
    {
        private ILogger<TransactionCreateViewModel> m_logger;

        public TransactionCreateViewModel(
            ILogger<TransactionCreateViewModel> logger,
            IAccountService accountService,
            ITransactionService transactionService,
            IViewModelFactory viewModelFactory,
            Transaction hint) : base(accountService, transactionService, viewModelFactory, 0)
        {
            m_logger = logger;

            if (hint != null)
            {
                m_selectedCreditAccount = Accounts.Single(a => a.AccountId == hint.CreditAccount.AccountId);
                m_selectedDebitAccount = Accounts.Single(a => a.AccountId == hint.DebitAccount.AccountId);
            }
            else
            {
                m_selectedCreditAccount =
                    Accounts
                        .FirstOrDefault(a => a.Type == AccountType.Capital || a.Type == AccountType.Income);
                m_selectedDebitAccount =
                    Accounts
                        .FirstOrDefault(a => a.Type == AccountType.Asset || a.Type == AccountType.Expense);
            }

            m_amount = 0m;
            m_at = DateTime.Now;
        }

        protected override void OKExecute(object obj)
        {
            Transaction transaction = ToTransaction();

            m_transactionService.Create(transaction);
            m_transactionId = transaction.TransactionId;
        }
    }
}
