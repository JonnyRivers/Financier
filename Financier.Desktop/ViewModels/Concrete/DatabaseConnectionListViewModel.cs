using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
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
        private string m_password;

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

            m_password = String.Empty;

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

        public string Password
        {
            get { return m_password; }
        }

        public ICommand ConnectCommand => new RelayCommand(ConnectExecute, ConnectCanExecute);
        public ICommand CreateCommand => new RelayCommand(CreateExecute);
        public ICommand EditCommand => new RelayCommand(EditExecute, EditCanExecute);
        public ICommand DeleteCommand => new RelayCommand(DeleteExecute, DeleteCanExecute);

        private void ConnectExecute(object obj)
        {
            if (!String.IsNullOrWhiteSpace(SelectedDatabaseConnection.UserId))
            {
                m_viewService.OpenDatabaseConnectionPasswordView(SelectedDatabaseConnection.UserId, out m_password);
            }
        }

        private bool ConnectCanExecute(object obj)
        {
            return (SelectedDatabaseConnection != null);
        }

        private void CreateExecute(object obj)
        {
            DatabaseConnection databaseConnection;
            if(m_viewService.OpenDatabaseConnectionCreateView(out databaseConnection))
            {
                IDatabaseConnectionItemViewModel newDatabaseConnectionViewModel =
                    m_viewModelFactory.CreateDatabaseConnectionItemViewModel(databaseConnection);
                DatabaseConnections.Add(newDatabaseConnectionViewModel);
            }
        }

        private void EditExecute(object obj)
        {
            DatabaseConnection databaseConnection;
            if (m_viewService.OpenDatabaseConnectionEditView(SelectedDatabaseConnection.DatabaseConnectionId, out databaseConnection))
            {
                DatabaseConnections.Remove(SelectedDatabaseConnection);
                SelectedDatabaseConnection = m_viewModelFactory.CreateDatabaseConnectionItemViewModel(databaseConnection);
                DatabaseConnections.Add(SelectedDatabaseConnection);
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedDatabaseConnection != null);
        }

        private void DeleteExecute(object obj)
        {
            if (m_viewService.OpenDatabaseConnectionDeleteConfirmationView())
            {
                // Update model
                m_databaseConnectionService.Delete(SelectedDatabaseConnection.DatabaseConnectionId);

                // Update view model
                DatabaseConnections.Remove(SelectedDatabaseConnection);
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedDatabaseConnection != null);
        }
    }
}
