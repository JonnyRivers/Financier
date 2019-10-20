using Microsoft.Extensions.Logging;
using System.Windows;

namespace Financier.Desktop.Services
{
    public class NoPendingCreditCardTransactionsViewService : INoPendingCreditCardTransactionsViewService
    {
        private readonly ILogger<NoPendingCreditCardTransactionsViewService> m_logger;

        public NoPendingCreditCardTransactionsViewService(ILoggerFactory loggerFactory)
        {
            m_logger = loggerFactory.CreateLogger<NoPendingCreditCardTransactionsViewService>();
        }


        public void Show(string accountName)
        {
            MessageBox.Show(
               $"There are no transactions to pay off from account '{accountName}'.",
               $"Nothing to pay off",
               MessageBoxButton.OK
            );
        }
    }
}
