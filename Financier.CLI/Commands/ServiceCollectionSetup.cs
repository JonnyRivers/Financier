using Financier.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Financier.CLI.Commands
{
    public class ServiceCollectionSetup
    {
        public static ServiceCollection SetupCoreServices(string databaseName)
        {
            var serviceCollection = new ServiceCollection();

            // .NET Services
            ILoggerFactory loggerFactory = new LoggerFactory().AddConsole().AddDebug();
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();
            
            string connectionString =
                @"Server=(localdb)\mssqllocaldb;" +
                $"Database=Financier_{databaseName};" +
                "Trusted_Connection=True;";
            serviceCollection.AddDbContext<FinancierDbContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            return serviceCollection;
        }
    }
}
