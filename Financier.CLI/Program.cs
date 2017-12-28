using Financier.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.CommandLineUtils;
using System.Linq;

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

            app.Command("create", ConfigureCreateCommand);

            return app.Execute(args);
        }

        static void ConfigureCreateCommand(CommandLineApplication command)
        {
            command.Description = "Ensure the database is created";

            command.OnExecute(() =>
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<FinancierDbContext>();
                    dbContextOptionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Financier_Test;Trusted_Connection=True;");
                    using (var dbContext = new FinancierDbContext(dbContextOptionsBuilder.Options))
                    {
                        dbContext.Database.EnsureCreated();

                        if (dbContext.Currencies.SingleOrDefault(c => c.Symbol == "$") == null)
                        {
                            var dollar = new Currency
                            {
                                Name = "US Dollar",
                                ShortName = "USD",
                                Symbol = "$"
                            };
                            dbContext.Currencies.Add(dollar);
                            dbContext.SaveChanges();
                        }

                        return 0;
                    }

                        
                });
        }
    }
}
