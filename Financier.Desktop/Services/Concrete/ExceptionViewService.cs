using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.Services
{
    public class ExceptionViewService : IExceptionViewService
    {
        private readonly ILogger<MainViewService> m_logger;
        private readonly IExceptionViewModelFactory m_exceptionViewModelFactory;

        public ExceptionViewService(ILogger<MainViewService> logger, IExceptionViewModelFactory exceptionViewModelFactory)
        {
            m_logger = logger;
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
