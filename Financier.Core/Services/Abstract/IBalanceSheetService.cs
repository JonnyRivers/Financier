using Financier.Entities;
using System;

namespace Financier.Services
{
    public interface IBalanceSheetService
    {
        BalanceSheet Generate(DateTime at);
    }
}
