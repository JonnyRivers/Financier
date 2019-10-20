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
    public class DatabaseConnectionListViewModelFactory : IDatabaseConnectionListViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IDatabaseConnectionService m_databaseConnectionService;
        private readonly IDatabaseConnectionItemViewModelFactory m_databaseConnectionItemViewModelFactory;
        private readonly IDatabaseConnectionCreateViewService m_databaseConnectionCreateViewService;
        private readonly IDatabaseConnectionEditViewService m_databaseConnectionEditViewService;
        private readonly IDatabaseConnectionPasswordViewService m_databaseConnectionPasswordViewService;
        private readonly IDeleteConfirmationViewService m_deleteConfirmationViewService;

        public DatabaseConnectionListViewModelFactory(
            ILoggerFactory loggerFactory,
            IDatabaseConnectionService databaseConnectionService,
            IDatabaseConnectionItemViewModelFactory databaseConnectionItemViewModelFactory,
            IDatabaseConnectionCreateViewService databaseConnectionCreateViewService,
            IDatabaseConnectionEditViewService databaseConnectionEditViewService,
            IDatabaseConnectionPasswordViewService databaseConnectionPasswordViewService,
            IDeleteConfirmationViewService deleteConfirmationViewService)
        {
            m_loggerFactory = loggerFactory;
            m_databaseConnectionService = databaseConnectionService;
            m_databaseConnectionItemViewModelFactory = databaseConnectionItemViewModelFactory;
            m_databaseConnectionCreateViewService = databaseConnectionCreateViewService;
            m_databaseConnectionEditViewService = databaseConnectionEditViewService;
            m_databaseConnectionPasswordViewService = databaseConnectionPasswordViewService;
            m_deleteConfirmationViewService = deleteConfirmationViewService;
        }

        public IDatabaseConnectionListViewModel Create()
        {
            return new DatabaseConnectionListViewModel(
                m_loggerFactory,
                m_databaseConnectionService,
                m_databaseConnectionItemViewModelFactory,
                m_databaseConnectionCreateViewService,
                m_databaseConnectionEditViewService,
                m_databaseConnectionPasswordViewService,
                m_deleteConfirmationViewService);
        }
    }

    public class DatabaseConnectionListViewModel : BaseViewModel, IDatabaseConnectionListViewModel
    {
        private readonly ILogger<DatabaseConnectionListViewModel> m_logger;
        private readonly IDatabaseConnectionService m_databaseConnectionService;
        private readonly IDatabaseConnectionItemViewModelFactory m_databaseConnectionItemViewModelFactory;
        private readonly IDatabaseConnectionCreateViewService m_databaseConnectionCreateViewService;
        private readonly IDatabaseConnectionEditViewService m_databaseConnectionEditViewService;
        private readonly IDatabaseConnectionPasswordViewService m_databaseConnectionPasswordViewService;
        private readonly IDeleteConfirmationViewService m_deleteConfirmationViewService;

        private ObservableCollection<IDatabaseConnectionItemViewModel> m_databaseConnections;
        private IDatabaseConnectionItemViewModel m_selectedDatabaseConnection;
        private string m_password;

        public DatabaseConnectionListViewModel(
            ILoggerFactory loggerFactory,
            IDatabaseConnectionService databaseConnectionService,
            IDatabaseConnectionItemViewModelFactory databaseConnectionItemViewModelFactory,
            IDatabaseConnectionCreateViewService databaseConnectionCreateViewService,
            IDatabaseConnectionEditViewService databaseConnectionEditViewService,
            IDatabaseConnectionPasswordViewService databaseConnectionPasswordViewService,
            IDeleteConfirmationViewService deleteConfirmationViewService)
        {
            m_logger = loggerFactory.CreateLogger<DatabaseConnectionListViewModel>();
            m_databaseConnectionService = databaseConnectionService;
            m_databaseConnectionItemViewModelFactory = databaseConnectionItemViewModelFactory;
            m_databaseConnectionCreateViewService = databaseConnectionCreateViewService;
            m_databaseConnectionEditViewService = databaseConnectionEditViewService;
            m_databaseConnectionPasswordViewService = databaseConnectionPasswordViewService;
            m_deleteConfirmationViewService = deleteConfirmationViewService;

            m_password = String.Empty;

            PopulateDatabaseConnections();
        }

        private void PopulateDatabaseConnections()
        {
            IEnumerable<DatabaseConnection> databaseConnections = m_databaseConnectionService.GetAll();
            IEnumerable<IDatabaseConnectionItemViewModel> databaseConnectionItemViewModels =
                databaseConnections.Select(dbc => m_databaseConnectionItemViewModelFactory.Create(dbc));

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
                m_databaseConnectionPasswordViewService.Show(SelectedDatabaseConnection.UserId, out m_password);
            }
        }

        private bool ConnectCanExecute(object obj)
        {
            return (SelectedDatabaseConnection != null);
        }

        private void CreateExecute(object obj)
        {
            DatabaseConnection databaseConnection;
            if(m_databaseConnectionCreateViewService.Show(out databaseConnection))
            {
                IDatabaseConnectionItemViewModel newDatabaseConnectionViewModel =
                    m_databaseConnectionItemViewModelFactory.Create(databaseConnection);
                DatabaseConnections.Add(newDatabaseConnectionViewModel);
            }
        }

        private void EditExecute(object obj)
        {
            DatabaseConnection databaseConnection;
            if (m_databaseConnectionEditViewService.Show(SelectedDatabaseConnection.DatabaseConnectionId, out databaseConnection))
            {
                DatabaseConnections.Remove(SelectedDatabaseConnection);
                SelectedDatabaseConnection = m_databaseConnectionItemViewModelFactory.Create(databaseConnection);
                DatabaseConnections.Add(SelectedDatabaseConnection);
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedDatabaseConnection != null);
        }

        private void DeleteExecute(object obj)
        {
            if (m_deleteConfirmationViewService.Show("database connection"))
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
