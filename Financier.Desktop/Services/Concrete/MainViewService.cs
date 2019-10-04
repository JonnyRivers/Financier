using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class MainViewService : IMainViewService
    {
        private readonly ILogger<MainViewService> m_logger;
        private readonly IMainViewModelFactory m_mainViewModelFactory;

        public MainViewService(ILogger<MainViewService> logger, IMainViewModelFactory mainViewModelFactory)
        {
            m_logger = logger;
            m_mainViewModelFactory = mainViewModelFactory;
        }

        public void Show()
        {
            IMainViewModel viewModel = m_mainViewModelFactory.Create();
            var mainWindow = new MainWindow(viewModel);
            mainWindow.Show();
        }
    }
}
