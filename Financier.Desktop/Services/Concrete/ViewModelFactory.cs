﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
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

        public IMainWindowViewModel CreateMainWindowViewModel()
        {
            return new MainWindowViewModel(
                m_serviceProvider.GetRequiredService<ILogger<MainWindowViewModel>>(),
                m_serviceProvider.GetRequiredService<IViewService>()
            );
        }

        public IAccountEditViewModel CreateAccountEditViewModel(int accountId)
        {
            return new AccountEditViewModel(
                m_serviceProvider.GetRequiredService<ILogger<AccountEditViewModel>>(),
                m_serviceProvider.GetRequiredService<IAccountService>(),
                m_serviceProvider.GetRequiredService<ICurrencyService>(),
                accountId
            );
        }

        public IAccountLinkViewModel CreateAccountLinkViewModel(AccountLink accountLink)
        {
            return new AccountLinkViewModel(
                m_serviceProvider.GetRequiredService<ILogger<AccountLinkViewModel>>()
            );
        }

        public IAccountListViewModel CreateAccountListViewModel()
        {
            return new AccountListViewModel(
                m_serviceProvider.GetRequiredService<ILogger<AccountListViewModel>>(),
                m_serviceProvider.GetRequiredService<IAccountService>(),
                m_serviceProvider.GetRequiredService<IConversionService>(),
                m_serviceProvider.GetRequiredService<ITransactionService>(),
                m_serviceProvider.GetRequiredService<ITransactionRelationshipService>(),
                m_serviceProvider.GetRequiredService<IViewService>()
            );
        }

        public IAccountTransactionListViewModel CreateAccountTransactionListViewModel(int accountId)
        {
            return new AccountTransactionListViewModel(
                m_serviceProvider.GetRequiredService<ILogger<AccountTransactionListViewModel>>(),
                m_serviceProvider.GetRequiredService<IAccountService>(),
                m_serviceProvider.GetRequiredService<IConversionService>(),
                m_serviceProvider.GetRequiredService<ITransactionService>(),
                m_serviceProvider.GetRequiredService<IViewService>()
            );
        }

        public IBudgetEditViewModel CreateBudgetEditViewModel(int budgetId)
        {
            return new BudgetEditViewModel(
                m_serviceProvider.GetRequiredService<ILogger<BudgetEditViewModel>>(),
                m_serviceProvider.GetRequiredService<IBudgetService>(),
                m_serviceProvider.GetRequiredService<IViewModelFactory>(),
                budgetId
            );
        }

        public IBudgetListViewModel CreateBudgetListViewModel()
        {
            return new BudgetListViewModel(
                m_serviceProvider.GetRequiredService<ILogger<BudgetListViewModel>>(),
                m_serviceProvider.GetRequiredService<IBudgetService>(),
                m_serviceProvider.GetRequiredService<IConversionService>(),
                m_serviceProvider.GetRequiredService<ICurrencyService>(),
                m_serviceProvider.GetRequiredService<ITransactionService>(),
                m_serviceProvider.GetRequiredService<IViewService>()
            );
        }

        public IBudgetTransactionListViewModel CreateBudgetTransactionListViewModel(int budgetId)
        {
            return new BudgetTransactionListViewModel(
                m_serviceProvider.GetRequiredService<ILogger<BudgetTransactionListViewModel>>(),
                m_serviceProvider.GetRequiredService<IAccountService>(),
                m_serviceProvider.GetRequiredService<IBudgetService>(),
                m_serviceProvider.GetRequiredService<IViewModelFactory>(),
                m_serviceProvider.GetRequiredService<IViewService>(),
                budgetId
            );
        }

        public IBudgetTransactionItemViewModel CreateBudgetTransactionItemViewModel(
            ObservableCollection<IAccountLinkViewModel> accountLinks,
            BudgetTransaction budgetTransaction,
            BudgetTransactionType type)
        {
            return new BudgetTransactionItemViewModel(
                m_serviceProvider.GetRequiredService<ILogger<BudgetTransactionItemViewModel>>(),
                accountLinks,
                budgetTransaction,
                type
            );
        }

        public IPaydayEventStartViewModel CreatePaydayEventStartViewModel(int budgetId)
        {
            return new PaydayEventStartViewModel(
                   m_serviceProvider.GetRequiredService<ILogger<PaydayEventStartViewModel>>(),
                   m_serviceProvider.GetRequiredService<IBudgetService>(),
                   budgetId
            );
        }

        public ITransactionBatchCreateConfirmViewModel CreateTransactionBatchCreateConfirmViewModel(
            IEnumerable<Transaction> transactions)
        {
            return new TransactionBatchCreateConfirmViewModel(
                m_serviceProvider.GetRequiredService<ILogger<TransactionBatchCreateConfirmViewModel>>(),
                m_serviceProvider.GetRequiredService<IConversionService>(),
                transactions
            );
        }

        public ITransactionEditViewModel CreateTransactionCreateViewModel(Transaction hint)
        {
            return new TransactionEditViewModel(
                m_serviceProvider.GetRequiredService<ILogger<TransactionEditViewModel>>(),
                m_serviceProvider.GetRequiredService<IAccountService>(),
                m_serviceProvider.GetRequiredService<IConversionService>(),
                m_serviceProvider.GetRequiredService<ITransactionService>(),
                hint
            );
        }

        public ITransactionEditViewModel CreateTransactionEditViewModel(int transactionId)
        {
            return new TransactionEditViewModel(
                m_serviceProvider.GetRequiredService<ILogger<TransactionEditViewModel>>(),
                m_serviceProvider.GetRequiredService<IAccountService>(),
                m_serviceProvider.GetRequiredService<IConversionService>(),
                m_serviceProvider.GetRequiredService<ITransactionService>(),
                transactionId
            );
        }

        public ITransactionListViewModel CreateTransactionListViewModel()
        {
            return new TransactionListViewModel(
                m_serviceProvider.GetRequiredService<ILogger<TransactionListViewModel>>(),
                m_serviceProvider.GetRequiredService<IAccountService>(),
                m_serviceProvider.GetRequiredService<IConversionService>(),
                m_serviceProvider.GetRequiredService<ITransactionService>(),
                m_serviceProvider.GetRequiredService<IViewService>()
            );
        }
    }
}
