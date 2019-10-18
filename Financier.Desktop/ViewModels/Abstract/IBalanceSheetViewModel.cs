using System;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModels
{
    public interface IBalanceSheetViewModelFactory
    {
        IBalanceSheetViewModel Create();
    }

    public interface IBalanceSheetViewModel
    {
        DateTime At { get; set; }

        ObservableCollection<IBalanceSheetItemViewModel> Assets { get; }
        decimal TotalAssets { get; }
        ObservableCollection<IBalanceSheetItemViewModel> Liabilities { get; }
        decimal TotalLiabilities { get; }
        decimal NetWorth { get; }
    }
}
