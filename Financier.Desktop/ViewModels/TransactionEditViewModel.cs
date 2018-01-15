using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModels
{
    public class TransactionEditViewModel : BaseViewModel, ITransactionEditViewModel
    {
        private ITransactionItemViewModel m_itemViewModel;

        public TransactionEditViewModel(ITransactionItemViewModel itemViewModel)
        {
            m_itemViewModel = itemViewModel;
        }
    }
}
