using System;
using System.Windows.Input;
using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class PaydayEventStartViewModelFactory : IPaydayEventStartViewModelFactory
    {
        private readonly ILogger<PaydayEventStartViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public PaydayEventStartViewModelFactory(
            ILogger<PaydayEventStartViewModelFactory> logger,
            IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IPaydayEventStartViewModel Create(int budgetId)
        {
            return m_serviceProvider.CreateInstance<PaydayEventStartViewModel>(budgetId);
        }
    }

    public class PaydayEventStartViewModel : IPaydayEventStartViewModel
    {
        private ILogger<PaydayEventStartViewModel> m_logger;
        private IBudgetService m_budgetService;

        public PaydayEventStartViewModel(
            ILogger<PaydayEventStartViewModel> logger,
            IBudgetService budgetService,
            int budgetId)
        {
            m_logger = logger;
            m_budgetService = budgetService;

            Budget budget = m_budgetService.Get(budgetId);

            AmountPaid = budget.InitialTransaction.Amount;
            At = DateTime.Now;
        }

        public decimal AmountPaid { get; set; }
        public DateTime At { get; set; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        public PaydayStart ToPaydayStart()
        {
            return new PaydayStart
            {
                AmountPaid = AmountPaid,
                At = At
            };
        }

        private void OKExecute(object obj)
        {

        }

        private void CancelExecute(object obj)
        {

        }
    }
}
