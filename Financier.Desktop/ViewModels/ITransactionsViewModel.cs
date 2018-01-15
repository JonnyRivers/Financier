using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionsViewModel
    {
        ObservableCollection<ITransactionViewModel> Transactions { get; }
        ITransactionViewModel SelectedTransaction { get; }

        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
