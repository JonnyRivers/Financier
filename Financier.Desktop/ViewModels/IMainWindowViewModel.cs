using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModels
{
    public interface IMainWindowViewModel
    {
        ObservableCollection<ITransactionViewModel> Transactions { get; }
    }
}
