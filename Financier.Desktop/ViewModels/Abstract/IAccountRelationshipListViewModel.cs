using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountRelationshipListViewModelFactory
    {
        IAccountRelationshipListViewModel Create();
    }

    public interface IAccountRelationshipListViewModel
    {
        ObservableCollection<IAccountRelationshipItemViewModel> AccountRelationships { get; }
        IAccountRelationshipItemViewModel SelectedAccountRelationship { get; }

        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
