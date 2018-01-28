using Financier.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IPaydayEventStartViewModel
    {
        void Setup(int budgetId);

        PaydayStart ToPaydayStart();

        decimal AmountPaid { get; set; }
        DateTime At { get; set; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
