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
        private IViewService m_viewService;

        public BudgetListViewModel(
            ILogger<BudgetListViewModel> logger,
            IBudgetService budgetService,
            ICurrencyService currencyService,
            IViewService viewService)
        {
            m_logger = logger;
            m_budgetService = budgetService;
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
                PopulateBudgets();
            }
        }

        private void EditExecute(object obj)
        {
            //if (m_viewService.OpenBudgetEditView(SelectedBudget.BudgetId))
            //{
            //    PopulateBudgets();
            //}
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedBudget != null);
        }

        private void DeleteExecute(object obj)
        {
            //if (m_viewService.OpenBudgetDeleteConfirmationView())
            //{
            //    m_budgetService.Delete(SelectedBudget.BudgetId);

            //    PopulateBudgets();
            //}
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedBudget != null);
        }

        private void PopulateBudgets()
        {
            Currency currency = m_currencyService.GetPrimary();
            IEnumerable<Budget> budgets = m_budgetService.GetAll();
            IEnumerable<IBudgetItemViewModel> budgetViewModels = budgets
                .OrderBy(b => b.Name)
                .Select(b =>
                    new BudgetItemViewModel
                    {
                        BudgetId = b.BudgetId,
                        Name = b.Name,
                        Period = b.Period,
                        InitialTransactionHint = 
                            $"{currency.Symbol}{b.InitialTransaction.Amount} " + 
                            $"from {b.InitialTransaction.CreditAccount.Name} " +
                            $"to {b.InitialTransaction.DebitAccount.Name}",
                        Transactions = b.Transactions.Count()
                    });

            Budgets = new ObservableCollection<IBudgetItemViewModel>(budgetViewModels);
        }
    }
}
