using System.Collections.Generic;

namespace Financier.Services
{
    public interface IAccountService
    {
        void Create(Account account);
        Account Get(int accountId);
        IEnumerable<Account> GetAll();
        void Update(Account account);
    }
}
