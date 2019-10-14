using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Windows;

namespace Financier.Desktop.Services
{
    public class ViewService : IViewService
    {
        private readonly ILogger<ViewService> m_logger;
        private readonly IViewModelFactory m_viewModelFactory;

        public ViewService(ILogger<ViewService> logger, IViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public bool OpenBudgetCreateView(out Budget budget)
        {
            budget = null;

            IBudgetDetailsViewModel viewModel = m_viewModelFactory.CreateBudgetCreateViewModel();
            var window = new BudgetDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                budget = viewModel.ToBudget();
                return true;
            }

            return false;
        }

        public bool OpenBudgetEditView(int budgetId, out Budget budget)
        {
            budget = null;

            IBudgetDetailsViewModel viewModel = m_viewModelFactory.CreateBudgetEditViewModel(budgetId);
            var window = new BudgetDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                budget = viewModel.ToBudget();
                return true;
            }

            return false;
        }

        public bool OpenForeignAmountView(
            decimal nativeAmount,
            string nativeCurrencyCode,
            string foreignCurrencyCode, 
            out decimal exchangedAmount)
        {
            exchangedAmount = 0m;

            IForeignAmountViewModel viewModel = m_viewModelFactory.CreateForeignAmountViewModel(
                nativeAmount,
                nativeCurrencyCode,
                foreignCurrencyCode
            );
            var window = new ForeignAmountWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                exchangedAmount = viewModel.NativeAmount;
                return true;
            }

            return false;
        }

        public bool OpenPaydayEventStartView(int budgetId, out PaydayStart paydayStart)
        {
            paydayStart = null;

            IPaydayEventStartViewModel viewModel = m_viewModelFactory.CreatePaydayEventStartViewModel(budgetId);
            var window = new PaydayEventStartWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                paydayStart = viewModel.ToPaydayStart();
                return true;
            }
            
            return false;
        }

        public void OpenNoPendingCreditCardTransactionsView(string accountName)
        {
            MessageBox.Show(
               $"There are no transactions to pay off from account '{accountName}'.",
               $"Nothing to pay off",
               MessageBoxButton.OK
            );
        }

        public bool OpenTransactionBatchCreateConfirmView(IEnumerable<Transaction> transactions)
        {
            ITransactionBatchCreateConfirmViewModel viewModel =
                m_viewModelFactory.CreateTransactionBatchCreateConfirmViewModel(transactions);
            var window = new TransactionBatchCreateConfirmWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }
        
        public bool OpenTransactionCreateView(Transaction hint, out Transaction transaction)
        {
            transaction = null;

            ITransactionDetailsViewModel viewModel = m_viewModelFactory.CreateTransactionCreateViewModel(hint);
            var window = new TransactionDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                transaction = viewModel.ToTransaction();
                return true;
            }

            return false;
        }

        public bool OpenReconcileBalanceView(int accountId, out Transaction transaction)
        {
            transaction = null;

            IReconcileBalanceViewModel viewModel = m_viewModelFactory.CreateReconcileBalanceViewModel(accountId);
            var window = new ReconcileBalanceWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                transaction = viewModel.ToTransaction();
                return true;
            }

            return false;
        }

        public bool OpenTransactionEditView(int transactionId, out Transaction updatedTransaction)
        {
            updatedTransaction = null;

            ITransactionDetailsViewModel viewModel = m_viewModelFactory.CreateTransactionEditViewModel(transactionId);
            var window = new TransactionDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue)
            {
                updatedTransaction = viewModel.ToTransaction();
                return result.Value;
            }

            return false;
        }
    }
}
