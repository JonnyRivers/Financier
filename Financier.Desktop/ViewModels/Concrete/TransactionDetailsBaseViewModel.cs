using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public abstract class TransactionDetailsBaseViewModel : BaseViewModel, ITransactionDetailsViewModel
    {
        protected IAccountService m_accountService;
        protected ITransactionService m_transactionService;
        protected IAccountLinkViewModelFactory m_accountLinkViewModelFactory;

        protected int m_transactionId;

        protected TransactionDetailsBaseViewModel(
            IAccountService accountService,
            ITransactionService transactionService,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            int transactionId)
        {
            m_accountService = accountService;
            m_transactionService = transactionService;
            m_accountLinkViewModelFactory = accountLinkViewModelFactory;

            m_transactionId = transactionId;

            IEnumerable<AccountLink> accountLinks = m_accountService.GetAllAsLinks();
            IEnumerable<IAccountLinkViewModel> accountLinkViewModels =
                accountLinks.Select(al => m_accountLinkViewModelFactory.Create(al));

            Accounts = new ObservableCollection<IAccountLinkViewModel>(accountLinkViewModels.OrderBy(alvm => alvm.Name));
        }

        public Transaction ToTransaction()
        {
            var transaction = new Transaction
            {
                TransactionId = m_transactionId,
                CreditAccount = SelectedCreditAccount.ToAccountLink(),
                DebitAccount = SelectedDebitAccount.ToAccountLink(),
                Amount = Amount,
                At = At,
            };

            return transaction;
        }

        protected IAccountLinkViewModel m_selectedCreditAccount;
        protected IAccountLinkViewModel m_selectedDebitAccount;
        protected decimal m_amount;
        protected DateTime m_at;

        public ObservableCollection<IAccountLinkViewModel> Accounts { get; }

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

        public IAccountLinkViewModel SelectedDebitAccount
        {
            get { return m_selectedDebitAccount; }
            set
            {
                if (m_selectedDebitAccount != value)
                {
                    m_selectedDebitAccount = value;

                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public decimal Amount
        {
            get { return m_amount; }
            set
            {
                if (m_amount != value)
                {
                    m_amount = value;

                    OnPropertyChanged();
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

        protected abstract void OKExecute(object obj);

        private bool OKCanExecute(object obj)
        {
            return (SelectedCreditAccount != null) && (SelectedDebitAccount != null);
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
