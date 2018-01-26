﻿using Financier.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountEditViewModel
    {
        void SetupForCreate();
        void SetupForEdit(int accountId);

        IEnumerable<AccountType> AccountTypes { get; }
        IEnumerable<Currency> Currencies { get; }

        int AccountId { get; }

        string Name { get; set; }
        AccountType SelectedAccountType { get; set; }
        Currency SelectedCurrency { get; set; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
