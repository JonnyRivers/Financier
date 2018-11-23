using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Financier.Services
{
    public class LocalDatabaseConnectionService : IDatabaseConnectionService
    {
        private string m_path;
        private List<DatabaseConnection> m_databaseConnections;

        public LocalDatabaseConnectionService()
        {
            string localApplicationDataDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string financierDataDirectory = Path.Combine(localApplicationDataDirectory, "Financier");
            if (!Directory.Exists(financierDataDirectory))
                Directory.CreateDirectory(financierDataDirectory);
            m_path = Path.Combine(financierDataDirectory, "DatabaseConnections.json");

            Load();
        }

        public void Create(DatabaseConnection databaseConnection)
        {
            databaseConnection.DatabaseConnectionId = 1;
            if (m_databaseConnections.Any())
            {
                databaseConnection.DatabaseConnectionId = m_databaseConnections.Max(dbc => dbc.DatabaseConnectionId) + 1;
            }

            m_databaseConnections.Add(databaseConnection);

            Save();
        }

        public void Delete(int databaseConnectionId)
        {
            DatabaseConnection databaseConnection = Get(databaseConnectionId);
            m_databaseConnections.Remove(databaseConnection);

            Save();
        }

        public DatabaseConnection Get(int databaseConnectionId)
        {
            return m_databaseConnections.Single(dbc => dbc.DatabaseConnectionId == databaseConnectionId);
        }

        public DatabaseConnection Get(string databaseConnectionName)
        {
            return m_databaseConnections.Single(dbc => dbc.Name == databaseConnectionName);
        }

        public IEnumerable<DatabaseConnection> GetAll()
        {
            return m_databaseConnections;
        }

        public void Update(DatabaseConnection databaseConnection)
        {
            DatabaseConnection existingDatabaseConnection = Get(databaseConnection.DatabaseConnectionId);

            if(existingDatabaseConnection == null)
                throw new InvalidOperationException($"Database connection with id {databaseConnection.DatabaseConnectionId} does not exist");

            existingDatabaseConnection.Database = databaseConnection.Database;
            existingDatabaseConnection.Name = databaseConnection.Name;
            existingDatabaseConnection.Server = databaseConnection.Server;
            existingDatabaseConnection.Type = databaseConnection.Type;
            existingDatabaseConnection.UserId = databaseConnection.UserId;

            Save();
        }

        private void Load()
        {
            if(File.Exists(m_path))
            {
                string fileContents = File.ReadAllText(m_path);
                JArray jsonObject = JArray.Parse(fileContents);
                m_databaseConnections = jsonObject.ToObject<List<DatabaseConnection>>();
            }
            else
            {
                m_databaseConnections = new List<DatabaseConnection>();
            }
        }

        private void Save()
        {
            JArray jsonObject = (JArray)JToken.FromObject(m_databaseConnections);
            string fileContents = jsonObject.ToString();
            File.WriteAllText(m_path, fileContents);
        }
    }
}
