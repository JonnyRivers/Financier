using Financier.Services;

namespace Financier.CLI.Services
{
    public interface ICashflowStatementWriterService
    {
        void Write(CashflowStatement cashflowStatement);
    }
}
