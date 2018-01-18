using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.Services
{
    public interface IViewService
    {
        void OpenMainView();

        bool OpenAccountCreateView();
        bool OpenAccountEditView(int accountId);

        bool OpenTransactionCreateView();
        bool OpenTransactionDeleteConfirmationView();
        bool OpenTransactionEditView(int transactionId);
    }
}
