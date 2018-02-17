using System;
using System.Collections.Generic;
using System.Text;

namespace Financier.Services
{
    public interface ICashflowService
    {
        CashflowStatement Generate(DateTime startAt, DateTime endAt);
    }
}
