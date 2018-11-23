using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for DatabaseConnectionListWindow.xaml
    /// </summary>
    public partial class DatabaseConnectionListWindow : Window
    {
        public DatabaseConnectionListWindow(IDatabaseConnectionListViewModel databaseConnectionListViewModel)
        {
            InitializeComponent();

            DataContext = databaseConnectionListViewModel;
        }

        private void OnConnect(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
