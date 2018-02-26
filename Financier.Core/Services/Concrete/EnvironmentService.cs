using System;

namespace Financier.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        private const string databaseConnectionStringEnvironmentVariableName = "FINANCIER_DBCS";
        private const string DefaultStringValue = "???";
        private const string ServerKey = "Server";
        private const string DatabaseKey = "Database";

        public string GetConnectionString()
        {
            string connectionString = Environment.GetEnvironmentVariable(databaseConnectionStringEnvironmentVariableName);

            if (string.IsNullOrEmpty(connectionString))
                throw new ApplicationException(
                    "Unable to get database connection from environement variable " + 
                    $"'{databaseConnectionStringEnvironmentVariableName}'");

            return connectionString;
        }

        public string GetConnectionSummary()
        {
            string connectionString = GetConnectionString();
            
            string serverName = DefaultStringValue;
            string databaseName = DefaultStringValue;
            string[] connectionStringTokens = connectionString.Split(';');
            foreach (string connectionStringToken in connectionStringTokens)
            {
                string[] tokenPair = connectionStringToken.Split('=');
                if (tokenPair.Length != 2)
                    continue;

                if (tokenPair[0] == ServerKey)
                    serverName = tokenPair[1];
                else if (tokenPair[0] == DatabaseKey)
                    databaseName = tokenPair[1];
            }

            return $"{databaseName} at {serverName}";
        }
    }
}
