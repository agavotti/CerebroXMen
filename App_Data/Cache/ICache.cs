using System;
using System.Runtime.Caching;

namespace CerebroXMenAPI.app_data
{
    public interface ICache
    {
        /// <summary>
        /// Agrega un elemento a cache con Clave = key y valor = object con una expiracion absoluta.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeout">Tiempo para expiracion del item en cache</param>
        /// <returns></returns>
        void Agregar(string key, object value, TimeSpan pTimeout);
        void Agregar(string key, object value, TimeSpan pTimeout, Action<CacheEntryUpdateArguments> pUpdateMethod);
        
        void Borrar(string key);
        T ObtenerValor<T>(string key) where T : class;
        T? ObtenerValorPrimitivo<T>(string key) where T : struct;
    }
}