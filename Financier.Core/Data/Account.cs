using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Financier.Data
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        public string Name { get; set; }
        [Required]
        public int CurrencyId { get; set; }

        public Currency Currency { get; set; }
    }
}
