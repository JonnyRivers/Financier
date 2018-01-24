using System;
using Financier.Services;

namespace Financier.CLI.Services
{
    public interface IBalanceSheetWriterService
    {
        void Write(BalanceSheet balanceSheet, DateTime at);
    }
}
