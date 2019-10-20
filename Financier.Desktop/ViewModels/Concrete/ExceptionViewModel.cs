using System;
using System.Windows.Input;
using Financier.Desktop.Commands;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class ExceptionViewModelFactory : IExceptionViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;

        public ExceptionViewModelFactory(ILoggerFactory loggerFactory)
        {
            m_loggerFactory = loggerFactory;
        }

        public IExceptionViewModel Create(Exception ex)
        {
            return new ExceptionViewModel(m_loggerFactory, ex);
        }
    }

    public class ExceptionViewModel : IExceptionViewModel
    {
        private ILogger<ExceptionViewModel> m_logger;

        public ExceptionViewModel(
            ILoggerFactory loggerFactory,
            Exception ex)
        {
            m_logger = loggerFactory.CreateLogger<ExceptionViewModel>();

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
