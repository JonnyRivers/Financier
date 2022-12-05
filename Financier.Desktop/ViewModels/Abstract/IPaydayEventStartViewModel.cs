using Financier.Services;
using System;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IPaydayEventStartViewModelFactory
    {
        IPaydayEventStartViewModel Create(int budgetId);
    }

    public interface IPaydayEventStartViewModel
    {
        PaydayStart ToPaydayStart();

        decimal AmountPaid { get; set; }
        DateTime At { get; set; }
        bool IncludeSurplus { get; set; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
