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

        public IBalanceSheetItemViewModel CreateBalanceSheetItemViewModel(BalanceSheetItem item)
        {
            return m_serviceProvider.CreateInstance<BalanceSheetItemViewModel>(item);
        }

        public IBudgetItemViewModel CreateBudgetItemViewModel(Budget budget, Currency primaryCurrency)
        {
            return m_serviceProvider.CreateInstance<BudgetItemViewModel>(budget, primaryCurrency);
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

        public IForeignAmountViewModel CreateForeignAmountViewModel(
            decimal nativeAmount,
            string nativeCurrencyCode,
            string foreignCurrencyCode)
        {
            return m_serviceProvider.CreateInstance<ForeignAmountViewModel>(nativeAmount, nativeCurrencyCode, foreignCurrencyCode);
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
