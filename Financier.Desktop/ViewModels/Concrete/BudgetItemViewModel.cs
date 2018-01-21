using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class BudgetItemViewModel : IBudgetItemViewModel
    {
        public int BudgetId { get; set; }
        public string Name { get; set; }
        public BudgetPeriod Period { get; set; }
        public string InitialTransactionHint { get; set; }
        public int Transactions { get; set; }
    }
}
