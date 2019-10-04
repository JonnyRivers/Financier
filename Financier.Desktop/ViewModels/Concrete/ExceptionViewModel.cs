using System;
using System.Windows.Input;
using Financier.Desktop.Commands;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class ExceptionViewModelFactory : IExceptionViewModelFactory
    {
        private readonly ILogger<ExceptionViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public ExceptionViewModelFactory(ILogger<ExceptionViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IExceptionViewModel Create(Exception ex)
        {
            return m_serviceProvider.CreateInstance<ExceptionViewModel>(ex);
        }
    }

    public class ExceptionViewModel : IExceptionViewModel
    {
        private ILogger<ExceptionViewModel> m_logger;

        public ExceptionViewModel(
            ILogger<ExceptionViewModel> logger,
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
