using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class ForeignAmountViewService : IForeignAmountViewService
    {
        private readonly ILogger<ForeignAmountViewService> m_logger;
        private readonly IForeignAmountViewModelFactory m_foreignAmountViewModelFactory;

        public ForeignAmountViewService(ILogger<ForeignAmountViewService> logger, IForeignAmountViewModelFactory foreignAmountViewModelFactory)
        {
            m_logger = logger;
            m_foreignAmountViewModelFactory = foreignAmountViewModelFactory;
        }

        public bool Show(
            decimal nativeAmount,
            string nativeCurrencyCode,
            string foreignCurrencyCode, 
            out decimal exchangedAmount)
        {
            exchangedAmount = 0m;

            IForeignAmountViewModel viewModel = m_foreignAmountViewModelFactory.Create(
                nativeAmount,
                nativeCurrencyCode,
                foreignCurrencyCode
            );
            var window = new ForeignAmountWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                exchangedAmount = viewModel.NativeAmount;
                return true;
            }

            return false;
        }
    }
}
