using System;
using System.Windows.Input;
using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class PaydayEventStartViewModel : IPaydayEventStartViewModel
    {
        private ILogger<PaydayEventStartViewModel> m_logger;
        private IBudgetService m_budgetService;

        public PaydayEventStartViewModel(
            ILogger<PaydayEventStartViewModel> logger,
            IBudgetService budgetService)
        {
            m_logger = logger;
            m_budgetService = budgetService;
        }

        public decimal AmountPaid { get; set; }
        public DateTime At { get; set; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        public void Setup(int budgetId)
        {
            Budget budget = m_budgetService.Get(budgetId);

            AmountPaid = budget.InitialTransaction.Amount;
            At = DateTime.Now;
        }

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
