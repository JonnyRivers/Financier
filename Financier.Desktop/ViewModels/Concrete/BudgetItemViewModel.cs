using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class BudgetItemViewModelFactory : IBudgetItemViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;

        public BudgetItemViewModelFactory(ILoggerFactory loggerFactory)
        {
            m_loggerFactory = loggerFactory;
        }

        public IBudgetItemViewModel Create(Budget budget, Currency currency)
        {
            return new BudgetItemViewModel(m_loggerFactory, budget, currency);
        }
    }

    public class BudgetItemViewModel : IBudgetItemViewModel
    {
        private ILogger<BudgetItemViewModel> m_logger;

        public BudgetItemViewModel(
            ILoggerFactory loggerFactory,
            Budget budget, 
            Currency currency)
        {
            m_logger = loggerFactory.CreateLogger<BudgetItemViewModel>();

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
