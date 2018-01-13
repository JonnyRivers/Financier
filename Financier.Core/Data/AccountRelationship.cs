using System.ComponentModel.DataAnnotations;

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
        [Required]
        public AccountRelationshipType Type { get; set; }

        public Account SourceAccount { get; set; }
        public Account DestinationAccount { get; set; }
    }
}
