using System.Collections.Generic;
using System.Linq;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class BudgetTransactionItemViewModel : IBudgetTransactionItemViewModel
    {
        private ILogger<BudgetTransactionItemViewModel> m_logger;
        private IAccountService m_accountService;

        public BudgetTransactionItemViewModel(ILogger<BudgetTransactionItemViewModel> logger, IAccountService accountService)
        {
            m_logger = logger;
            m_accountService = accountService;

            // TODO: pass this through!
            // TODO: do we need a lightweight version?  We are hitting transactions here when we don't need to.
            // Maybe we need an account link get all at the account service level, given how expensive it is to retrieve accounts
            IEnumerable<Account> accounts = m_accountService.GetAll();
            IEnumerable<IAccountLinkViewModel> accountLinks = accounts.Select(CreateAccountLink);
            AccountLinks = accountLinks;
        }

        public void Setup(BudgetTransaction budgetTransaction, BudgetTransactionType type)
        {
            Amount = budgetTransaction.Amount;
            BudgetTransactionId = budgetTransaction.BudgetTransactionId;
            SelectedCreditAccount = AccountLinks.Single(al => al.AccountId == budgetTransaction.CreditAccount.AccountId);
            SelectedDebitAccount = AccountLinks.Single(al => al.AccountId == budgetTransaction.DebitAccount.AccountId);
            Type = type;
        }

        public int BudgetTransactionId { get; set; }
        public BudgetTransactionType Type { get; set; }
        public IAccountLinkViewModel SelectedCreditAccount { get; set; }
        public IAccountLinkViewModel SelectedDebitAccount { get; set; }
        public decimal Amount { get; set; }

        public IEnumerable<IAccountLinkViewModel> AccountLinks { get; private set; }

        private static IAccountLinkViewModel CreateAccountLink(Account account)
        {
            IAccountLinkViewModel accountLinkViewModel = IoC.ServiceProvider.Instance.GetRequiredService<IAccountLinkViewModel>();
            accountLinkViewModel.Setup(account);

            return accountLinkViewModel;
        }
    }
}
