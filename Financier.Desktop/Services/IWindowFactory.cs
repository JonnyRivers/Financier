using Financier.Desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Financier.Desktop.Services
{
    public interface IWindowFactory
    {
        Window CreateMainWindow();

        Window CreateAccountCreateWindow();

        Window CreateTransactionCreateWindow();
        Window CreateTransactionEditWindow(int transactionId);
    }
}
