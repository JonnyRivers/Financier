using System;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IUnhandledExceptionViewModel
    {
        string Message { get; }
        string Details { get; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
