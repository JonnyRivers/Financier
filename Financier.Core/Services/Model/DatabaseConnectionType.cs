using System.ComponentModel;

namespace Financier.Services
{
    public enum DatabaseConnectionType
    {
        [Description("SQL Server Express LocalDB")]
        SqlServerLocalDB,
        [Description("Azure Hosted SQL Server Instance")]
        SqlServerAzure
    }
}
