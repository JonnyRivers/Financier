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
    public class DatabaseConnectionListViewModel : BaseViewModel, IDatabaseConnectionListViewModel
    {
        private ILogger<DatabaseConnectionListViewModel> m_logger;
        private IDatabaseConnectionService m_databaseConnectionService;
        private IDatabaseConnectionViewModelFactory m_viewModelFactory;
        private IDatabaseConnectionViewService m_viewService;

        private ObservableCollection<IDatabaseConnectionItemViewModel> m_databaseConnections;
        private IDatabaseConnectionItemViewModel m_selectedDatabaseConnection;

        public DatabaseConnectionListViewModel(
            ILogger<DatabaseConnectionListViewModel> logger,
            IDatabaseConnectionService databaseConnectionService,
            IDatabaseConnectionViewModelFactory viewModelFactory,
            IDatabaseConnectionViewService viewService)
        {
            m_logger = logger;
            m_databaseConnectionService = databaseConnectionService;
            m_viewModelFactory = viewModelFactory;
            m_viewService = viewService;

            PopulateDatabaseConnections();
        }

        private void PopulateDatabaseConnections()
        {
            IEnumerable<DatabaseConnection> databaseConnections = m_databaseConnectionService.GetAll();
            IEnumerable<IDatabaseConnectionItemViewModel> databaseConnectionItemViewModels =
                databaseConnections.Select(dbc => m_viewModelFactory.CreateDatabaseConnectionItemViewModel(dbc));

            DatabaseConnections = new ObservableCollection<IDatabaseConnectionItemViewModel>(databaseConnectionItemViewModels);
        }

        public ObservableCollection<IDatabaseConnectionItemViewModel> DatabaseConnections
        {
            get { return m_databaseConnections; }
            set
            {
                if (m_databaseConnections != value)
                {
                    m_databaseConnections = value;

                    OnPropertyChanged();
                }
            }
        }

        public IDatabaseConnectionItemViewModel SelectedDatabaseConnection
        {
            get { return m_selectedDatabaseConnection; }
            set
            {
                if (m_selectedDatabaseConnection != value)
                {
                    m_selectedDatabaseConnection = value;

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
            //Budget newBudget;
            //if (m_viewService.OpenBudgetCreateView(out newBudget))
            //{
            //    Currency primaryCurrency = m_currencyService.GetPrimary();
            //    IBudgetItemViewModel newBudgetViewModel =
            //        m_viewModelFactory.CreateBudgetItemViewModel(newBudget, primaryCurrency);
            //    Budgets.Add(newBudgetViewModel);
            //}
        }

        private void EditExecute(object obj)
        {
            //Budget updatedBudget;
            //if (m_viewService.OpenBudgetEditView(SelectedBudget.BudgetId, out updatedBudget))
            //{
            //    Currency primaryCurrency = m_currencyService.GetPrimary();

            //    Budgets.Remove(SelectedBudget);
            //    SelectedBudget = m_viewModelFactory.CreateBudgetItemViewModel(updatedBudget, primaryCurrency);
            //    Budgets.Add(SelectedBudget);
            //}
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedDatabaseConnection != null);
        }

        private void DeleteExecute(object obj)
        {
            //if (m_viewService.OpenBudgetDeleteConfirmationView())
            //{
            //    // Update model
            //    m_budgetService.Delete(SelectedBudget.BudgetId);

            //    // Update view model
            //    Budgets.Remove(SelectedBudget);
            //}
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedDatabaseConnection != null);
        }
    }
}
