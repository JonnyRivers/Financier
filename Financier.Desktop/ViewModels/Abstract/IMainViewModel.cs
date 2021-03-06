﻿using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IMainViewModelFactory
    {
        IMainViewModel Create();
    }

    public interface IMainViewModel
    {
        ICommand AccountsViewCommand { get; }
        ICommand AccountRelationshipsViewCommand { get; }
        ICommand BalanceSheetViewCommand { get; }
        ICommand BudgetsViewCommand { get; }
        ICommand TransactionsViewCommand { get; }
        
    }
}
