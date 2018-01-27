using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class AccountCreateMessage
    {
        public AccountCreateMessage(Account account)
        {
            Account = account;
        }

        public Account Account { get; private set; }
    }
}
