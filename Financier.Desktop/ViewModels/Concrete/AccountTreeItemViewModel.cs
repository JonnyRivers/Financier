using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class AccountTreeItemViewModelFactory : IAccountTreeItemViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;

        public AccountTreeItemViewModelFactory(ILoggerFactory loggerFactory)
        {
            m_loggerFactory = loggerFactory;
        }

        public IAccountTreeItemViewModel Create(Account account, IEnumerable<Transaction> transactions)
        {
            return new AccountTreeItemViewModel(
                m_loggerFactory,
                account, 
                transactions, 
                new IAccountTreeItemViewModel[0]);
        }

        public IAccountTreeItemViewModel Create(Account account, IEnumerable<Transaction> transactions, IEnumerable<IAccountTreeItemViewModel> childAccountVMs)
        {
            return new AccountTreeItemViewModel(
                m_loggerFactory,
                account,
                transactions,
                childAccountVMs);
        }
    }

    public class AccountTreeItemViewModel : BaseViewModel, IAccountTreeItemViewModel
    {
        private ILogger<AccountTreeItemViewModel> m_logger;

        private string m_name;
        private AccountType m_type;
        private AccountSubType m_subType;
        private string m_currencyName;

        public AccountTreeItemViewModel(
            ILoggerFactory loggerFactory,
            Account account,
            IEnumerable<Transaction> transactions,
            IEnumerable<IAccountTreeItemViewModel> childAccountVMs)
        {
            m_logger = loggerFactory.CreateLogger<AccountTreeItemViewModel>();

            AccountId = account.AccountId;
            m_name = account.Name;
            m_type = account.Type;
            m_subType = account.SubType;
            m_currencyName = account.Currency.Name;

            decimal debitSum = transactions.Where(t => t.DebitAccount.AccountId == AccountId).Sum(t => t.Amount);
            decimal creditSum = transactions.Where(t => t.CreditAccount.AccountId == AccountId).Sum(t => t.Amount);
            decimal balance = debitSum - creditSum;

            CurrencySymbol = string.Empty;
            ChildAccountItems = new ObservableCollection<IAccountTreeItemViewModel>(childAccountVMs);
            Balance = balance + ChildAccountItems.Sum(a => a.Balance);
        }

        public int AccountId { get; private set; }

        public decimal Balance { get; }
        public string CurrencySymbol { get; }
        public ObservableCollection<IAccountTreeItemViewModel> ChildAccountItems { get; }

        public string Name
        {
            get { return m_name; }
            private set
            {
                if (m_name != value)
                {
                    m_name = value;

                    OnPropertyChanged();
                }
            }
        }
        public AccountType Type
        {
            get { return m_type; }
            private set
            {
                if (m_type != value)
                {
                    m_type = value;

                    OnPropertyChanged();
                }
            }
        }
        public AccountSubType SubType
        {
            get { return m_subType; }
            private set
            {
                if (m_subType != value)
                {
                    m_subType = value;

                    OnPropertyChanged();
                }
            }
        }
        public string CurrencyName
        {
            get { return m_currencyName; }
            private set
            {
                if (m_currencyName != value)
                {
                    m_currencyName = value;

                    OnPropertyChanged();
                }
            }
        }
    }
}
