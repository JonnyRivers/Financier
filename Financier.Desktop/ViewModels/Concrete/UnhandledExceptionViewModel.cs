using System;
using System.Windows.Input;
using Financier.Desktop.Commands;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class UnhandledExceptionViewModel : IUnhandledExceptionViewModel
    {
        private ILogger<UnhandledExceptionViewModel> m_logger;

        public UnhandledExceptionViewModel(
            ILogger<UnhandledExceptionViewModel> logger,
            Exception ex)
        {
            m_logger = logger;

            Message = ex.Message;
            Details = ex.ToString();
        }

        public string Message { get; }
        public string Details { get; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {

        }

        private void CancelExecute(object obj)
        {

        }
    }
}
