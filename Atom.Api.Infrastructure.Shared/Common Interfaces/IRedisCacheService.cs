using System;
using System.Threading.Tasks;

namespace Atom.Api.Infrastructure.Shared.Common_Interfaces
{
    public interface IRedisCacheService
    {
        void SetCacheValueAsync(string key, Byte[] value);
        Task<Byte[]> GetCachedValueAsync(string key);
        Task<bool> HasValudBeenCached(string key);
    }
}
