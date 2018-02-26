using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
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
}
