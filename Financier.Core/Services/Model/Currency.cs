namespace Financier.Services
{
    public class Currency
    {
        public int CurrencyId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Symbol { get; set; }
        public bool IsPrimary { get; set; }
    }
}
