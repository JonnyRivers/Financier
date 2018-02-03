using Microsoft.Extensions.DependencyInjection;
using System;

namespace Financier
{
    public static class ServiceProviderExtensions
    {
        public static T CreateInstance<T>(this IServiceProvider serviceProvider, params object[] parameters)
        {
            return ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
        }
    }
}
