using Financier.Services;

namespace Financier.CLI.Services
{
    public interface IIncomeExpenseStatementWriterService
    {
        void Write(IncomeExpenseStatement incomeExpenseStatement, string outputPath);
    }
}
