using System;

namespace Financier.Services
{
    public class DatabaseConnection
    {
        public int DatabaseConnectionId { get; set; }
        public string Name { get; set; }
        public DatabaseConnectionType Type { get; set; }

        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }

        public string BuildConnectionString(string password)
        {
            if(Type == DatabaseConnectionType.SqlServerAzure)
            {
                return $"Server={Server};Database={Database};" + 
                    $"Persist Security Info=False;User ID={UserId};Password={password};" + 
                    "MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;" + 
                    "Connection Timeout=30;";
            }
            else if (Type == DatabaseConnectionType.SqlServerLocalDB)
            {
                return $"Server={Server};Database={Database};Trusted_Connection=True;";
            }

            throw new NotImplementedException();
        }
    }
}
