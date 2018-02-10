using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModels
{
    public class AccountRelationshipItemViewModel : IAccountRelationshipItemViewModel
    {
        public int AccountRelationshipId => throw new NotImplementedException();

        public IAccountLinkViewModel SourceAccount => throw new NotImplementedException();

        public IAccountLinkViewModel DestinationAccount => throw new NotImplementedException();

        public AccountRelationshipType Type => throw new NotImplementedException();
    }
}
