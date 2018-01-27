using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class AccountUpdateMessage
    {
        public AccountUpdateMessage(Account account)
        {
            Account = account;
        }

        public Account Account { get; private set; }
    }
}
