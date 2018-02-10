using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AccountRelationshipListWindow.xaml
    /// </summary>
    public partial class AccountRelationshipListWindow : Window
    {
        public AccountRelationshipListWindow(IAccountRelationshipListViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
