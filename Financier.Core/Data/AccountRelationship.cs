using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Financier.Data
{
    public class AccountRelationship
    {
        [Key]
        public int AccountRelationshipId { get; set; }
        public int SourceAccountId { get; set; }
        public int DestinationAccountId { get; set; }
        public AccountRelationshipType Type { get; set; }

        public Account SourceAccount { get; set; }
        public Account DestinationAccount { get; set; }
    }
}
