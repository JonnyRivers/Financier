using System;
using Financier.Desktop.ViewModels;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class ConnectionViewModelFactory : IConnectionViewModelFactory
    {
        private readonly ILogger<ConnectionViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public ConnectionViewModelFactory(ILogger<ConnectionViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IConnectionWindowViewModel CreateConnectionWindowViewModel()
        {
            return m_serviceProvider.CreateInstance<ConnectionWindowViewModel>();
        }
    }
}
