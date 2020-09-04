namespace Atom.Api.Infrastructure.Persistence.Configurations
{
    public class CacheOptions
    {
        public int CacheTimeSpanInMinutes { get; set; }
        public string LocalCacheName { get; set; }
        public string CacheAddress { get; set; }
    }

}
