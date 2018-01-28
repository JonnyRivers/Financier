using Financier.Desktop.ViewModels;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class ConversionService : IConversionService
    {
        private ILogger<ConversionService> m_logger;

        public ConversionService(ILogger<ConversionService> logger)
        {
            m_logger = logger;
        }

        public IAccountItemViewModel AccountToItemViewModel(Account account)
        {
            IAccountItemViewModel viewModel
                = IoC.ServiceProvider.Instance.GetRequiredService<IAccountItemViewModel>();

            viewModel.Setup(account);

            return viewModel;
        }

        public IAccountLinkViewModel AccountLinkToViewModel(AccountLink accountLink)
        {
            IAccountLinkViewModel viewModel
                = IoC.ServiceProvider.Instance.GetRequiredService<IAccountLinkViewModel>();

            viewModel.Setup(accountLink);

            return viewModel;
        }

        public IAccountTransactionItemViewModel TransactionToAccountTransactionItemViewModel(Transaction transaction)
        {
            IAccountTransactionItemViewModel viewModel
                = IoC.ServiceProvider.Instance.GetRequiredService<IAccountTransactionItemViewModel>();

            viewModel.Setup(transaction);

            return viewModel;
        }

        public IBudgetItemViewModel BudgetToItemViewModel(Budget budget, Currency currency)
        {
            IBudgetItemViewModel viewModel
                = IoC.ServiceProvider.Instance.GetRequiredService<IBudgetItemViewModel>();

            viewModel.Setup(budget, currency);

            return viewModel;
        }

        public ITransactionItemViewModel TransactionToItemViewModel(Transaction transaction)
        {
            ITransactionItemViewModel viewModel
                = IoC.ServiceProvider.Instance.GetRequiredService<ITransactionItemViewModel>();

            viewModel.Setup(transaction);

            return viewModel;
        }
    }
}
