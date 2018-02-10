using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountRelationshipCreateViewModel : IAccountRelationshipDetailsViewModel
    {
        private ILogger<AccountRelationshipCreateViewModel> m_logger;
        private IAccountService m_accountService;
        private IAccountRelationshipService m_accountRelationshipService;

        private int m_accountRelationshipId;

        public AccountRelationshipCreateViewModel(
            ILogger<AccountRelationshipCreateViewModel> logger,
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            AccountRelationship hint)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;

            Accounts = accountService.GetAllAsLinks();
            Types = Enum.GetValues(typeof(AccountRelationshipType)).Cast<AccountRelationshipType>();

            SourceAccount = Accounts.Single(a => a.AccountId == hint.SourceAccount.AccountId);
            DestinationAccount = Accounts.Single(a => a.AccountId == hint.DestinationAccount.AccountId);
            SelectedType = hint.Type;
        }

        public IEnumerable<AccountLink> Accounts { get; }
        public IEnumerable<AccountRelationshipType> Types { get; }

        public AccountLink SourceAccount { get; set; }
        public AccountLink DestinationAccount { get; set; }
        public AccountRelationshipType SelectedType { get; set; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {
            AccountRelationship accountRelationship = ToAccountRelationship();

            m_accountRelationshipService.Create(accountRelationship);
            m_accountRelationshipId = accountRelationship.AccountRelationshipId;
        }

        private void CancelExecute(object obj)
        {

        }

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
    }
}
