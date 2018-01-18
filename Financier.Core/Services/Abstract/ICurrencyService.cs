using Financier.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Financier.Services
{
    public interface ICurrencyService
    {
        IEnumerable<Currency> GetAll();
        Currency GetPrimary();
    }
}
