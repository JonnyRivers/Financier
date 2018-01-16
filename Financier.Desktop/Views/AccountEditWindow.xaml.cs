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
    /// Interaction logic for AccountEditWindow.xaml
    /// </summary>
    public partial class AccountEditWindow : Window
    {
        public AccountEditWindow(IAccountEditViewModel accountEditViewModel)
        {
            InitializeComponent();

            DataContext = accountEditViewModel;
        }
    }
}
