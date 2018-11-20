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
            Save();
        }

        public void Create(DatabaseConnection databaseConnection)
        {
            throw new NotImplementedException();
        }

        public void Delete(int databaseConnectionId)
        {
            throw new NotImplementedException();
        }

        public DatabaseConnection Get(int databaseConnectionId)
        {
            return m_databaseConnections.SingleOrDefault(dbc => dbc.DatabaseConnectionId == databaseConnectionId);
        }

        public IEnumerable<DatabaseConnection> GetAll()
        {
            return m_databaseConnections;
        }

        public void Update(DatabaseConnection databaseConnection)
        {
            throw new NotImplementedException();
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
