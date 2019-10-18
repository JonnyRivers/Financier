using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountRelationshipDetailsViewModelFactory : IAccountRelationshipDetailsViewModelFactory
    {
        private readonly ILogger<AccountRelationshipDetailsViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public AccountRelationshipDetailsViewModelFactory(
            ILogger<AccountRelationshipDetailsViewModelFactory> logger, 
            IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IAccountRelationshipDetailsViewModel Create(AccountRelationship hint)
        {
            return m_serviceProvider.CreateInstance<AccountRelationshipCreateViewModel>(hint);
        }

        IAccountRelationshipDetailsViewModel IAccountRelationshipDetailsViewModelFactory.Create(int accountRelationshipId)
        {
            return m_serviceProvider.CreateInstance<AccountRelationshipEditViewModel>(accountRelationshipId);
        }
    }

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

    public class AccountRelationshipCreateViewModel : AccountRelationshipDetailsBaseViewModel
    {
        private ILogger<AccountRelationshipCreateViewModel> m_logger;

        public AccountRelationshipCreateViewModel(
            ILogger<AccountRelationshipCreateViewModel> logger,
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            AccountRelationship hint) : base(accountService, accountRelationshipService, 0)
        {
            m_logger = logger;

            SourceAccount = Accounts.Single(a => a.AccountId == hint.SourceAccount.AccountId);
            DestinationAccount = Accounts.Single(a => a.AccountId == hint.DestinationAccount.AccountId);
            SelectedType = hint.Type;
        }

        protected override void OKExecute(object obj)
        {
            AccountRelationship accountRelationship = ToAccountRelationship();

            m_accountRelationshipService.Create(accountRelationship);
            m_accountRelationshipId = accountRelationship.AccountRelationshipId;
        }
    }

    public class AccountRelationshipEditViewModel : AccountRelationshipDetailsBaseViewModel
    {
        private ILogger<AccountRelationshipEditViewModel> m_logger;

        public AccountRelationshipEditViewModel(
            ILogger<AccountRelationshipEditViewModel> logger,
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            int accountRelationshipId) : base(accountService, accountRelationshipService, accountRelationshipId)
        {
            m_logger = logger;

            AccountRelationship accountRelationship = m_accountRelationshipService.Get(accountRelationshipId);

            SourceAccount = Accounts.Single(a => a.AccountId == accountRelationship.SourceAccount.AccountId);
            DestinationAccount = Accounts.Single(a => a.AccountId == accountRelationship.DestinationAccount.AccountId);
            SelectedType = accountRelationship.Type;
        }

        protected override void OKExecute(object obj)
        {
            AccountRelationship accountRelationship = ToAccountRelationship();

            m_accountRelationshipService.Update(accountRelationship);
        }
    }
}
