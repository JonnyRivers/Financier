using Financier.Entities;
using Financier.Services;
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
            ILoggerFactory loggerFactory = new LoggerFactory().AddDebug();
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            IDataConfigService dataConfigService = new DataConfigRegistryService();
            string connectionString = dataConfigService.GetConnectionString(databaseName);

            serviceCollection.AddDbContext<FinancierDbContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            return serviceCollection;
        }
    }
}
