using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Swiftly.DatabaseContext
{
    /// <summary>
    /// The class that registers project-wide context classes 
    /// </summary>
    public static class SwiftlyDatabaseContextRegistar
    {
        /// <summary>
        /// Adds an action-based configuration for all db context to the service collection
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddDbContextConfigurer(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> action)
        {
            serviceCollection.AddSingleton(typeof(ISwiftlyDbConfigurer), new SwiftlyDbActionConfigurer(action));
            return serviceCollection;
        }
        /// <summary>
        /// Adds a db context registry to the collection
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddDbContextRegistry(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(SwitlyDbContextRegisty), new SwitlyDbContextRegisty());
            return serviceCollection;
        }

        /// <summary>
        /// Finds the context registry in the service collection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static SwitlyDbContextRegisty FindDbContextRegistry(this IServiceCollection collection)
        {
            foreach (var serviceDescriptor in collection)
            {
                if (serviceDescriptor.ServiceType == typeof(SwitlyDbContextRegisty) &&
                    serviceDescriptor.ImplementationInstance is SwitlyDbContextRegisty registryInstance)
                    return registryInstance;
            }
            return null;
        }
    }
}

