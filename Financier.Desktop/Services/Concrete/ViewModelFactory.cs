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

        public IAccountRelationshipDetailsViewModel CreateAccountRelationshipCreateViewModel(AccountRelationship hint)
        {
            return m_serviceProvider.CreateInstance<AccountRelationshipCreateViewModel>(hint);
        }

        public IAccountRelationshipDetailsViewModel CreateAccountRelationshipEditViewModel(int accountRelationshipId)
        {
            return m_serviceProvider.CreateInstance<AccountRelationshipEditViewModel>(accountRelationshipId);
        }

        public IAccountRelationshipItemViewModel CreateAccountRelationshipItemViewModel(AccountRelationship accountRelationship)
        {
            return m_serviceProvider.CreateInstance<AccountRelationshipItemViewModel>(accountRelationship);
        }

        public IAccountRelationshipTypeFilterViewModel CreateAccountRelationshipTypeFilterViewModel(AccountRelationshipType? type)
        {
            return new AccountRelationshipTypeFilterViewModel(type);
        }

        public IBalanceSheetItemViewModel CreateBalanceSheetItemViewModel(BalanceSheetItem item)
        {
            return m_serviceProvider.CreateInstance<BalanceSheetItemViewModel>(item);
        }

        public IBudgetDetailsViewModel CreateBudgetCreateViewModel()
        {
            return m_serviceProvider.CreateInstance<BudgetCreateViewModel>();
        }

        public IBudgetDetailsViewModel CreateBudgetEditViewModel(int budgetId)
        {
            return m_serviceProvider.CreateInstance<BudgetEditViewModel>(budgetId);
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

        public ITransactionDetailsViewModel CreateTransactionCreateViewModel(Transaction hint)
        {
            return m_serviceProvider.CreateInstance<TransactionCreateViewModel>(hint);
        }

        public ITransactionDetailsViewModel CreateTransactionEditViewModel(int transactionId)
        {
            return m_serviceProvider.CreateInstance<TransactionEditViewModel>(transactionId);
        }

        public IReconcileBalanceViewModel CreateReconcileBalanceViewModel(int accountId)
        {
            return m_serviceProvider.CreateInstance<ReconcileBalanceViewModel>(accountId);
        }

        public ITransactionItemViewModel CreateTransactionItemViewModel(Transaction transaction)
        {
            return m_serviceProvider.CreateInstance<TransactionItemViewModel>(transaction);
        }
    }
}
