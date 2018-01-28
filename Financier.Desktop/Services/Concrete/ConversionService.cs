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

        public IAccountLinkViewModel AccountLinkToViewModel(AccountLink accountLink)
        {
            IAccountLinkViewModel accountLinkViewModel
                = IoC.ServiceProvider.Instance.GetRequiredService<IAccountLinkViewModel>();

            accountLinkViewModel.Setup(accountLink);

            return accountLinkViewModel;
        }

        public IBudgetItemViewModel BudgetToItemViewModel(Budget budget, Currency currency)
        {
            IBudgetItemViewModel budgetItemViewModel
                = IoC.ServiceProvider.Instance.GetRequiredService<IBudgetItemViewModel>();

            budgetItemViewModel.Setup(budget, currency);

            return budgetItemViewModel;
        }

        public ITransactionItemViewModel TransactionToItemViewModel(Transaction transaction)
        {
            ITransactionItemViewModel transactionItemViewModel
                = IoC.ServiceProvider.Instance.GetRequiredService<ITransactionItemViewModel>();

            transactionItemViewModel.Setup(transaction);

            return transactionItemViewModel;
        }
    }
}
