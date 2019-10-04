using System;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IExceptionViewModelFactory
    {
        IExceptionViewModel Create(Exception ex);
    }

    public interface IExceptionViewModel
    {
        string Message { get; }
        string Details { get; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
