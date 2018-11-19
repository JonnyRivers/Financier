using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class ConnectionViewService : IConnectionViewService
    {
        private readonly ILogger<ConnectionViewService> m_logger;
        private readonly IConnectionViewModelFactory m_viewModelFactory;

        public ConnectionViewService(ILogger<ConnectionViewService> logger, IConnectionViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public IConnection OpenConnectionView()
        {
            var viewModel = m_viewModelFactory.CreateConnectionWindowViewModel();
            var connectionWindow = new ConnectionWindow(viewModel);
            connectionWindow.ShowDialog();

            return viewModel.Connection;
        }
    }
}
