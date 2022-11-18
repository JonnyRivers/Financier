using System;
using System.Collections.Generic;
using Financier.Services;

namespace Financier.CLI.Services
{
    public interface IBalanceSheetHistoryWriterService
    {
        void Write(IDictionary<DateTime, BalanceSheet> balanceSheetsByDate);
    }
}
