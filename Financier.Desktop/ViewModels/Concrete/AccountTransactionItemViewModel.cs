using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class AccountTransactionItemViewModel : BaseViewModel, IAccountTransactionItemViewModel
    {
        private ILogger<TransactionItemViewModel> m_logger;
        private IConversionService m_conversionService;

        private decimal m_balance;

        public AccountTransactionItemViewModel(
            ILogger<TransactionItemViewModel> logger,
            IConversionService conversionService)
        {
            m_logger = logger;
            m_conversionService = conversionService;

            m_balance = 0;
        }

        public void Setup(Transaction transaction)
        {
            TransactionId = transaction.TransactionId;
            CreditAccount = m_conversionService.AccountLinkToViewModel(transaction.CreditAccount);
            DebitAccount = m_conversionService.AccountLinkToViewModel(transaction.DebitAccount);
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
