using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountTreeItemViewModel
    {
        int AccountId { get; }
        decimal Balance { get; }
        string CurrencySymbol { get; }
        ObservableCollection<IAccountTreeItemViewModel> ChildAccountItems { get; }
        string Name { get; }
        AccountSubType SubType { get; }
    }
}
