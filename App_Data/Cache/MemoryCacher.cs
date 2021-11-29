using System;
using System.Runtime.Caching;

namespace CerebroXMenAPI.app_data
{
    public class MemoryCacher : ICache
    {
        public MemoryCacher()
        {
        }

        public T ObtenerValor<T>(string key) where T : class
        {
            var memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key) == false)
            {
                return null;
            }

            return memoryCache.Get(key) as T;
        }

        public T? ObtenerValorPrimitivo<T>(string key) where T : struct
        {
            var memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key) == false)
            {
                return null;
            }
            return memoryCache.Get(key) as T?;
        }

        public void Agregar(string key, object value, TimeSpan pTimeSpan)
        {
            DateTimeOffset v_DTONuevoTimeout = DateTimeOffset.Now.Add(pTimeSpan);

            if (value == null) return;
            var memoryCache = MemoryCache.Default;
            memoryCache.Set(key, value, v_DTONuevoTimeout);
        }

        public void Agregar(string key, object value, TimeSpan pTimeSpan, Action<CacheEntryUpdateArguments> pUpdateMethod)
        {
            DateTimeOffset v_DTONuevoTimeout = DateTimeOffset.Now.Add(pTimeSpan);

            if (value == null) return;

            CacheItemPolicy v_clsCacheItem = new CacheItemPolicy
            {
                AbsoluteExpiration = v_DTONuevoTimeout,
                UpdateCallback = (a) => pUpdateMethod(a)
            };

            var memoryCache = MemoryCache.Default;
            
            memoryCache.Set(key, value, v_clsCacheItem);
        }

        public void Borrar(string key)
        {
            var memoryCache = MemoryCache.Default;
            if (memoryCache.Contains(key) == false) return;

            memoryCache.Remove(key);
        }
    }
}