using Financier.Desktop.ViewModels;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.Services
{
    public class ConversionService : IConversionService
    {
        private ILogger<ConversionService> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public ConversionService(ILogger<ConversionService> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IAccountItemViewModel AccountToItemViewModel(Account account)
        {
            IAccountItemViewModel viewModel
                = m_serviceProvider.GetRequiredService<IAccountItemViewModel>();

            viewModel.Setup(account);

            return viewModel;
        }

        public IAccountLinkViewModel AccountLinkToViewModel(AccountLink accountLink)
        {
            IAccountLinkViewModel viewModel
                = m_serviceProvider.GetRequiredService<IAccountLinkViewModel>();

            viewModel.Setup(accountLink);

            return viewModel;
        }

        public IAccountTransactionItemViewModel TransactionToAccountTransactionItemViewModel(Transaction transaction)
        {
            IAccountTransactionItemViewModel viewModel
                = m_serviceProvider.GetRequiredService<IAccountTransactionItemViewModel>();

            viewModel.Setup(transaction);

            return viewModel;
        }

        public IBudgetItemViewModel BudgetToItemViewModel(Budget budget, Currency currency)
        {
            IBudgetItemViewModel viewModel
                = m_serviceProvider.GetRequiredService<IBudgetItemViewModel>();

            viewModel.Setup(budget, currency);

            return viewModel;
        }

        public ITransactionItemViewModel TransactionToItemViewModel(Transaction transaction)
        {
            ITransactionItemViewModel viewModel
                = m_serviceProvider.GetRequiredService<ITransactionItemViewModel>();

            viewModel.Setup(transaction);

            return viewModel;
        }
    }
}
