using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly ILogger<ViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public ViewModelFactory(ILogger<ViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IBudgetTransactionListViewModel CreateBudgetTransactionListViewModel(int budgetId)
        {
            return m_serviceProvider.CreateInstance<BudgetTransactionListViewModel>(budgetId);
        }

        public IBudgetTransactionItemViewModel CreateBudgetTransactionItemViewModel(
            ObservableCollection<IAccountLinkViewModel> accountLinks,
            BudgetTransaction budgetTransaction,
            BudgetTransactionType type)
        {
            return m_serviceProvider.CreateInstance<BudgetTransactionItemViewModel>(accountLinks, budgetTransaction, type);
        }

        public IPaydayEventStartViewModel CreatePaydayEventStartViewModel(int budgetId)
        {
            return m_serviceProvider.CreateInstance<PaydayEventStartViewModel>(budgetId);
        }

        public ITransactionBatchCreateConfirmViewModel CreateTransactionBatchCreateConfirmViewModel(
            IEnumerable<Transaction> transactions)
        {
            return m_serviceProvider.CreateInstance<TransactionBatchCreateConfirmViewModel>(transactions);
        }

        public IReconcileBalanceViewModel CreateReconcileBalanceViewModel(int accountId)
        {
            return m_serviceProvider.CreateInstance<ReconcileBalanceViewModel>(accountId);
        }
    }
}
