using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class AccountCreateMessage
    {
        public AccountCreateMessage(Account newAccount)
        {
            NewAccount = newAccount;
        }

        public Account NewAccount { get; private set; }
    }
}
