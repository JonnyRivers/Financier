using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class BudgetTransactionItemViewModelFactory : IBudgetTransactionItemViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;

        public BudgetTransactionItemViewModelFactory(ILoggerFactory loggerFactory)
        {
            m_loggerFactory = loggerFactory;
        }

        public IBudgetTransactionItemViewModel Create(
            ObservableCollection<IAccountLinkViewModel> accountLinks, 
            BudgetTransaction budgetTransaction, 
            BudgetTransactionType type)
        {
            return new BudgetTransactionItemViewModel(
                m_loggerFactory,
                accountLinks, 
                budgetTransaction, 
                type);
        }
    }

    public class BudgetTransactionItemViewModel : BaseViewModel, IBudgetTransactionItemViewModel
    {
        private ILogger<BudgetTransactionItemViewModel> m_logger;

        private decimal m_amount;

        public BudgetTransactionItemViewModel(
            ILoggerFactory loggerFactory,
            ObservableCollection<IAccountLinkViewModel> accountLinks,
            BudgetTransaction budgetTransaction,
            BudgetTransactionType type)
        {
            m_logger = loggerFactory.CreateLogger<BudgetTransactionItemViewModel>();

            AccountLinks = accountLinks;
            m_amount = budgetTransaction.Amount;
            BudgetTransactionId = budgetTransaction.BudgetTransactionId;
            SelectedCreditAccount = AccountLinks.Single(al => al.AccountId == budgetTransaction.CreditAccount.AccountId);
            SelectedDebitAccount = AccountLinks.Single(al => al.AccountId == budgetTransaction.DebitAccount.AccountId);
            Type = type;
        }

        public BudgetTransaction ToBudgetTransaction()
        {
            var budgetTransaction = new BudgetTransaction
            {
                BudgetTransactionId = BudgetTransactionId,
                CreditAccount = SelectedCreditAccount.ToAccountLink(),
                DebitAccount = SelectedDebitAccount.ToAccountLink(),
                Amount = m_amount
            };

            return budgetTransaction;
        }

        public int BudgetTransactionId { get; set; }
        public BudgetTransactionType Type { get; set; }
        public IAccountLinkViewModel SelectedCreditAccount { get; set; }
        public IAccountLinkViewModel SelectedDebitAccount { get; set; }
        public decimal Amount
        {
            get { return m_amount; }
            set
            {
                if (value != m_amount)
                {
                    m_amount = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<IAccountLinkViewModel> AccountLinks { get; private set; }
        public bool AmountIsReadOnly
        {
            get => Type == BudgetTransactionType.Surplus;
        }
    }
}
