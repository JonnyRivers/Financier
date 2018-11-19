using Financier.Desktop.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class ConnectionWindowViewModel : BaseViewModel, IConnectionWindowViewModel
    {
        private ILogger<ConnectionWindowViewModel> m_logger;

        public ConnectionWindowViewModel(
            ILogger<ConnectionWindowViewModel> logger
        )
        {
            m_logger = logger;
        }

        public IConnection Connection => throw new System.NotImplementedException();
    }
}
