using Microsoft.Win32;
using System.Collections.Generic;

namespace Financier.Services
{
    public class DataConfigRegistryService : IDataConfigService
    {
        private const RegistryHive Hive = RegistryHive.CurrentUser;
        private const RegistryView View = RegistryView.Default;

        private readonly RegistryKey m_baseKey;

        public DataConfigRegistryService()
        {
            m_baseKey = RegistryKey.OpenBaseKey(Hive, View);
        }

        public string GetConnectionString(string databaseName)
        {
            RegistryKey databaseKey = m_baseKey.OpenSubKey($"Software\\Financier\\Databases\\{databaseName}");
            string connectionString = (string)databaseKey.GetValue("ConnectionString");

            return connectionString;
        }

        public string GetCurrentDatabase()
        {
            RegistryKey desktopKey = m_baseKey.OpenSubKey($"Software\\Financier\\Desktop");
            string currentDatabase = (string)desktopKey.GetValue("CurrentDatabase");

            return currentDatabase;
        }

        public IEnumerable<string> GetDatabases()
        {
            RegistryKey databasesKey = m_baseKey.OpenSubKey($"Software\\Financier\\Databases");
            string[] databaseNames = databasesKey.GetSubKeyNames();

            return databaseNames;
        }
    }
}
