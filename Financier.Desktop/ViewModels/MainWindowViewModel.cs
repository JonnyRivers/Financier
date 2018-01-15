using Financier.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IMainWindowViewModel
    {
        public MainWindowViewModel(FinancierDbContext dbContext)
        {
            IEnumerable<ITransactionViewModel> transactionVMs = dbContext.Transactions
                .Select(t =>
                    new TransactionViewModel(
                        t.CreditAccount.Name,
                        t.DebitAccount.Name,
                        t.Amount,
                        t.At));
            Transactions = new ObservableCollection<ITransactionViewModel>(transactionVMs);
        }

        public ObservableCollection<ITransactionViewModel> Transactions { get; }
    }
}
