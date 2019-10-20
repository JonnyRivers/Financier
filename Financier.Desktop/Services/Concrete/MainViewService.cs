using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class MainViewService : IMainViewService
    {
        private readonly ILogger<MainViewService> m_logger;
        private readonly IMainViewModelFactory m_mainViewModelFactory;

        public MainViewService(ILoggerFactory loggerFactory, IMainViewModelFactory mainViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<MainViewService>();
            m_mainViewModelFactory = mainViewModelFactory;
        }

        public void Show()
        {
            IMainViewModel mainViewModel = m_mainViewModelFactory.Create();
            var mainWindow = new MainWindow(mainViewModel);
            mainWindow.Show();
        }
    }
}
