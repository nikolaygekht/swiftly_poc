using Microsoft.Extensions.DependencyInjection;
using Swiftly.DatabaseContext;
using Swiftly.User.Api.Data;
using Swiftly.User.Api.Data.Provider.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swiftly.User.Api
{

    /// <summary>
    /// Service class to register User API EF providers in the DI container
    /// </summary>
    public static class ServiceProviderTools
    {
        /// <summary>
        /// Add service collection
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddUserApi(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAutoMapper(typeof(MappingProfile));
            serviceCollection.AddDbContext<UserApiContext>();
            serviceCollection.FindDbContextRegistry()?.AddContext<UserApiContext>();
            serviceCollection.AddScoped<IUserDao, UserApiDao>();
            serviceCollection.AddScoped<IUserApi, UserApi>();
            return serviceCollection;
        }
    }
}
