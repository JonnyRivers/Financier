using System.Collections.Generic;

namespace Financier.Services
{
    public interface IAccountRelationshipService
    {
        void Create(AccountRelationship accountRelationship);
        void Delete(int accountRelationshipId);
        AccountRelationship Get(int accountRelationshipId);
        IEnumerable<AccountRelationship> GetAll();
        void Update(AccountRelationship accountRelationship);
    }
}
