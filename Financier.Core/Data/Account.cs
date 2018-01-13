﻿using System.ComponentModel.DataAnnotations;

namespace Financier.Data
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        [Required]
        public AccountType Type { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int CurrencyId { get; set; }

        public Currency Currency { get; set; }
    }
}
