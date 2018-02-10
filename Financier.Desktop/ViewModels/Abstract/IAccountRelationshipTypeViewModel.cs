namespace Financier.Desktop.ViewModels
{
    public interface IAccountRelationshipTypeFilterViewModel
    {
        string Name { get; }
        AccountRelationshipType? Type { get; }
    }
}
