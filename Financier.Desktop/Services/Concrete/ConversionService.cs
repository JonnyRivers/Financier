using Financier.Desktop.ViewModels;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Financier.Desktop.Services
{
    public class ConversionService : IConversionService
    {
        public IAccountItemViewModel AccountToItemViewModel(Account account)
        {
            IAccountItemViewModel accountItemViewModel 
                = IoC.ServiceProvider.Instance.GetRequiredService<IAccountItemViewModel>();

            accountItemViewModel.Setup(account);

            return accountItemViewModel;
        }
    }
}
