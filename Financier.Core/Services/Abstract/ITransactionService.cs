using System.Collections.Generic;

namespace Financier.Services
{
    public interface ITransactionService
    {
        void Create(Transaction transaction);
        void Delete(int transactionId);
        Transaction Get(int transactionId);
        IEnumerable<Transaction> GetAll();
        IEnumerable<Transaction> GetAll(int accountId, bool includeLogicalAccounts);
        void Update(Transaction transaction);
    }
}
