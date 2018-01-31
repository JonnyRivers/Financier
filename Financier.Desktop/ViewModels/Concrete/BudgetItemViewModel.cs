using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class BudgetItemViewModel : IBudgetItemViewModel
    {
        private ILogger<BudgetItemViewModel> m_logger;

        public BudgetItemViewModel(
            ILogger<BudgetItemViewModel> logger,
            Budget budget, 
            Currency currency)
        {
            m_logger = logger;

            BudgetId = budget.BudgetId;
            Name = budget.Name;
            Period = budget.Period;
            InitialTransactionHint =
                $"{currency.Symbol}{budget.InitialTransaction.Amount} " +
                $"from {budget.InitialTransaction.CreditAccount.Name} " +
                $"to {budget.InitialTransaction.DebitAccount.Name}";
            Transactions = budget.Transactions.Count();
        }

        public int BudgetId { get; }
        public string Name { get; }
        public BudgetPeriod Period { get; }
        public string InitialTransactionHint { get; }
        public int Transactions { get; }
    }
}
