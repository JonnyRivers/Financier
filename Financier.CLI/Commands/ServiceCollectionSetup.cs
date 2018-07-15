using Financier.Entities;
using Financier.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Financier.CLI.Commands
{
    public class ServiceCollectionSetup
    {
        public static ServiceCollection SetupCoreServices()
        {
            var serviceCollection = new ServiceCollection();

            // .NET Services
            ILoggerFactory loggerFactory = new LoggerFactory().AddDebug();
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            // We have to build a temporary service provider to get the connection string for the DbContext.
            // Perhaps there is a better way.
            serviceCollection.AddSingleton<IEnvironmentService, EnvironmentService>();
            IEnvironmentService environmentService =
                serviceCollection.BuildServiceProvider().GetRequiredService<IEnvironmentService>();
            string connectionString = environmentService.GetConnectionString();

            serviceCollection.AddDbContext<FinancierDbContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            return serviceCollection;
        }
    }
}
