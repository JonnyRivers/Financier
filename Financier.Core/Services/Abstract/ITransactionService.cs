using System.Collections.Generic;

namespace Financier.Services
{
    public interface ITransactionService
    {
        bool Any();
        void Create(Transaction transaction);
        void Delete(int transactionId);
        Transaction Get(int transactionId);
        Transaction GetMostRecent();
        IEnumerable<Transaction> GetAll();
        IEnumerable<Transaction> GetAll(int accountId, bool includeLogicalAccounts);
        void Update(Transaction transaction);
    }
}
