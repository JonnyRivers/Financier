using Financier.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountRelationshipDetailsViewModel
    {
        AccountRelationship ToAccountRelationship();

        IEnumerable<AccountLink> Accounts { get; }
        IEnumerable<AccountRelationshipType> Types { get; }

        AccountLink SourceAccount { get; set; }
        AccountLink DestinationAccount { get; set; }
        AccountRelationshipType SelectedType { get; set; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
