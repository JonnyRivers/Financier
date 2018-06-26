using System;

namespace Financier.Services
{
    public interface ICashflowService
    {
        CashflowStatement Generate(CashflowPeriod period, DateTime startAt, DateTime endAt);
    }
}
