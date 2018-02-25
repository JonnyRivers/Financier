using Financier.Desktop.Commands;
using Financier.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public abstract class AccountRelationshipDetailsBaseViewModel : IAccountRelationshipDetailsViewModel
    {
        protected IAccountService m_accountService;
        protected IAccountRelationshipService m_accountRelationshipService;

        protected int m_accountRelationshipId;

        protected AccountRelationshipDetailsBaseViewModel(
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            int accountRelationshipId)
        {
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;

            m_accountRelationshipId = accountRelationshipId;

            Accounts = accountService.GetAllAsLinks();
            Types = Enum.GetValues(typeof(AccountRelationshipType)).Cast<AccountRelationshipType>();
        }

        public IEnumerable<AccountLink> Accounts { get; }
        public IEnumerable<AccountRelationshipType> Types { get; }

        public AccountLink SourceAccount { get; set; }
        public AccountLink DestinationAccount { get; set; }
        public AccountRelationshipType SelectedType { get; set; }

        public AccountRelationship ToAccountRelationship()
        {
            return new AccountRelationship
            {
                AccountRelationshipId = m_accountRelationshipId,
                SourceAccount = SourceAccount,
                DestinationAccount = DestinationAccount,
                Type = SelectedType
            };
        }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        protected abstract void OKExecute(object obj);

        private void CancelExecute(object obj)
        {

        }
    }
}
