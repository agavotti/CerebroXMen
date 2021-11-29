using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CerebroXMenAPI.app_data
{
    public interface IConexion
    {
        string NombreConexion { get; set; }
        string ConnectionString { get; set; }
        string GetDetalleConexion();

        bool Conectado { get; set; }
        void Conectar();
        Task ConectarAsync();
        void Desconectar();
        void Dispose();

        void IniciarTransaccion();
        void FinalizarTransaccion(bool Commit);

        #region Obsoletos

        [Obsolete("Usar metodo sin parametro de log")]
        DataTable Consultar(IDbCommand Cmd, System.Reflection.MethodBase Metodo);

        [Obsolete("Usar metodo con nuevo QueryBD")]
        DataTable Consultar<T>(Query<T> Query, string Tabla, System.Reflection.MethodBase Metodo);

        [Obsolete("Usar metodo con nuevo QueryBD")]
        DataTable Consultar<T>(Query<T> Query, string Tabla, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "");

        [Obsolete("Usar metodo sin parametro de log")]
        void EjecutarSPNonQuery(IDbCommand Cmd, System.Reflection.MethodBase Metodo);

        [Obsolete("Usar metodo sin parametro de log")]
        DataTable EjecutarSPConsulta(IDbCommand Cmd, string TablaTemporal, System.Reflection.MethodBase Metodo);

        #endregion

        DataTable Consultar(IDbCommand Cmd, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "");

        DataTable Consultar<T>(QueryBD<T> Query, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "");

        void EjecutarNonQuery(IDbCommand Cmd, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "");
        void EjecutarSPNonQuery(IDbCommand Cmd, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "");
        DataTable EjecutarSPConsulta(IDbCommand Cmd, string TablaTemporal, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "");

        Task EjecutarNonQueryAsync(IDbCommand Cmd, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "");
        Task EjecutarSPNonQueryAsync(IDbCommand Cmd, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "");
    }
}