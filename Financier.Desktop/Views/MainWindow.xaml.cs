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
        private IMainWindowViewModel _viewModel;

        public MainWindow(IMainWindowViewModel viewModel)
        {
            InitializeComponent();
            
            _viewModel = viewModel;

            DataContext = viewModel;
        }
    }
}
