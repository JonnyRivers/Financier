using Financier.Entities;
using Microsoft.Extensions.Logging;

namespace Financier.Services
{
    public class TransactionRelationshipService : ITransactionRelationshipService
    {
        private ILogger<TransactionRelationshipService> m_logger;
        private FinancierDbContext m_dbContext;

        public TransactionRelationshipService(ILogger<TransactionRelationshipService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Create(TransactionRelationship transactionRelationship)
        {
            var transactionRelationshipEntity = new Entities.TransactionRelationship
            {
                SourceTransactionId = transactionRelationship.SourceTransaction.TransactionId,
                DestinationTransactionId = transactionRelationship.DestinationTransaction.TransactionId,
                Type = transactionRelationship.Type
            };
            m_dbContext.TransactionRelationships.Add(transactionRelationshipEntity);
            m_dbContext.SaveChanges();

            transactionRelationship.TransactionRelationshipId = transactionRelationshipEntity.TransactionRelationshipId;
        }
    }
}
