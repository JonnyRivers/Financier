namespace Financier.Desktop.Services
{
    public interface IDatabaseConnectionPasswordViewService
    {
        bool Show(string userId, out string password);
    }
}
