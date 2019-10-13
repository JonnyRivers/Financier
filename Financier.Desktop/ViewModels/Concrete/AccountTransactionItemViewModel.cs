using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class AccountTransactionItemViewModelFactory : IAccountTransactionItemViewModelFactory
    {
        private readonly ILogger<AccountTransactionItemViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public AccountTransactionItemViewModelFactory(
            ILogger<AccountTransactionItemViewModelFactory> logger, 
            IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IAccountTransactionItemViewModel Create(Transaction transaction)
        {
            return m_serviceProvider.CreateInstance<AccountTransactionItemViewModel>(transaction);
        }
    }

    public class AccountTransactionItemViewModel : BaseViewModel, IAccountTransactionItemViewModel
    {
        private ILogger<AccountTransactionItemViewModel> m_logger;
        private IAccountLinkViewModelFactory m_accountLinkViewModelFactory;

        private decimal m_balance;

        public AccountTransactionItemViewModel(
            ILogger<AccountTransactionItemViewModel> logger,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            Transaction transaction)
        {
            m_logger = logger;
            m_accountLinkViewModelFactory = accountLinkViewModelFactory;

            m_balance = 0;

            TransactionId = transaction.TransactionId;
            CreditAccount = m_accountLinkViewModelFactory.Create(transaction.CreditAccount);
            DebitAccount = m_accountLinkViewModelFactory.Create(transaction.DebitAccount);
            At = transaction.At;
            Amount = transaction.Amount;
        }

        public int TransactionId { get; private set; }
        public IAccountLinkViewModel CreditAccount { get; private set; }
        public IAccountLinkViewModel DebitAccount { get; private set; }
        public DateTime At { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Balance
        {
            get { return m_balance; }
            set
            {
                if (m_balance != value)
                {
                    m_balance = value;

                    OnPropertyChanged();
                }
            }
        }
    }
}
