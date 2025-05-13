using LazyCache;
using LazyCache.Providers;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;
namespace CabUserService.Infrastructures.Helper;

public static class CacheHelper
{
    public static IEnumerable<ICacheEntry> GetAllCacheEntries(ICacheProvider cacheProvider)
    {
        var cacheEntries = new List<ICacheEntry>();
        var field = typeof(MemoryCacheProvider).GetField("cache", BindingFlags.NonPublic | BindingFlags.Instance);
        IMemoryCache cacheObj = field.GetValue(cacheProvider) as MemoryCache;

        var cacheProperty = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
        var cacheCollection = (ICollection)cacheProperty.GetValue(cacheObj, null);

        foreach (var cacheItem in cacheCollection)
        {
            var valProp = cacheItem.GetType().GetProperty("Value");
            ICacheEntry cacheValue = (ICacheEntry)valProp.GetValue(cacheItem, null);

            cacheEntries.Add(cacheValue);
        };

        return cacheEntries;

    }
}