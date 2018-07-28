﻿using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModels
{
    public interface IBalanceSheetViewModel
    {
        ObservableCollection<IBalanceSheetItemViewModel> Assets { get; }
        decimal TotalAssets { get; }
        ObservableCollection<IBalanceSheetItemViewModel> Liabilities { get; }
        decimal TotalLiabilities { get; }
        decimal NetWorth { get; }
    }
}
