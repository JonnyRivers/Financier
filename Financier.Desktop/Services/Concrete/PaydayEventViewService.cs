using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class PaydayEventViewService : IPaydayEventViewService
    {
        private readonly ILogger<PaydayEventViewService> m_logger;
        private readonly IPaydayEventStartViewModelFactory m_paydayEventStartViewModelFactory;

        public PaydayEventViewService(
            ILoggerFactory loggerFactory,
            IPaydayEventStartViewModelFactory paydayEventStartViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<PaydayEventViewService>();
            m_paydayEventStartViewModelFactory = paydayEventStartViewModelFactory;
        }

        public bool Show(int budgetId, out PaydayStart paydayStart)
        {
            paydayStart = null;

            IPaydayEventStartViewModel viewModel = m_paydayEventStartViewModelFactory.Create(budgetId);
            var window = new PaydayEventStartWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                paydayStart = viewModel.ToPaydayStart();
                return true;
            }
            
            return false;
        }
    }
}
