using Financier.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IMainWindowViewModel
    {
        public MainWindowViewModel(ITransactionsViewModel transactionsViewModel)
        {
            TransactionsViewModel = transactionsViewModel;
        }

        public ITransactionsViewModel TransactionsViewModel { get; }
    }
}
