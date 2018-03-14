using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AccountTreeViewWindow.xaml
    /// </summary>
    public partial class AccountTreeWindow : Window
    {
        public AccountTreeWindow(IAccountTreeViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
