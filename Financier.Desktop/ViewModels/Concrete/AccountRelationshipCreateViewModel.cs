using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class AccountRelationshipCreateViewModel : IAccountRelationshipDetailsViewModel
    {
        public IEnumerable<AccountLink> Accounts => throw new NotImplementedException();

        public int AccountRelationshipId => throw new NotImplementedException();

        public AccountLink SourceAccount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AccountLink DestinationAccount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public AccountRelationshipType Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICommand OKCommand => throw new NotImplementedException();

        public ICommand CancelCommand => throw new NotImplementedException();

        public AccountRelationship ToAccountRelationship()
        {
            throw new NotImplementedException();
        }
    }
}
