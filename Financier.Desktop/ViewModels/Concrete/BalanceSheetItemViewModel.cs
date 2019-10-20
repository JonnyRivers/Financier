using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class BalanceSheetItemViewModelFactory : IBalanceSheetItemViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;

        public BalanceSheetItemViewModelFactory(ILoggerFactory loggerFactory)
        {
            m_loggerFactory = loggerFactory;
        }

        public IBalanceSheetItemViewModel Create(BalanceSheetItem balanceSheetItem)
        {
            return new BalanceSheetItemViewModel(
                m_loggerFactory,
                balanceSheetItem);
        }
    }

    public class BalanceSheetItemViewModel : BaseViewModel, IBalanceSheetItemViewModel
    {
        private ILogger<BalanceSheetItemViewModel> m_logger;

        public BalanceSheetItemViewModel(
            ILoggerFactory loggerFactory,
            BalanceSheetItem balanceSheetItem)
        {
            m_logger = loggerFactory.CreateLogger<BalanceSheetItemViewModel>();

            Name = balanceSheetItem.Name;
            Balance = balanceSheetItem.Balance;
        }

        public string Name { get; }
        public decimal Balance { get; }
    }
}
