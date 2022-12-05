using System;
using System.Windows.Input;
using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class PaydayEventStartViewModelFactory : IPaydayEventStartViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IBudgetService m_budgetService;

        public PaydayEventStartViewModelFactory(
            ILoggerFactory loggerFactory,
            IBudgetService budgetService)
        {
            m_loggerFactory = loggerFactory;
            m_budgetService = budgetService;
        }

        public IPaydayEventStartViewModel Create(int budgetId)
        {
            return new PaydayEventStartViewModel(
                m_loggerFactory,
                m_budgetService,
                budgetId);
        }
    }

    public class PaydayEventStartViewModel : IPaydayEventStartViewModel
    {
        private readonly ILogger<PaydayEventStartViewModel> m_logger;
        private readonly IBudgetService m_budgetService;

        public PaydayEventStartViewModel(
            ILoggerFactory loggerFactory,
            IBudgetService budgetService,
            int budgetId)
        {
            m_logger = loggerFactory.CreateLogger<PaydayEventStartViewModel>();
            m_budgetService = budgetService;

            Budget budget = m_budgetService.Get(budgetId);

            AmountPaid = budget.InitialTransaction.Amount;
            At = DateTime.Now;
            IncludeSurplus = false;
        }

        public decimal AmountPaid { get; set; }
        public DateTime At { get; set; }
        public bool IncludeSurplus { get; set; }

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
