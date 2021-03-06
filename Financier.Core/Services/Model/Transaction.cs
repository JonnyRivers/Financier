﻿using System;

namespace Financier.Services
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public AccountLink CreditAccount { get; set; }
        public AccountLink DebitAccount { get; set; }
        public decimal Amount { get; set; }
        public DateTime At { get; set; }
    }
}
