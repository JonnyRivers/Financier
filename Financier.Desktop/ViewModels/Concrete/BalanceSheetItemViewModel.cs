using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
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
