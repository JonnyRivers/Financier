﻿using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IDatabaseConnectionListViewModel
    {
        IDatabaseConnectionItemViewModel SelectedDatabaseConnection { get; }
    }
}
