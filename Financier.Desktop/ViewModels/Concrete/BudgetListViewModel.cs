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
    public class BudgetListViewModel : BaseViewModel, IBudgetListViewModel
    {
        private ILogger<BudgetListViewModel> m_logger;
        private IBudgetService m_budgetService;
        private ICurrencyService m_currencyService;
        private ITransactionService m_transactionService;
        private IViewModelFactory m_viewModelFactory;
        private IViewService m_viewService;

        private ObservableCollection<IBudgetItemViewModel> m_budgets;
        private IBudgetItemViewModel m_selectedBudget;

        public BudgetListViewModel(
            ILogger<BudgetListViewModel> logger,
            IBudgetService budgetService,
            ICurrencyService currencyService,
            ITransactionService transactionService,
            IViewModelFactory viewModelFactory,
            IViewService viewService)
        {
            m_logger = logger;
            m_budgetService = budgetService;
            m_currencyService = currencyService;
            m_transactionService = transactionService;
            m_viewModelFactory = viewModelFactory;
            m_viewService = viewService;

            PopulateBudgets();
        }

        private void PopulateBudgets()
        {
            Currency primaryCurrency = m_currencyService.GetPrimary();
            IEnumerable<Budget> budgets = m_budgetService.GetAll();
            IEnumerable<IBudgetItemViewModel> budgetViewModels =
                budgets.Select(b => m_viewModelFactory.CreateBudgetItemViewModel(b, primaryCurrency));

            Budgets = new ObservableCollection<IBudgetItemViewModel>(budgetViewModels.OrderBy(b => b.Name));
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
            if (m_viewService.OpenBudgetCreateView(out newBudget))
            {
                Currency primaryCurrency = m_currencyService.GetPrimary();
                IBudgetItemViewModel newBudgetViewModel = 
                    m_viewModelFactory.CreateBudgetItemViewModel(newBudget, primaryCurrency);
                Budgets.Add(newBudgetViewModel);
                // TODO: Is there a better way to maintain ObservableCollection<T> sorting?
                // https://github.com/JonnyRivers/Financier/issues/29
                Budgets = new ObservableCollection<IBudgetItemViewModel>(Budgets.OrderBy(b => b.Name));
            }
        }

        private void EditExecute(object obj)
        {
            Budget updatedBudget;
            if (m_viewService.OpenBudgetEditView(SelectedBudget.BudgetId, out updatedBudget))
            {
                Currency primaryCurrency = m_currencyService.GetPrimary();

                Budgets.Remove(SelectedBudget);
                SelectedBudget = m_viewModelFactory.CreateBudgetItemViewModel(updatedBudget, primaryCurrency);
                Budgets.Add(SelectedBudget);
                
                // TODO: Is there a better way to maintain ObservableCollection<T> sorting?
                // https://github.com/JonnyRivers/Financier/issues/29
                Budgets = new ObservableCollection<IBudgetItemViewModel>(Budgets.OrderBy(b => b.Name));
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedBudget != null);
        }

        private void DeleteExecute(object obj)
        {
            if (m_viewService.OpenBudgetDeleteConfirmationView())
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
            if (m_viewService.OpenPaydayEventStartView(SelectedBudget.BudgetId, out paydayStart))
            {
                IEnumerable<Transaction> prospectiveTransasctions =
                    m_budgetService.MakePaydayTransactions(SelectedBudget.BudgetId, paydayStart);

                if (m_viewService.OpenTransactionBatchCreateConfirmView(prospectiveTransasctions))
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
