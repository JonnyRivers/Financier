using Financier.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionBatchCreateConfirmViewModelFactory
    {
        ITransactionBatchCreateConfirmViewModel Create(IEnumerable<Transaction> transactions);
    }

    public interface ITransactionBatchCreateConfirmViewModel
    {
        ObservableCollection<ITransactionItemViewModel> Transactions { get; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
