using Financier.Entities;
using Financier.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Financier.CLI.Commands
{
    public class ServiceCollectionSetup
    {
        public static ServiceCollection SetupCoreServices(string databaseConnectionName, string password)
        {
            var serviceCollection = new ServiceCollection();

            // .NET Services
            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddDebug();
            });

            serviceCollection.AddSingleton<IDatabaseConnectionService, LocalDatabaseConnectionService>();
            IDatabaseConnectionService databaseConnectionService =
                serviceCollection.BuildServiceProvider().GetRequiredService<IDatabaseConnectionService>();
            DatabaseConnection databaseConnection = databaseConnectionService.Get(databaseConnectionName);
            string connectionString = databaseConnection.BuildConnectionString(password);

            serviceCollection.AddDbContext<FinancierDbContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            return serviceCollection;
        }
    }
}
