using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class BalanceSheetItemViewModelFactory : IBalanceSheetItemViewModelFactory
    {
        private readonly ILogger<BalanceSheetItemViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public BalanceSheetItemViewModelFactory(ILogger<BalanceSheetItemViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IBalanceSheetItemViewModel Create(BalanceSheetItem balanceSheetItem)
        {
            return m_serviceProvider.CreateInstance<BalanceSheetItemViewModel>(balanceSheetItem);
        }
    }

    public class BalanceSheetItemViewModel : BaseViewModel, IBalanceSheetItemViewModel
    {
        private ILogger<BalanceSheetItemViewModel> m_logger;

        public BalanceSheetItemViewModel(
            ILogger<BalanceSheetItemViewModel> logger,
            BalanceSheetItem balanceSheetItem)
        {
            m_logger = logger;

            Name = balanceSheetItem.Name;
            Balance = balanceSheetItem.Balance;
        }

        public string Name { get; }
        public decimal Balance { get; }
    }
}
