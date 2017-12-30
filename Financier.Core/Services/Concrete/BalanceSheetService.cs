using Financier.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Financier.Services
{
    public class BalanceSheetService : IBalanceSheetService
    {
        ILogger<BalanceSheetService> m_logger;
        FinancierDbContext m_dbContext;

        public BalanceSheetService(ILogger<BalanceSheetService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }
    }
}
