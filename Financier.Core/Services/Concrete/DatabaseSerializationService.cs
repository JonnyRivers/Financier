using Financier.Data;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Xml.Linq;

namespace Financier.Services
{
    public class DatabaseSerializationService : IDatabaseSerializationService
    {
        ILogger<DatabaseSerializationService> m_logger;
        FinancierDbContext m_dbContext;

        public DatabaseSerializationService(ILogger<DatabaseSerializationService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Load(string path)
        {
            XDocument document = XDocument.Load(path);
            XElement rootElement = document.Root;
            XElement[] accountElements = rootElement.Elements(XName.Get("Account")).ToArray();
            XElement[] accountRelationshipElements = rootElement.Elements(XName.Get("AccountRelationship")).ToArray();
            XElement[] currencyElements = rootElement.Elements(XName.Get("Currency")).ToArray();
            XElement[] transactionElements = rootElement.Elements(XName.Get("Transaction")).ToArray();

            throw new System.NotImplementedException();
        }

        public void Save(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}
