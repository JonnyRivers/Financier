using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IAccountRelationshipCreateViewService
    {
        bool Show(AccountRelationship hint, out AccountRelationship newAccountRelationship);
    }
}
