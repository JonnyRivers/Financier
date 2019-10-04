﻿using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Financier.Desktop.Tests.Concrete
{
    internal class FakeViewModelFactory : IViewModelFactory
    {
        public IAccountDetailsViewModel CreateAccountCreateViewModel()
        {
            throw new NotImplementedException();
        }

        public IAccountDetailsViewModel CreateAccountEditViewModel(int accountId)
        {
            throw new NotImplementedException();
        }

        public IAccountItemViewModel CreateAccountItemViewModel(Account account)
        {
            throw new NotImplementedException();
        }

        public IAccountLinkViewModel CreateAccountLinkViewModel(AccountLink accountLink)
        {
            return new StubAccountLinkViewModel(accountLink);
        }

        public IAccountListViewModel CreateAccountListViewModel()
        {
            throw new NotImplementedException();
        }

        public IAccountTransactionItemViewModel CreateAccountTransactionItemViewModel(Transaction transaction)
        {
            return new StubAccountTransactionItemViewModel(this, transaction);
        }

        public IAccountTransactionListViewModel CreateAccountTransactionListViewModel(int accountId)
        {
            throw new NotImplementedException();
        }

        public IAccountRelationshipDetailsViewModel CreateAccountRelationshipCreateViewModel(AccountRelationship hint)
        {
            throw new NotImplementedException();
        }

        public IAccountRelationshipDetailsViewModel CreateAccountRelationshipEditViewModel(int accountRelationshipId)
        {
            throw new NotImplementedException();
        }

        public IAccountRelationshipItemViewModel CreateAccountRelationshipItemViewModel(AccountRelationship accountRelationship)
        {
            return new StubAccountRelationshipItemViewModel(this, accountRelationship);
        }

        public IAccountRelationshipListViewModel CreateAccountRelationshipListViewModel()
        {
            throw new NotImplementedException();
        }

        public IAccountRelationshipTypeFilterViewModel CreateAccountRelationshipTypeFilterViewModel(AccountRelationshipType? type)
        {
            return new StubAccountRelationshipTypeFilterViewModel(type);
        }

        public IBudgetDetailsViewModel CreateBudgetCreateViewModel()
        {
            throw new NotImplementedException();
        }

        public IBudgetDetailsViewModel CreateBudgetEditViewModel(int budgetId)
        {
            throw new NotImplementedException();
        }

        public IBudgetItemViewModel CreateBudgetItemViewModel(Budget budget, Currency primaryCurrency)
        {
            throw new NotImplementedException();
        }

        public IBudgetListViewModel CreateBudgetListViewModel()
        {
            throw new NotImplementedException();
        }

        public IBudgetTransactionItemViewModel CreateBudgetTransactionItemViewModel(ObservableCollection<IAccountLinkViewModel> accountLinks, BudgetTransaction budgetTransaction, BudgetTransactionType type)
        {
            throw new NotImplementedException();
        }

        public IBudgetTransactionListViewModel CreateBudgetTransactionListViewModel(int budgetId)
        {
            throw new NotImplementedException();
        }

        public IMainViewModel CreateMainWindowViewModel()
        {
            throw new NotImplementedException();
        }

        public IPaydayEventStartViewModel CreatePaydayEventStartViewModel(int budgetId)
        {
            throw new NotImplementedException();
        }

        public ITransactionBatchCreateConfirmViewModel CreateTransactionBatchCreateConfirmViewModel(IEnumerable<Transaction> transactions)
        {
            throw new NotImplementedException();
        }

        public ITransactionDetailsViewModel CreateTransactionCreateViewModel(Transaction hint)
        {
            throw new NotImplementedException();
        }

        public ITransactionDetailsViewModel CreateTransactionEditViewModel(int transactionId)
        {
            throw new NotImplementedException();
        }

        public ITransactionItemViewModel CreateTransactionItemViewModel(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public ITransactionListViewModel CreateTransactionListViewModel()
        {
            throw new NotImplementedException();
        }

        public IReconcileBalanceViewModel CreateReconcileBalanceViewModel(int accountId)
        {
            throw new NotImplementedException();
        }

        public IForeignAmountViewModel CreateForeignAmountViewModel(
            decimal nativeAmount,
            string nativeCurrencyCode,
            string foreignCurrencyCode)
        {
            throw new NotImplementedException();
        }

        public IAccountTreeViewModel CreateAccountTreeViewModel()
        {
            throw new NotImplementedException();
        }

        public IAccountTreeItemViewModel CreateAccountTreeItemViewModel(Account account, IEnumerable<Transaction> transactions)
        {
            throw new NotImplementedException();
        }

        public IAccountTreeItemViewModel CreateAccountTreeItemViewModel(Account account, IEnumerable<Transaction> transactions, IEnumerable<IAccountTreeItemViewModel> childAccountVMs)
        {
            throw new NotImplementedException();
        }

        public IExceptionViewModel CreateUnhandledExceptionViewModel(Exception ex)
        {
            throw new NotImplementedException();
        }

        public IBalanceSheetViewModel CreateBalanceSheetViewModel()
        {
            throw new NotImplementedException();
        }

        public IBalanceSheetItemViewModel CreateBalanceSheetItemViewModel(BalanceSheetItem balanceSheetItem)
        {
            throw new NotImplementedException();
        }
    }
}
