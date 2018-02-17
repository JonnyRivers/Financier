using System.Collections.Generic;

namespace Financier.Services
{
    public interface ITransactionRelationshipService
    {
        void Create(TransactionRelationship transactionRelationship);
        void CreateMany(IEnumerable<TransactionRelationship> transactionRelationships);
    }
}
