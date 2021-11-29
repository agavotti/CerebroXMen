using System;
using System.Runtime.Caching;

namespace CerebroXMenAPI.app_data
{
    public class NoneCacher : ICache
    {
        public NoneCacher()
        {
        }

        public void Agregar(string key, object value, TimeSpan pTimeSpan)
        {
        }

        public void Agregar(string key, object value, TimeSpan pTimeSpan, Action<CacheEntryUpdateArguments> pUpdateMethod)
        {
        }

        public void Borrar(string key)
        {
        }

        public T ObtenerValor<T>(string key) where T : class
        {
            return null;
        }

        public T? ObtenerValorPrimitivo<T>(string key) where T : struct
        {
            return null;
        }
    }
}
