using Financier.Data;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Financier.Services
{
    public class DatabaseCreationService : IDatabaseCreationService
    {
        ILogger<DatabaseCreationService> m_logger;
        FinancierDbContext m_dbContext;

        public DatabaseCreationService(ILogger<DatabaseCreationService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Create()
        {
            m_dbContext.Database.EnsureCreated();
            m_logger.LogInformation("Database is created");

            if (!m_dbContext.Currencies.Any())
            {
                var dollar = new Currency
                {
                    Name = "US Dollar",
                    ShortName = "USD",
                    Symbol = "$"
                };
                m_dbContext.Currencies.Add(dollar);
                var sterling = new Currency
                {
                    Name = "UK Sterling",
                    ShortName = "GBP",
                    Symbol = "£"
                };
                m_dbContext.Currencies.Add(sterling);
                m_dbContext.SaveChanges();

                m_logger.LogInformation("Populated database with default currencies");
            }
        }

        public void Obliterate()
        {
            m_dbContext.Database.EnsureDeleted();

            m_logger.LogInformation("Database is obliterated");
        }

        public void Populate()
        {
            m_dbContext.Database.EnsureDeleted();
            m_logger.LogInformation("Database is obliterated");

            m_dbContext.Database.EnsureCreated();
            m_logger.LogInformation("Database is created");

            var dollar = new Currency
            {
                Name = "US Dollar",
                ShortName = "USD",
                Symbol = "$"
            };
            m_dbContext.Currencies.Add(dollar);
            var sterling = new Currency
            {
                Name = "UK Sterling",
                ShortName = "GBP",
                Symbol = "£"
            };
            m_dbContext.Currencies.Add(sterling);
            m_dbContext.SaveChanges();

            m_logger.LogInformation("Populated database with default currencies");
        }
    }
}
