using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.Services
{
    public class ExceptionViewService : IExceptionViewService
    {
        private readonly ILogger<MainViewService> m_logger;
        private readonly IExceptionViewModelFactory m_exceptionViewModel;

        public ExceptionViewService(ILogger<MainViewService> logger, IExceptionViewModelFactory exceptionViewModel)
        {
            m_logger = logger;
            m_exceptionViewModel = exceptionViewModel;
        }

        public void Show(Exception ex)
        {
            IExceptionViewModel viewModel = m_exceptionViewModel.Create(ex);
            var window = new UnhandledExceptionWindow(viewModel);
            window.Show();
        }
    }
}
