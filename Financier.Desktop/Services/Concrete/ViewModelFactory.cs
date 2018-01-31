using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public static class ServiceProviderExtensions
    {
        public static T CreateInstance<T>(this IServiceProvider serviceProvider, params object[] parameters)
        {
            return ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
        }
    }

    public class ViewModelFactory : IViewModelFactory
    {
        private readonly ILogger<ViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public ViewModelFactory(ILogger<ViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IMainWindowViewModel CreateMainWindowViewModel()
        {
            return m_serviceProvider.GetRequiredService<IMainWindowViewModel>();
        }

        public IAccountEditViewModel CreateAccountEditViewModel(int accountId)
        {
            return m_serviceProvider.CreateInstance<AccountEditViewModel>(accountId);
        }

        public IAccountItemViewModel CreateAccountItemViewModel(Account account)
        {
            return m_serviceProvider.CreateInstance<AccountItemViewModel>(account);
        }

        public IAccountLinkViewModel CreateAccountLinkViewModel(AccountLink accountLink)
        {
            return m_serviceProvider.CreateInstance<AccountLinkViewModel>(accountLink);
        }

        public IAccountListViewModel CreateAccountListViewModel()
        {
            return m_serviceProvider.GetRequiredService<AccountListViewModel>();
        }

        public IAccountTransactionListViewModel CreateAccountTransactionListViewModel(int accountId)
        {
            return m_serviceProvider.CreateInstance<AccountTransactionListViewModel>(accountId);
        }

        public IAccountTransactionItemViewModel CreateAccountTransactionItemViewModel(Transaction transaction)
        {
            return m_serviceProvider.CreateInstance<AccountTransactionItemViewModel>(transaction);
        }

        public IBudgetEditViewModel CreateBudgetEditViewModel(int budgetId)
        {
            return m_serviceProvider.CreateInstance<BudgetEditViewModel>(budgetId);
        }

        public IBudgetItemViewModel CreateBudgetItemViewModel(Budget budget, Currency primaryCurrency)
        {
            return m_serviceProvider.CreateInstance<BudgetItemViewModel>(budget, primaryCurrency);
        }

        public IBudgetListViewModel CreateBudgetListViewModel()
        {
            return m_serviceProvider.GetRequiredService<IBudgetListViewModel>();
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

        public ITransactionEditViewModel CreateTransactionCreateViewModel(Transaction hint)
        {
            return m_serviceProvider.CreateInstance<TransactionEditViewModel>(hint);
        }

        public ITransactionEditViewModel CreateTransactionEditViewModel(int transactionId)
        {
            return m_serviceProvider.CreateInstance<TransactionEditViewModel>(transactionId);
        }

        public ITransactionItemViewModel CreateTransactionItemViewModel(Transaction transaction)
        {
            return m_serviceProvider.CreateInstance<TransactionItemViewModel>(transaction);
        }

        public ITransactionListViewModel CreateTransactionListViewModel()
        {
            return m_serviceProvider.GetRequiredService<ITransactionListViewModel>();
        }
    }
}
