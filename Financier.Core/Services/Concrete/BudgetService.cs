using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Financier.Services
{
    public class BudgetService : IBudgetService
    {
        private ILogger<BudgetService> m_logger;
        private Entities.FinancierDbContext m_dbContext;

        public BudgetService(ILogger<BudgetService> logger, Entities.FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Create(Budget budget)
        {
            throw new NotImplementedException();
        }

        public Budget Get(int budgetId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Budget> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(Budget budget)
        {
            throw new NotImplementedException();
        }
    }
}
