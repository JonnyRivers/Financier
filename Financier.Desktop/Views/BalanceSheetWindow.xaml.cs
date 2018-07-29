using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for BalanceSheetWindow.xaml
    /// </summary>
    public partial class BalanceSheetWindow : Window
    {
        public BalanceSheetWindow(IBalanceSheetViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
