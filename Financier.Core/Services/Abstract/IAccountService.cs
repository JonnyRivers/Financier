using System;
using System.Collections.Generic;

namespace Financier.Services
{
    public interface IAccountService
    {
        void Create(Account account);
        Account Get(int accountId);
        IEnumerable<Account> GetAll();
        IEnumerable<AccountLink> GetAllAsLinks();
        IEnumerable<Account> GetAllPhysical();
        void Update(Account account);

        decimal GetBalance(int accountId, bool includeLogical);
        decimal GetBalanceAt(int accountId, DateTime at, bool includeLogical);
    }
}
