using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;

namespace Hera.Core.Cache
{
    public class InMemoryCache : ICacheService
    {
        public T GetOrSet<T>(string key, int cacheHours, Func<T> getItemCallBack) where T : class
        {
            T item = MemoryCache.Default.Get(key) as T;
            if (item == null)
            {
                item = getItemCallBack();
                MemoryCache.Default.Add(key, item, DateTime.Now.AddHours(1));
            }
            return item;
        }

        public T SetAndGet<T>(string key, int cacheHours, Func<T> getItemCallBack) where T : class
        {
            MemoryCache.Default.Remove(key);
            var item = getItemCallBack();
            MemoryCache.Default.Add(key, item, DateTime.Now.AddHours(cacheHours));
            return item;
        }

        public void Clear(string key)
        {
            MemoryCache.Default.Remove(key);
            var keyList = MemoryCache.Default.Select(x => x.Key).ToList();
            foreach (var item in keyList)
            {
                if (item.StartsWith(key))
                    MemoryCache.Default.Remove(item);
            }
        }

        public void MobileClear()
        {
            var keyType = typeof(Keys);
            var filesList = keyType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).ToList().Where(x => x.IsLiteral && !x.IsInitOnly).ToList();
            filesList.ForEach(x => MemoryCache.Default.Remove(x.Name));
        }
    }
}
