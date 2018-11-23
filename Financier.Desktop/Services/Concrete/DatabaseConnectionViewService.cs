using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace Financier.Desktop.Services
{
    public class DatabaseConnectionViewService : IDatabaseConnectionViewService
    {
        private readonly ILogger<DatabaseConnectionViewService> m_logger;
        private readonly IDatabaseConnectionViewModelFactory m_viewModelFactory;

        public DatabaseConnectionViewService(ILogger<DatabaseConnectionViewService> logger, IDatabaseConnectionViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public bool OpenDatabaseConnectionCreateView(out DatabaseConnection newDatabaseConnection)
        {
            newDatabaseConnection = null;

            IDatabaseConnectionDetailsViewModel viewModel = m_viewModelFactory.CreateDatabaseConnectionCreateViewModel();
            var window = new DatabaseConnectionDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                newDatabaseConnection = viewModel.ToDatabaseConnection();
                return true;
            }

            return false;
        }

        public bool OpenDatabaseConnectionDeleteConfirmationView()
        {
            return OpenDeleteConfirmationView("database connection");
        }

        public bool OpenDatabaseConnectionEditView(int databaseConnectionId, out DatabaseConnection updatedDatabaseConnection)
        {
            updatedDatabaseConnection = null;

            IDatabaseConnectionDetailsViewModel viewModel = m_viewModelFactory.CreateDatabaseConnectionEditViewModel(databaseConnectionId);
            var window = new DatabaseConnectionDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                updatedDatabaseConnection = viewModel.ToDatabaseConnection();
                return true;
            }

            return false;
        }

        public bool OpenDatabaseConnectionListView(out DatabaseConnection databaseConnection, out string password)
        {
            databaseConnection = null;
            password = string.Empty;

            var viewModel = m_viewModelFactory.CreateDatabaseConnectionListViewModel();
            var connectionWindow = new DatabaseConnectionListWindow(viewModel);
            bool? result = connectionWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                databaseConnection = viewModel.SelectedDatabaseConnection.ToDatabaseConnection();
                password = viewModel.Password;
                return true;
            }

            return false;
        }

        public bool OpenDatabaseConnectionPasswordView(string userId, out string password)
        {
            password = null;

            var viewModel = m_viewModelFactory.CreateDatabaseConnectionPasswordViewModel(userId);
            var passwordWindow = new DatabaseConnectionPasswordWindow(viewModel);
            bool? result = passwordWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                password = viewModel.Password;
                return true;
            }

            return false;
        }

        // TODO - this is duplicated with ViewService
        private bool OpenDeleteConfirmationView(string context)
        {
            MessageBoxResult confirmResult = MessageBox.Show(
               $"Are you sure you want to delete this {context}?  This cannot be undone.",
               $"Really delete {context}?",
               MessageBoxButton.YesNo
            );

            return (confirmResult == MessageBoxResult.Yes);
        }
    }
}
