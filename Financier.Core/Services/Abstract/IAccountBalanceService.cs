namespace Financier.Services
{
    public interface IAccountBalanceService
    {
        decimal GetBalance(int accountId);
    }
}
