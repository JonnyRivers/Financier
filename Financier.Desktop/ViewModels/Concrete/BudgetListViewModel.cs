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
        private IMessageService m_messageService;
        private ITransactionService m_transactionService;
        private IViewService m_viewService;

        public BudgetListViewModel(
            ILogger<BudgetListViewModel> logger,
            IBudgetService budgetService,
            IConversionService conversionService,
            ICurrencyService currencyService,
            IMessageService messageService,
            ITransactionService transactionService,
            IViewService viewService)
        {
            m_logger = logger;
            m_budgetService = budgetService;
            m_conversionService = conversionService;
            m_currencyService = currencyService;
            m_messageService = messageService;
            m_transactionService = transactionService;
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
        public ICommand PaydayCommand => new RelayCommand(PaydayExecute, PaydayCanExecute);

        private void CreateExecute(object obj)
        {
            int newBudgetId = m_viewService.OpenBudgetCreateView();
            if (newBudgetId > 0)
            {
                Currency primaryCurrency = m_currencyService.GetPrimary();
                Budget newBudget = m_budgetService.Get(newBudgetId);
                IBudgetItemViewModel newBudgetViewModel = m_conversionService.BudgetToItemViewModel(newBudget, primaryCurrency);
                Budgets.Add(newBudgetViewModel);
                // TODO: Is there a better way to maintain ObservableCollection<T> sorting?
                // https://github.com/JonnyRivers/Financier/issues/29
                Budgets = new ObservableCollection<IBudgetItemViewModel>(Budgets.OrderBy(b => b.Name));
            }
        }

        private void EditExecute(object obj)
        {
            if (m_viewService.OpenBudgetEditView(SelectedBudget.BudgetId))
            {
                Currency primaryCurrency = m_currencyService.GetPrimary();
                Budget budget = m_budgetService.Get(SelectedBudget.BudgetId);
                SelectedBudget.Setup(budget, primaryCurrency);
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
                    m_budgetService.MakePaydayTransactions(paydayStart);

                if (m_viewService.OpenTransactionBatchCreateConfirmView(prospectiveTransasctions))
                {
                    foreach (Transaction prospectiveTransasction in prospectiveTransasctions)
                    {
                        m_transactionService.Create(prospectiveTransasction);
                        m_messageService.Send(new TransactionCreateMessage(prospectiveTransasction));
                    }
                }
            }
        }

        private bool PaydayCanExecute(object obj)
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
