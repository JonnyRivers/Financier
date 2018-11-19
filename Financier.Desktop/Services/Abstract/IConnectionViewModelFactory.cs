using Financier.Desktop.ViewModels;

namespace Financier.Desktop.Services
{
    public interface IConnectionViewModelFactory
    {
        IConnectionWindowViewModel CreateConnectionWindowViewModel();
    }
}
