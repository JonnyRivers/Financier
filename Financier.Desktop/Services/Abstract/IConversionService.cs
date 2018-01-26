using Financier.Desktop.ViewModels;
using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IConversionService
    {
        IAccountItemViewModel AccountToItemViewModel(Account account);
    }
}
