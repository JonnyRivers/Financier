namespace Financier.Services
{
    public class TransactionRelationship
    {
        public int TransactionRelationshipId { get; set; }
        public Transaction SourceTransaction { get; set; }
        public Transaction DestinationTransaction { get; set; }
        public TransactionRelationshipType Type { get; set; }
    }
}
