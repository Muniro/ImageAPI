using Atom.Api.Infrastructure.Shared.Common_Interfaces;
using Atom.Api.WebApi.Configurations;
using EasyCaching.Core;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Atom.Api.WebApi.Services
{
    public class RedisCacheService : IRedisCacheService
    {

        private IEasyCachingProvider cachingProvider;
        private readonly ImageOptions options;
        private IEasyCachingProviderFactory cachingProviderFactory;

        public RedisCacheService(IOptions<ImageOptions> options,IEasyCachingProviderFactory cachingProviderFactory)
        {
            this.options = options.Value;
            this.cachingProviderFactory = cachingProviderFactory;
            this.cachingProvider = this.cachingProviderFactory.GetCachingProvider(options.Value.LocalCacheName);
            
        }
        public async Task<Byte[]> GetCachedValueAsync(string key)
        {
            Byte[] cachedValue = null;

            using (var multiplexer = ConnectionMultiplexer.Connect("localhost:6379"))
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
           

            using(var multiplexer= ConnectionMultiplexer.Connect("localhost:6379"))
            {
                var db = multiplexer.GetDatabase();
                db.StringSet(key, value, TimeSpan.FromMinutes(60));
            }

        }
    }
}
