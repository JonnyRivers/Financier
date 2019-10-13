using Financier.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountTreeItemViewModelFactory
    {
        IAccountTreeItemViewModel Create(Account account, IEnumerable<Transaction> transactions);
        IAccountTreeItemViewModel Create(Account account, IEnumerable<Transaction> transactions, IEnumerable<IAccountTreeItemViewModel> childAccountVMs);
    }

    public interface IAccountTreeItemViewModel
    {
        int AccountId { get; }
        decimal Balance { get; }
        string CurrencySymbol { get; }
        ObservableCollection<IAccountTreeItemViewModel> ChildAccountItems { get; }
        string Name { get; }
        AccountSubType SubType { get; }
        AccountType Type { get; }
    }
}
