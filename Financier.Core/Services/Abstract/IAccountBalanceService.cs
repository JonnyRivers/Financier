using System;
using System.Collections.Generic;
using System.Text;

namespace Financier.Services
{
    public interface IAccountBalanceService
    {
        decimal GetBalance(int accountId);
    }
}
