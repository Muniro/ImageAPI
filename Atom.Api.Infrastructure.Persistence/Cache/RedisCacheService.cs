using Atom.Api.Infrastructure.Persistence.Configurations;
using Atom.Api.Infrastructure.Shared.Common_Interfaces;
using EasyCaching.Core;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Atom.Api.Infrastructure.Persistence.Cache
{
    public class RedisCacheService : IRedisCacheService
    {

        private IEasyCachingProvider cachingProvider;
        private readonly CacheOptions options;
        private IEasyCachingProviderFactory cachingProviderFactory;

        public RedisCacheService(IOptions<CacheOptions> cacheOptions,IEasyCachingProviderFactory cachingProviderFactory)
        {
            this.options = cacheOptions.Value;
            this.cachingProviderFactory = cachingProviderFactory;
            this.cachingProvider = this.cachingProviderFactory.GetCachingProvider(options.LocalCacheName);
            
        }
        public async Task<Byte[]> GetCachedValueAsync(string key)
        {
            Byte[] cachedValue = null;

            using (var multiplexer = ConnectionMultiplexer.Connect(options.CacheAddress))
            {
                var db = multiplexer.GetDatabase();
                cachedValue= await db.StringGetAsync(key);
            }
            return cachedValue;
        }

        public async Task<bool> HasValudBeenCached(string key)
        {
            return await this.cachingProvider.ExistsAsync(key);
        }

        public void SetCacheValueAsync(string key, Byte[] value)
        {
           

            using(var multiplexer= ConnectionMultiplexer.Connect(options.CacheAddress))
            {
                var db = multiplexer.GetDatabase();
                db.StringSet(key, value, TimeSpan.FromMinutes((int)options.CacheTimeSpanInMinutes));
            }

        }
    }
}
