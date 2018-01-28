using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for BudgetListWindow.xaml
    /// </summary>
    public partial class BudgetListWindow : Window
    {
        public BudgetListWindow(IBudgetListViewModel budgetListViewModel)
        {
            InitializeComponent();

            DataContext = budgetListViewModel;
        }
    }
}
