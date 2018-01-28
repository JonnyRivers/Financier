﻿using Financier.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionBatchCreateConfirmViewModel
    {
        void Setup(IEnumerable<Transaction> transactions);

        ObservableCollection<ITransactionItemViewModel> Transactions { get; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
