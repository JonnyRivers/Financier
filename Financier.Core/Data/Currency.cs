using System.ComponentModel.DataAnnotations;

namespace Financier.Data
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ShortName { get; set; }
        [Required]
        public string Symbol { get; set; }
        [Required]
        public bool IsPrimary { get; set; }
    }
}
