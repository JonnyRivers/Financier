using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class MainViewService : IMainViewService
    {
        private readonly ILogger<MainViewService> m_logger;
        private readonly IMainViewModel m_mainViewModel;

        public MainViewService(ILogger<MainViewService> logger, IMainViewModel mainViewModel)
        {
            m_logger = logger;
            m_mainViewModel = mainViewModel;
        }

        public void Show()
        {
            var mainWindow = new MainWindow(m_mainViewModel);
            mainWindow.Show();
        }
    }
}
