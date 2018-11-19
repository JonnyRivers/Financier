namespace Financier.Desktop.Services
{
    public interface IConnection
    {
        string ConnectionString { get; }
        DatabaseType Type { get; } 
    }
}
