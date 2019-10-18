using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class BudgetItemViewModelFactory : IBudgetItemViewModelFactory
    {
        private readonly ILogger<BudgetItemViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public BudgetItemViewModelFactory(ILogger<BudgetItemViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IBudgetItemViewModel Create(Budget budget, Currency currency)
        {
            return m_serviceProvider.CreateInstance<BudgetItemViewModel>(budget, currency);
        }
    }

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
