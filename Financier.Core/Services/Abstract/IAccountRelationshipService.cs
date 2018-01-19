using Financier.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Financier.Services
{
    public interface IAccountRelationshipService
    {
        IEnumerable<AccountRelationship> GetAll(AccountRelationshipType type);
    }
}
