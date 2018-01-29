using System.ComponentModel.DataAnnotations;

namespace Financier.Entities
{
    public class TransactionRelationship
    {
        [Key]
        public int TransactionRelationshipId { get; set; }
        [Required]
        public int SourceTransactionId { get; set; }
        [Required]
        public int DestinationTransactionId { get; set; }
        [Required]
        public TransactionRelationshipType Type { get; set; }

        public Transaction SourceTransaction { get; set; }
        public Transaction DestinationTransaction { get; set; }
    }
}
