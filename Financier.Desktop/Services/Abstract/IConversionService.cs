using Financier.Desktop.ViewModels;
using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IConversionService
    {
        IAccountItemViewModel AccountToItemViewModel(Account account);
        IAccountLinkViewModel AccountLinkToViewModel(AccountLink accountLink);
        IBudgetItemViewModel BudgetToItemViewModel(Budget budget, Currency currency);
        ITransactionItemViewModel TransactionToItemViewModel(Transaction transaction, decimal balance);
    }
}
