namespace Financier.Services
{
    public class AccountRelationship
    {
        public int AccountRelationshipId { get; internal set; }
        public AccountLink SourceAccount { get; set; }
        public AccountLink DestinationAccount { get; set; }
        public AccountRelationshipType Type { get; set; }
    }
}
