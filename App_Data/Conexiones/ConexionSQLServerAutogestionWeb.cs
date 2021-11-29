using System;

namespace CerebroXMenAPI.app_data
{
    public class ConexionSQLServerAutogestionWeb : ConexionSQLServer
    {
        public static string CadenaConexion { get; set; }

        public static ConexionSQLServerAutogestionWeb New()
        {
            if (string.IsNullOrWhiteSpace(CadenaConexion)) throw new Exception("Cadena de conexion no iniciada.");
            return new ConexionSQLServerAutogestionWeb(CadenaConexion);
        }

        public ConexionSQLServerAutogestionWeb(string CadenaConexion) : base("SqlServerAutogestionWeb", CadenaConexion)
        {
        }
    }
}