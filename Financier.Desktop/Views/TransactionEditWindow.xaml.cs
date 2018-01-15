using Financier.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for TransactionWindow.xaml
    /// </summary>
    public partial class TransactionEditWindow : Window
    {
        public TransactionEditWindow(ITransactionEditViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
