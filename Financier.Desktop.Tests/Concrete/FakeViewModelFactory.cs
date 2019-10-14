using Financier.Desktop.Services;
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

        public IBudgetTransactionItemViewModel CreateBudgetTransactionItemViewModel(ObservableCollection<IAccountLinkViewModel> accountLinks, BudgetTransaction budgetTransaction, BudgetTransactionType type)
        {
            throw new NotImplementedException();
        }

        public IBudgetTransactionListViewModel CreateBudgetTransactionListViewModel(int budgetId)
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

        public IBalanceSheetItemViewModel CreateBalanceSheetItemViewModel(BalanceSheetItem balanceSheetItem)
        {
            throw new NotImplementedException();
        }
    }
}
