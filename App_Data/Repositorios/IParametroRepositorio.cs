using System;

namespace CerebroXMenAPI.app_data
{
    public interface IParametroRepositorio
    {
        IParametro<T> GetParametro<T>(string NombreParametro, T ValorPorDefecto);
        /// <summary>
        /// Busca un parametro, primero en la cache despues en la base de datos. Timeout en segundos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="NombreParametro"></param>
        /// <param name="ValorPorDefecto"></param>
        /// <param name="Timeout">En segundos</param>
        /// <returns></returns>
        IParametro<T> GetParametroConCache<T>(string NombreParametro, T ValorPorDefecto, TimeSpan Timeout);
        int GetProximoParametroAutoincremental(string NombreParametro);
    }
}