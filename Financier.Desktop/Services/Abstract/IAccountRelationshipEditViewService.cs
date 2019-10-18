using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IAccountRelationshipEditViewService
    {
        bool Show(int accountRelationshipId, out AccountRelationship updatedAccountRelationship);
    }
}
