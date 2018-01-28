﻿using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IMainWindowViewModel
    {
        ICommand AccountsViewCommand { get; }
        ICommand BudgetsViewCommand { get; }
        ICommand TransactionsViewCommand { get; }
    }
}
