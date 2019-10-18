using Microsoft.Extensions.Logging;
using System.Windows;

namespace Financier.Desktop.Services
{
    public class DeleteConfirmationViewService : IDeleteConfirmationViewService
    {
        private readonly ILogger<DeleteConfirmationViewService> m_logger;

        public DeleteConfirmationViewService(ILogger<DeleteConfirmationViewService> logger)
        {
            m_logger = logger;
        }

        // TODO - this is duplicated with IDatabaseConnectionViewService
        public bool Show(string context)
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
