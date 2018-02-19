namespace Financier.Desktop.ViewModels
{
    public interface IForeignAmountViewModel
    {
        decimal ForeignAmount { get; set; }
        string ForeignCurrencyCode { get; }
        decimal ForeignToNativeRate { get; }
        decimal NativeAmount { get; }
        string NativeCurrencyCode { get; }
    }
}
