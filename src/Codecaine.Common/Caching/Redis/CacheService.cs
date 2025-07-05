using Codecaine.Common.Caching.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecaine.Common.Caching.Redis
{

    /// <summary>
    /// CacheService provides methods to interact with Redis cache.
    /// </summary>
    public class CacheService : ICacheService
    {
        
        private readonly IDatabase _db;

        public CacheService(IOptions<CachingSetting> cacheSetting)
        {
           var redis = ConnectionMultiplexer.Connect(cacheSetting.Value.ConnectionString);
            _db = redis.GetDatabase();
        }

        public void SetCacheItem(string key, string value, TimeSpan? expiry = null)
        {
            _db.StringSet(key, value, expiry);
        }

        public string GetCacheItem(string key)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return _db.StringGet(key);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public bool RemoveCacheItem(string key)
        {
            return _db.KeyDelete(key);
        }

        public bool CacheItemExists(string key)
        {
            return _db.KeyExists(key);
        }
    }
}
