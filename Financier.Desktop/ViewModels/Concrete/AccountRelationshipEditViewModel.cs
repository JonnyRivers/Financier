using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
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
