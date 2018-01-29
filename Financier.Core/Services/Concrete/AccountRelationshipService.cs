using Financier.Entities;
using Microsoft.Extensions.Logging;

namespace Financier.Services
{
    public class AccountRelationshipService : IAccountRelationshipService
    {
        private ILogger<AccountRelationshipService> m_logger;
        private FinancierDbContext m_dbContext;

        public AccountRelationshipService(ILogger<AccountRelationshipService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }
    }
}
