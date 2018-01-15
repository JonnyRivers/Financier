using Financier.Desktop.ViewModels;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ILogger<MainWindow> _logger;
        private IMainWindowViewModel _viewModel;

        public MainWindow(ILogger<MainWindow> logger, IMainWindowViewModel viewModel)
        {
            InitializeComponent();

            _logger = logger;
            _viewModel = viewModel;

            DataContext = viewModel;
        }
    }
}
