using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace CerebroXMenAPI.app_data
{
    public class ParametroWebRepositorioDB : IParametroRepositorio
    {
        public IParametro<T> GetParametro<T>(string NombreParametro, T ValorPorDefecto)
        {
            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "SELECT Valor FROM Parametros WHERE ParametroID=@NombreParametro"
                };

                SqlParameter parametro = new SqlParameter("@NombreParametro", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = NombreParametro
                };
                cmd.Parameters.Add(parametro);

                DataTable dt = ConexionSQLServerAutogestionWeb.New().Consultar(cmd);
                if (dt.Rows.Count > 0)
                {
                    return new Parametro<T>(NombreParametro, dt.Rows[0][0].ToString(), ValorPorDefecto, CultureInfo.CurrentCulture);
                }
            }
            catch (Exception ex)
            {
            }

            return new Parametro<T>(NombreParametro, ValorPorDefecto);
        }

        public IParametro<T> GetParametroConCache<T>(string NombreParametro, T ValorPorDefecto, TimeSpan Timeout)
        {
            string clave = $"ParametroWeb_{NombreParametro}";
            IParametro<T> parametro = CacheFactory.GetDefaultCache().ObtenerValor<IParametro<T>>(clave);
            if (parametro != null) return parametro;

            parametro = GetParametro(NombreParametro, ValorPorDefecto);
            CacheFactory.GetDefaultCache().Agregar(clave, parametro, Timeout);
            return parametro;
        }

        public int GetProximoParametroAutoincremental(string NombreParametro)
        {
            int operacionid = 0;

            try
            {
                SqlCommand cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = @"DECLARE @ID int; 
                            SELECT @ID = Valor + 1 FROM Parametros WHERE ParametroID = @NombreParametro;
                                    UPDATE Parametros SET Valor = @ID WHERE ParametroID = @NombreParametro2;
                            SELECT @ID; "
                };

                SqlParameter param = new SqlParameter("@NombreParametro", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = NombreParametro
                };
                cmd.Parameters.Add(param);

                param = new SqlParameter("@NombreParametro2", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = NombreParametro
                };
                cmd.Parameters.Add(param);

                DataTable dt = ConexionSQLServerAutogestionWeb.New().Consultar(cmd);
                operacionid = Convert.ToInt32(dt.Rows[0][0]) + 1;
            }
            catch (Exception ex)
            {
            }

            return operacionid;
        }
    }
}
