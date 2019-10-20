using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class BudgetListViewModelFactory : IBudgetListViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IBudgetService m_budgetService;
        private readonly ICurrencyService m_currencyService;
        private readonly ITransactionService m_transactionService;
        private readonly IDeleteConfirmationViewService m_deleteConfirmationViewService;
        private readonly IBudgetItemViewModelFactory m_budgetItemViewModelFactory;
        private readonly IBudgetCreateViewService m_budgetCreateViewService;
        private readonly IBudgetEditViewService m_budgetEditViewService;
        private readonly IPaydayEventViewService m_paydayEventViewService;
        private readonly ITransactionBatchCreateConfirmViewService m_transactionBatchCreateConfirmViewService;

        public BudgetListViewModelFactory(
            ILoggerFactory loggerFactory,
            IBudgetService budgetService,
            ICurrencyService currencyService,
            ITransactionService transactionService,
            IDeleteConfirmationViewService deleteConfirmationViewService,
            IBudgetItemViewModelFactory budgetItemViewModelFactory,
            IBudgetCreateViewService budgetCreateViewService,
            IBudgetEditViewService budgetEditViewService,
            IPaydayEventViewService paydayEventViewService,
            ITransactionBatchCreateConfirmViewService transactionBatchCreateConfirmViewService)
        {
            m_loggerFactory = loggerFactory;
            m_budgetService = budgetService;
            m_currencyService = currencyService;
            m_transactionService = transactionService;
            m_deleteConfirmationViewService = deleteConfirmationViewService;
            m_budgetItemViewModelFactory = budgetItemViewModelFactory;
            m_budgetCreateViewService = budgetCreateViewService;
            m_budgetEditViewService = budgetEditViewService;
            m_paydayEventViewService = paydayEventViewService;
            m_transactionBatchCreateConfirmViewService = transactionBatchCreateConfirmViewService;
        }

        public IBudgetListViewModel Create()
        {
            return new BudgetListViewModel(
                m_loggerFactory,
                m_budgetService,
                m_currencyService,
                m_transactionService,
                m_deleteConfirmationViewService,
                m_budgetItemViewModelFactory,
                m_budgetCreateViewService,
                m_budgetEditViewService,
                m_paydayEventViewService,
                m_transactionBatchCreateConfirmViewService);
        }
    }

    public class BudgetListViewModel : BaseViewModel, IBudgetListViewModel
    {
        private readonly ILogger<BudgetListViewModel> m_logger;
        private readonly IBudgetService m_budgetService;
        private readonly ICurrencyService m_currencyService;
        private readonly ITransactionService m_transactionService;
        private readonly IDeleteConfirmationViewService m_deleteConfirmationViewService;
        private readonly IBudgetItemViewModelFactory m_budgetItemViewModelFactory;
        private readonly IBudgetCreateViewService m_budgetCreateViewService;
        private readonly IBudgetEditViewService m_budgetEditViewService;
        private readonly IPaydayEventViewService m_paydayEventViewService;
        private readonly ITransactionBatchCreateConfirmViewService m_transactionBatchCreateConfirmViewService;

        private ObservableCollection<IBudgetItemViewModel> m_budgets;
        private IBudgetItemViewModel m_selectedBudget;

        public BudgetListViewModel(
            ILoggerFactory loggerFactory,
            IBudgetService budgetService,
            ICurrencyService currencyService,
            ITransactionService transactionService,
            IDeleteConfirmationViewService deleteConfirmationViewService,
            IBudgetItemViewModelFactory budgetItemViewModelFactory,
            IBudgetCreateViewService budgetCreateViewService,
            IBudgetEditViewService budgetEditViewService,
            IPaydayEventViewService paydayEventViewService,
            ITransactionBatchCreateConfirmViewService transactionBatchCreateConfirmViewService)
        {
            m_logger = loggerFactory.CreateLogger<BudgetListViewModel>();
            m_budgetService = budgetService;
            m_currencyService = currencyService;
            m_transactionService = transactionService;
            m_deleteConfirmationViewService = deleteConfirmationViewService;
            m_budgetItemViewModelFactory = budgetItemViewModelFactory;
            m_budgetCreateViewService = budgetCreateViewService;
            m_budgetEditViewService = budgetEditViewService;
            m_paydayEventViewService = paydayEventViewService;
            m_transactionBatchCreateConfirmViewService = transactionBatchCreateConfirmViewService;

            PopulateBudgets();
        }

        private void PopulateBudgets()
        {
            Currency primaryCurrency = m_currencyService.GetPrimary();
            IEnumerable<Budget> budgets = m_budgetService.GetAll();
            IEnumerable<IBudgetItemViewModel> budgetViewModels =
                budgets.Select(b => m_budgetItemViewModelFactory.Create(b, primaryCurrency));

            Budgets = new ObservableCollection<IBudgetItemViewModel>(budgetViewModels);
        }

        public ObservableCollection<IBudgetItemViewModel> Budgets
        {
            get { return m_budgets; }
            set
            {
                if (m_budgets != value)
                {
                    m_budgets = value;

                    OnPropertyChanged();
                }
            }
        }

        public IBudgetItemViewModel SelectedBudget
        {
            get { return m_selectedBudget; }
            set
            {
                if (m_selectedBudget != value)
                {
                    m_selectedBudget = value;

                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand CreateCommand => new RelayCommand(CreateExecute);
        public ICommand EditCommand => new RelayCommand(EditExecute, EditCanExecute);
        public ICommand DeleteCommand => new RelayCommand(DeleteExecute, DeleteCanExecute);
        public ICommand PaydayCommand => new RelayCommand(PaydayExecute, PaydayCanExecute);

        private void CreateExecute(object obj)
        {
            Budget newBudget;
            if (m_budgetCreateViewService.Show(out newBudget))
            {
                Currency primaryCurrency = m_currencyService.GetPrimary();
                IBudgetItemViewModel newBudgetViewModel =
                    m_budgetItemViewModelFactory.Create(newBudget, primaryCurrency);
                Budgets.Add(newBudgetViewModel);
            }
        }

        private void EditExecute(object obj)
        {
            Budget updatedBudget;
            if (m_budgetEditViewService.Show(SelectedBudget.BudgetId, out updatedBudget))
            {
                Currency primaryCurrency = m_currencyService.GetPrimary();

                Budgets.Remove(SelectedBudget);
                SelectedBudget = m_budgetItemViewModelFactory.Create(updatedBudget, primaryCurrency);
                Budgets.Add(SelectedBudget);
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedBudget != null);
        }

        private void DeleteExecute(object obj)
        {
            if (m_deleteConfirmationViewService.Show("budget"))
            {
                // Update model
                m_budgetService.Delete(SelectedBudget.BudgetId);

                // Update view model
                Budgets.Remove(SelectedBudget);
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedBudget != null);
        }

        private void PaydayExecute(object obj)
        {
            PaydayStart paydayStart;
            if (m_paydayEventViewService.Show(SelectedBudget.BudgetId, out paydayStart))
            {
                IEnumerable<Transaction> prospectiveTransasctions =
                    m_budgetService.MakePaydayTransactions(SelectedBudget.BudgetId, paydayStart);

                if (m_transactionBatchCreateConfirmViewService.Show(prospectiveTransasctions))
                {
                    m_transactionService.CreateMany(prospectiveTransasctions);
                }
            }
        }

        private bool PaydayCanExecute(object obj)
        {
            return (SelectedBudget != null);
        }
    }
}
