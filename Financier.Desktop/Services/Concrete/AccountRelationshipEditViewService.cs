using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Windows;

namespace Financier.Desktop.Services
{
    public class AccountRelationshipEditViewService : IAccountRelationshipEditViewService
    {
        private readonly ILogger<AccountRelationshipEditViewService> m_logger;
        private readonly IAccountRelationshipDetailsViewModelFactory m_accountRelationshipDetailsViewModelFactory;

        public AccountRelationshipEditViewService(
            ILoggerFactory loggerFactory,
            IAccountRelationshipDetailsViewModelFactory accountRelationshipDetailsViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<AccountRelationshipEditViewService>();
            m_accountRelationshipDetailsViewModelFactory = accountRelationshipDetailsViewModelFactory;
        }

        public bool Show(int accountRelationshipId, out AccountRelationship updatedAccountRelationship)
        {
            updatedAccountRelationship = null;

            IAccountRelationshipDetailsViewModel viewModel =
                m_accountRelationshipDetailsViewModelFactory.Create(accountRelationshipId);
            var window = new AccountRelationshipDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue)
            {
                updatedAccountRelationship = viewModel.ToAccountRelationship();
                return result.Value;
            }

            return false;
        }
    }
}
