using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Financier.Data
{
    public class AccountRelationship
    {
        [Key]
        public int AccountRelationshipId { get; set; }
        [Required]
        public int SourceAccountId { get; set; }
        [Required]
        public int DestinationAccountId { get; set; }
        public AccountRelationshipType Type { get; set; }

        public Account SourceAccount { get; set; }
        public Account DestinationAccount { get; set; }
    }
}
