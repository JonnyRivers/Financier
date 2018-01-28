using Financier.Services;
using System.Collections.Generic;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountLinkViewModel
    {
        void Setup(AccountLink accountLink);
        
        AccountLink ToAccountLink();

        int AccountId { get; }
        IEnumerable<int> LogicalAccountIds { get; set; }
        string Name { get; set; }
        AccountType Type { get; set; }
    }
}
