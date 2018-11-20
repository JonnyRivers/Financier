using System;

namespace Financier.Services
{
    public class DatabaseConnection
    {
        public int DatabaseConnectionId { get; set; }
        public string Name { get; set; }
        public DatabaseConnectionType Type { get; set; }

        public string BuildConnectionString()
        {
            throw new NotImplementedException();
        }
    }
}
