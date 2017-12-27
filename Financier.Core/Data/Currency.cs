using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Financier.Data
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Symbol { get; set; }
    }
}
