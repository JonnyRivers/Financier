using Financier.Data;
using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.IoC
{
    public static class ServiceProvider
    {
        private static IServiceProvider m_instance;

        public static IServiceProvider Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = Build();

                return m_instance;
            }
        }

        private static IServiceProvider Build()
        {
            var serviceCollection = new ServiceCollection();

            // Framework services
            ILoggerFactory loggerFactory = new LoggerFactory().AddDebug();
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            string connectionString =
                @"Server=(localdb)\mssqllocaldb;" +
                $"Database=Financier_FamilyFortunesTest;" +
                "Trusted_Connection=True;";
            serviceCollection.AddDbContext<FinancierDbContext>(
                options => options.UseSqlServer(connectionString),
                ServiceLifetime.Transient);

            serviceCollection.AddTransient<IAccountBalanceService, AccountBalanceService>();

            serviceCollection.AddTransient<IWindowFactory, WindowFactory>();

            // Constructing view models and views via the service provider seems like a step too far

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
