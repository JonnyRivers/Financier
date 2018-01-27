using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class AccountUpdateMessage
    {
        public AccountUpdateMessage(Account updatedAccount)
        {
            UpdatedAccount = updatedAccount;
        }

        public Account UpdatedAccount { get; private set; }
    }
}
