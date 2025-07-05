using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Codecaine.Common.Caching.Distributed
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public bool CacheItemExists(string key)
        {
            var cacheValue = _cache.GetString(key);
            bool keyExists = cacheValue != null;
            return keyExists;
        }

        public string GetCacheItem(string key)
        {
            return _cache.GetString(key) ?? string.Empty;
        }

        public bool RemoveCacheItem(string key)
        {
            _cache.Remove(key);
            return true;
        }

        public void SetCacheItem(string key, string value, TimeSpan? expiry = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            };
            _cache.SetString(key, value, options);
        }
    }
}
