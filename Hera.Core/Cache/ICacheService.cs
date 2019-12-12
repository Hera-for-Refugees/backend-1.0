using System;

namespace Hera.Core.Cache
{
    public interface ICacheService
    {
        T GetOrSet<T>(string key, int cacheHours, Func<T> getItemCallBack) where T : class;
    }
}
