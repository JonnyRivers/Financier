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
        private IConversionService m_conversionService;
        private ICurrencyService m_currencyService;
        private IViewService m_viewService;

        public BudgetListViewModel(
            ILogger<BudgetListViewModel> logger,
            IBudgetService budgetService,
            IConversionService conversionService,
            ICurrencyService currencyService,
            IViewService viewService)
        {
            m_logger = logger;
            m_budgetService = budgetService;
            m_conversionService = conversionService;
            m_currencyService = currencyService;
            m_viewService = viewService;

            PopulateBudgets();
        }

        private ObservableCollection<IBudgetItemViewModel> m_budgets;
        private IBudgetItemViewModel m_selectedBudget;

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

        private void CreateExecute(object obj)
        {
            if (m_viewService.OpenBudgetCreateView())
            {
                // TODO: Budget list VM should be partially repopulated after adds, deletes and edits
                // https://github.com/JonnyRivers/Financier/issues/26
                PopulateBudgets();
            }
        }

        private void EditExecute(object obj)
        {
            if (m_viewService.OpenBudgetEditView(SelectedBudget.BudgetId))
            {
                // TODO: Budget list VM should be partially repopulated after adds, deletes and edits
                // https://github.com/JonnyRivers/Financier/issues/26
                PopulateBudgets();
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
                m_budgetService.Delete(SelectedBudget.BudgetId);

                // TODO: Budget list VM should be partially repopulated after adds, deletes and edits
                // https://github.com/JonnyRivers/Financier/issues/26
                PopulateBudgets();
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedBudget != null);
        }

        private void PopulateBudgets()
        {
            Currency primaryCurrency = m_currencyService.GetPrimary();
            IEnumerable<Budget> budgets = m_budgetService.GetAll();
            IEnumerable<IBudgetItemViewModel> budgetViewModels =
                budgets.Select(b => m_conversionService.BudgetToItemViewModel(b, primaryCurrency));

            Budgets = new ObservableCollection<IBudgetItemViewModel>(budgetViewModels.OrderBy(b => b.Name));
        }
    }
}
