using System;
using Financier.Desktop.ViewModels;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class MainViewModelFactory : IMainViewModelFactory
    {
        private readonly ILogger<MainViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public MainViewModelFactory(ILogger<MainViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IMainViewModel Create()
        {
            return m_serviceProvider.CreateInstance<MainViewModel>();
        }
    }
}
