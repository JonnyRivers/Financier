namespace Financier.Desktop.Services
{
    public interface IForeignAmountViewService
    {
        bool Show(
            decimal nativeAmount,
            string nativeCurrencyCode,
            string foreignCurrencyCode, 
            out decimal exchangedAmount);
    }
}
