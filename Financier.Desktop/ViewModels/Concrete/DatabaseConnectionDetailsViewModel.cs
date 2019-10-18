using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class DatabaseConnectionDetailsViewModelFactory : IDatabaseConnectionDetailsViewModelFactory
    {
        private readonly ILogger<DatabaseConnectionDetailsViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public DatabaseConnectionDetailsViewModelFactory(ILogger<DatabaseConnectionDetailsViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IDatabaseConnectionDetailsViewModel Create()
        {
            return m_serviceProvider.CreateInstance<DatabaseConnectionCreateViewModel>();
        }

        public IDatabaseConnectionDetailsViewModel Create(int databaseConnectionId)
        {
            return m_serviceProvider.CreateInstance<DatabaseConnectionCreateViewModel>(databaseConnectionId);
        }
    }

    public abstract class DatabaseConnectionDetailsBaseViewModel : IDatabaseConnectionDetailsViewModel
    {
        protected int m_databaseConnectionId;

        protected readonly IDatabaseConnectionService m_databaseConnectionService;

        public DatabaseConnectionDetailsBaseViewModel(
            IDatabaseConnectionService databaseConnectionService,
            int databaseConnectionId)
        {
            m_databaseConnectionService = databaseConnectionService;

            m_databaseConnectionId = databaseConnectionId;

            DatabaseConnectionTypes = Enum.GetValues(typeof(DatabaseConnectionType)).Cast<DatabaseConnectionType>();
        }

        public IEnumerable<DatabaseConnectionType> DatabaseConnectionTypes { get; }

        public string Name { get; set; }
        public DatabaseConnectionType SelectedType { get; set; }

        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }

        // TODO - this is duplicated with DatabaseConnectionItemViewModel
        public DatabaseConnection ToDatabaseConnection()
        {
            return new DatabaseConnection
            {
                DatabaseConnectionId = m_databaseConnectionId,
                Name = Name,
                Type = SelectedType,
                Server = Server,
                Database = Database,
                UserId = UserId
            };
        }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        protected abstract void OKExecute(object obj);

        private void CancelExecute(object obj)
        {

        }
    }

    public class DatabaseConnectionCreateViewModel : DatabaseConnectionDetailsBaseViewModel
    {
        private readonly ILogger<DatabaseConnectionCreateViewModel> m_logger;

        public DatabaseConnectionCreateViewModel(
            ILogger<DatabaseConnectionCreateViewModel> logger,
            IDatabaseConnectionService databaseConnectionService) : base(databaseConnectionService, 0)
        {
            m_logger = logger;

            Name = "New Connection";
            SelectedType = DatabaseConnectionType.SqlServerLocalDB;
            Server = String.Empty;
            Database = String.Empty;
            UserId = String.Empty;
        }

        protected override void OKExecute(object obj)
        {
            DatabaseConnection databaseConnection = ToDatabaseConnection();

            m_databaseConnectionService.Create(databaseConnection);
            m_databaseConnectionId = databaseConnection.DatabaseConnectionId;
        }
    }

    public class DatabaseConnectionEditViewModel : DatabaseConnectionDetailsBaseViewModel
    {
        private readonly ILogger<DatabaseConnectionEditViewModel> m_logger;

        public DatabaseConnectionEditViewModel(
            ILogger<DatabaseConnectionEditViewModel> logger,
            IDatabaseConnectionService databaseConnectionService,
            int databaseConnectionId) : base(databaseConnectionService, databaseConnectionId)
        {
            m_logger = logger;

            DatabaseConnection databaseConnection = m_databaseConnectionService.Get(m_databaseConnectionId);

            Name = databaseConnection.Name;
            SelectedType = databaseConnection.Type;
            Server = databaseConnection.Server;
            Database = databaseConnection.Database;
            UserId = databaseConnection.UserId;
        }

        protected override void OKExecute(object obj)
        {
            DatabaseConnection databaseConnection = ToDatabaseConnection();

            m_databaseConnectionService.Update(databaseConnection);
        }
    }
}
