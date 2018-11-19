namespace Financier.Desktop.Services
{
    public class Connection : IConnection
    {
        public string ConnectionString { get; set; }

        public DatabaseType Type { get; set; }
    }
}
