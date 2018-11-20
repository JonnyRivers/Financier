using Financier.Desktop.Commands;
using Financier.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
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
}
