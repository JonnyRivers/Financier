using Financier.Services;
using System.Collections.Generic;

namespace Financier.Desktop.ViewModels
{
    public interface IBudgetTransactionItemViewModel
    {
        void Setup(BudgetTransaction budgetTransaction, BudgetTransactionType type);
        BudgetTransaction ToBudgetTransaction();

        int BudgetTransactionId { get; set; }
        BudgetTransactionType Type { get; set; }
        IAccountLinkViewModel SelectedCreditAccount { get; set; }
        IAccountLinkViewModel SelectedDebitAccount { get; set; }
        decimal Amount { get; set; }

        IEnumerable<IAccountLinkViewModel> AccountLinks { get; }
    }
}
