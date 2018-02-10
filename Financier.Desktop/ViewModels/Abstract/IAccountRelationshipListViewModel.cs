using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountRelationshipListViewModel
    {
        ObservableCollection<IAccountLinkViewModel> AccountFilters { get; }
        IAccountLinkViewModel SelectedAccountFilter { get; }
        ObservableCollection<IAccountRelationshipTypeViewModel> RelationshipTypeFilters { get; }
        IAccountRelationshipTypeViewModel SelectedRelationshipTypeFilter { get; }

        ObservableCollection<IAccountRelationshipItemViewModel> AccountRelationships { get; }
        IAccountRelationshipItemViewModel SelectedAccountRelationship { get; }

        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
