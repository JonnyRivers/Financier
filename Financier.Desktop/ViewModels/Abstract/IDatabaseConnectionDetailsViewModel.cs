﻿using Financier.Services;
using System.Collections.Generic;

namespace Financier.Desktop.ViewModels
{
    public interface IDatabaseConnectionDetailsViewModelFactory
    {
        IDatabaseConnectionDetailsViewModel Create();
        IDatabaseConnectionDetailsViewModel Create(int databaseConnectionId);
    }

    public interface IDatabaseConnectionDetailsViewModel
    {
        IEnumerable<DatabaseConnectionType> DatabaseConnectionTypes { get; }

        string Name { get; }
        DatabaseConnectionType SelectedType { get; }

        string Server { get; }
        string Database { get; }
        string UserId { get; }

        DatabaseConnection ToDatabaseConnection();
    }
}
