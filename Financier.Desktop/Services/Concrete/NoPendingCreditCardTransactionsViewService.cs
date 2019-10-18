using Microsoft.Extensions.Logging;
using System.Windows;

namespace Financier.Desktop.Services
{
    public class NoPendingCreditCardTransactionsViewService : INoPendingCreditCardTransactionsViewService
    {
        private readonly ILogger<NoPendingCreditCardTransactionsViewService> m_logger;

        public NoPendingCreditCardTransactionsViewService(ILogger<NoPendingCreditCardTransactionsViewService> logger)
        {
            m_logger = logger;
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
