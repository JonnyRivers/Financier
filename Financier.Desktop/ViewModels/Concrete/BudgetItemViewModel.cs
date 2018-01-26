using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class BudgetItemViewModel : IBudgetItemViewModel
    {
        private ILogger<BudgetItemViewModel> m_logger;

        public BudgetItemViewModel(
            ILogger<BudgetItemViewModel> logger)
        {
            m_logger = logger;
        }

        public void Setup(Budget budget, Currency currency)
        {
            BudgetId = budget.BudgetId;
            Name = budget.Name;
            Period = budget.Period;
            InitialTransactionHint =
                $"{currency.Symbol}{budget.InitialTransaction.Amount} " +
                $"from {budget.InitialTransaction.CreditAccount.Name} " +
                $"to {budget.InitialTransaction.DebitAccount.Name}";
            Transactions = budget.Transactions.Count();
        }

        public int BudgetId { get; private set; }
        public string Name { get; private set; }
        public BudgetPeriod Period { get; private set; }
        public string InitialTransactionHint { get; private set; }
        public int Transactions { get; private set; }
    }
}
