using System.Collections.Generic;

namespace Financier.Services
{
    public class AccountLink
    {
        public int AccountId { get; set; }
        public IEnumerable<int> LogicalAccountIds { get; set; }
        public string Name { get; set; }
        public AccountType Type { get; set; }
    }
}
