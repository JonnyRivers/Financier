using Financier.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountItemViewModel
    {
        int AccountId { get; }
        string Name { get; }
        AccountType Type { get; }
        string CurrencyName { get; }
        decimal Balance { get; }
    }
}
