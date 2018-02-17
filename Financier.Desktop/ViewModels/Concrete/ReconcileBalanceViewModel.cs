using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class ReconcileBalanceViewModel : BaseViewModel, IReconcileBalanceViewModel
    {
        private ILogger<ReconcileBalanceViewModel> m_logger;
        private IAccountService m_accountService;
        private ITransactionService m_transactionService;
        private IViewModelFactory m_viewModelFactory;

        private int m_accountId;
        private decimal m_openingBalance;
        private decimal m_targetBalance;
        private IAccountLinkViewModel m_selectedCreditAccount;
        private DateTime m_at;

        private Transaction m_newTransaction;

        public ReconcileBalanceViewModel(
            ILogger<ReconcileBalanceViewModel> logger,
            IAccountService accountService,
            ITransactionService transactionService,
            IViewModelFactory viewModelFactory,
            int accountId)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_transactionService = transactionService;
            m_viewModelFactory = viewModelFactory;

            m_accountId = accountId;
            m_openingBalance = m_targetBalance = m_accountService.GetBalance(m_accountId, true);
            m_at = DateTime.Now;

            IEnumerable<AccountLink> accountLinks = m_accountService.GetAllAsLinks().Where(al => al.Type == AccountType.Income);
            IEnumerable<IAccountLinkViewModel> accountLinkViewModels =
                accountLinks.Select(al => m_viewModelFactory.CreateAccountLinkViewModel(al));
            
            Accounts = new ObservableCollection<IAccountLinkViewModel>(accountLinkViewModels.OrderBy(alvm => alvm.Name));
            m_selectedCreditAccount = Accounts.FirstOrDefault();
        }

        public ObservableCollection<IAccountLinkViewModel> Accounts { get; }

        public decimal Balance
        {
            get { return m_targetBalance; }
            set
            {
                if (m_targetBalance != value)
                {
                    m_targetBalance = value;

                    OnPropertyChanged();
                }
            }
        }
        public IAccountLinkViewModel SelectedCreditAccount
        {
            get { return m_selectedCreditAccount; }
            set
            {
                if (m_selectedCreditAccount != value)
                {
                    m_selectedCreditAccount = value;

                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        public DateTime At
        {
            get { return m_at; }
            set
            {
                if (m_at != value)
                {
                    m_at = value;

                    OnPropertyChanged();
                }
            }
        }

        public ICommand OKCommand => new RelayCommand(OKExecute, OKCanExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {
            m_newTransaction = new Transaction
            {
                DebitAccount = m_accountService.GetAsLink(m_accountId),
                CreditAccount = m_accountService.GetAsLink(SelectedCreditAccount.AccountId),
                Amount = m_targetBalance - m_openingBalance,
                At = m_at
            };
            m_transactionService.Create(m_newTransaction);
        }

        private bool OKCanExecute(object obj)
        {
            return (SelectedCreditAccount != null);
        }

        private void CancelExecute(object obj)
        {

        }

        public Transaction ToTransaction()
        {
            return m_newTransaction;
        }
    }
}
