using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class TransactionEditViewModel : TransactionDetailsBaseViewModel
    {
        private ILogger<TransactionEditViewModel> m_logger;

        public TransactionEditViewModel(
            ILogger<TransactionEditViewModel> logger,
            IAccountService accountService,
            ITransactionService transactionService,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            int transactionId) : base(accountService, transactionService, accountLinkViewModelFactory, transactionId)
        {
            m_logger = logger;

            Transaction transaction = m_transactionService.Get(m_transactionId);

            m_selectedCreditAccount = Accounts.Single(a => a.AccountId == transaction.CreditAccount.AccountId);
            m_selectedDebitAccount = Accounts.Single(a => a.AccountId == transaction.DebitAccount.AccountId);
            m_amount = transaction.Amount;
            m_at = transaction.At;
        }

        protected override void OKExecute(object obj)
        {
            Transaction transaction = ToTransaction();

            m_transactionService.Update(transaction);
        }
    }
}
