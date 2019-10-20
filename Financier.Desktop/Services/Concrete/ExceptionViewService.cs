using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.Services
{
    public class ExceptionViewService : IExceptionViewService
    {
        private readonly ILogger<ExceptionViewService> m_logger;
        private readonly IExceptionViewModelFactory m_exceptionViewModelFactory;

        public ExceptionViewService(ILoggerFactory loggerFactory, IExceptionViewModelFactory exceptionViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<ExceptionViewService>();
            m_exceptionViewModelFactory = exceptionViewModelFactory;
        }

        public void Show(Exception ex)
        {
            IExceptionViewModel viewModel = m_exceptionViewModelFactory.Create(ex);
            var window = new UnhandledExceptionWindow(viewModel);
            window.Show();
        }
    }
}
