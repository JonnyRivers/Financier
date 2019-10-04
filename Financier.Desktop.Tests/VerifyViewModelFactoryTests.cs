using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Financier.UnitTesting;
using Financier.UnitTesting.DbSetup;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Tests
{
    /// <summary>
    /// These tests mitigate the problem of ViewModel constructor changes
    /// </summary>
    [TestClass]
    public class VerifyViewModelFactoryTests
    {
        [TestMethod]
        public void TestExceptionViewModelCreation()
        {
            ServiceCollection serviceCollection = CreateServiceCollection();
            serviceCollection.AddTransient<IExceptionViewModelFactory, ExceptionViewModelFactory>();

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            IExceptionViewModelFactory factory = serviceProvider.GetRequiredService<IExceptionViewModelFactory>();
            factory.Create(new Exception());
        }

        private ServiceCollection CreateServiceCollection()
        {
            // Framework services
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();

            return serviceCollection;
        }
    }
}
