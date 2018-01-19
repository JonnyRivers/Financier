using System.Collections.Generic;

namespace Financier.Services
{
    public interface ICurrencyService
    {
        IEnumerable<Currency> GetAll();
        Currency GetPrimary();
    }
}
