using Microsoft.Extensions.CommandLineUtils;

namespace Financier.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "fin",
            };
            app.HelpOption("-h|--help");

            app.Command("create", Commands.CreateCommand.Configure);
            app.Command("obliterate", Commands.ObliterateCommand.Configure);
            app.Command("load", Commands.LoadCommand.Configure);
            app.Command("save", Commands.SaveCommand.Configure);
            app.Command("balance-sheet", Commands.BalanceSheetCommand.Configure);
            app.Command("cashflow", Commands.CashflowCommand.Configure);

            return app.Execute(args);
        }
    }
}
