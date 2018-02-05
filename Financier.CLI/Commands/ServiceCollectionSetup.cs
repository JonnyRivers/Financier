using Financier.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

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

            RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            RegistryKey databaseKey = baseKey.OpenSubKey($"Software\\Financier\\Databases\\{databaseName}");
            string connectionString = (string)databaseKey.GetValue("ConnectionString");

            serviceCollection.AddDbContext<FinancierDbContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            return serviceCollection;
        }
    }
}
