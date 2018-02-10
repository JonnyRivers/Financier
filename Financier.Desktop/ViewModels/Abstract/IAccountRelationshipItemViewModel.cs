namespace Financier.Desktop.ViewModels
{
    public interface IAccountRelationshipItemViewModel
    {
        int AccountRelationshipId { get; }
        IAccountLinkViewModel SourceAccount { get; }
        IAccountLinkViewModel DestinationAccount { get; }
        AccountRelationshipType Type { get; }
    }
}
