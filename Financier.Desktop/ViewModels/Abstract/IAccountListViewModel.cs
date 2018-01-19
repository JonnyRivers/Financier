using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountListViewModel
    {
        ObservableCollection<IAccountItemViewModel> Accounts { get; }
        IAccountItemViewModel SelectedAccount { get; }

        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
    }
}
