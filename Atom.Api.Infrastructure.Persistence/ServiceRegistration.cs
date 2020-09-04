using Atom.Api.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Configuration;
using EasyCaching.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Atom.Api.Infrastructure.Shared.Common_Interfaces;
using Atom.Api.Infrastructure.Persistence.Cache;

namespace Atom.Api.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            
            var cacheOptionsSection =
                configuration.GetSection("CacheOptions");
            services.Configure<CacheOptions>(cacheOptionsSection);


            services.AddEasyCaching(options =>
            {
                //use redis cache
                options.UseRedis(redisConfig =>
                {
                    //Setup Endpoint, could be dynamically set the server and port details.
                    redisConfig.DBConfig.Endpoints.Add(new ServerEndPoint("localhost", 6379));

                    /* For production recommend setting up Password!, like this */
                    //Setup password if applicable
                    //if (!string.IsNullOrEmpty(serverPassword))
                    //{
                    //    redisConfig.DBConfig.Password = serverPassword;
                    //}

                    /* ======================= */


                    //Allow admin operations
                    redisConfig.DBConfig.AllowAdmin = true;
                },
                    "LocalRedis");
            });

            services.AddTransient<IRedisCacheService, RedisCacheService>();

        }
    }
}
