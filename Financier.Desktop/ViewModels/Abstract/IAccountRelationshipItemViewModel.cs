using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountRelationshipItemViewModelFactory
    {
        IAccountRelationshipItemViewModel Create(AccountRelationship accountRelationship);
    }

    public interface IAccountRelationshipItemViewModel
    {
        int AccountRelationshipId { get; }
        IAccountLinkViewModel SourceAccount { get; }
        IAccountLinkViewModel DestinationAccount { get; }
        AccountRelationshipType Type { get; }
    }
}
