using System.Configuration;

namespace CerebroXMenAPI.app_data
{
    public class DBStorageContext : IStorageContext
    {
        public DBStorageContext(string CadenaConexion)
        {
            ConexionSQLServerAutogestionWeb.CadenaConexion = CadenaConexion;

            ParametroWebRepositorio = new ParametroWebRepositorioDB();

        }

        public IParametroRepositorio ParametroWebRepositorio { get; set; }
    }
}