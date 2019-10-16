using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class PaydayEventViewService : IPaydayEventViewService
    {
        private readonly ILogger<PaydayEventViewService> m_logger;
        private readonly IViewModelFactory m_viewModelFactory;

        public PaydayEventViewService(ILogger<PaydayEventViewService> logger, IViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public bool Show(int budgetId, out PaydayStart paydayStart)
        {
            paydayStart = null;

            IPaydayEventStartViewModel viewModel = m_viewModelFactory.CreatePaydayEventStartViewModel(budgetId);
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
