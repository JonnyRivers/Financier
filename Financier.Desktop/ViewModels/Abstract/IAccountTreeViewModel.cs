using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountTreeViewModel
    {
        ObservableCollection<IAccountTreeItemViewModel> AccountItems { get; }
    }
}
