namespace Financier.Desktop.ViewModels
{
    public class AccountEditViewModel : IAccountEditViewModel
    {
        public AccountEditViewModel(
            IAccountOverviewViewModel accountOverviewViewModel
            
        )
        {
            AccountOverviewViewModel = accountOverviewViewModel;
        }

        public IAccountOverviewViewModel AccountOverviewViewModel { get; }
    }
}
