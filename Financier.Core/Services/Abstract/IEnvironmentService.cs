namespace Financier.Services
{
    public interface IEnvironmentService
    {
        string GetConnectionString();
        string GetConnectionSummary();
    }
}
