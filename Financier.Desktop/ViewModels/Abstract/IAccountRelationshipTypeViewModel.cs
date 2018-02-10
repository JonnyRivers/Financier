namespace Financier.Desktop.ViewModels
{
    public interface IAccountRelationshipTypeViewModel
    {
        string Name { get; }
        AccountRelationshipType? Type { get; }
    }
}
