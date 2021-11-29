using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using CerebroXMenAPI.app_data;

namespace CerebroXMenAPI.app_data
{
    public abstract class ConexionSQLServer : IConexion, IDisposable
    {
        public string NombreConexion { get; set; }
        public string ConnectionString { get; set; }

        public bool Conectado { get; set; }

        protected bool DisposedValue { get; set; }

        protected SqlConnection ConexionSql { get; set; }

        protected SqlTransaction TransaccionSQL { get; set; }

        protected ConexionSQLServer(string NombreConexion, string CadenaConexion)
        {
            ConnectionString = CadenaConexion;
            this.NombreConexion = NombreConexion;
            DisposedValue = false;
            Conectado = false;
        }

        public string GetDetalleConexion()
        {
            SqlConnectionStringBuilder conectionStringBuilder = new SqlConnectionStringBuilder(ConnectionString);
            return $"{NombreConexion}: {conectionStringBuilder.DataSource} - {conectionStringBuilder.InitialCatalog}";
        }

        public void Conectar()
        {
            if (Conectado) return; // Ya estoy conectado

            ConexionSql = new SqlConnection(ConnectionString);
            ConexionSql.Open();
            Conectado = true;
        }

        public void Desconectar()
        {
            ConexionSql.Close();
            Conectado = false;
        }

        public void IniciarTransaccion()
        {
            TransaccionSQL = ConexionSql.BeginTransaction();
        }

        public void FinalizarTransaccion(bool Commit)
        {
            try
            {
                if (ConexionSql.State == ConnectionState.Open && TransaccionSQL != null)
                {
                    if (Commit == false)
                    {
                        TransaccionSQL.Rollback();
                    }
                    else
                    {
                        TransaccionSQL.Commit();
                    }
                }
                TransaccionSQL = null;
            }
            catch (Exception ex)
            {
                TransaccionSQL = null;
                throw new Exception("Error al Cerrar la Transacción en la base de datos (SQLServer)", ex);
            }
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    Desconectar();
                }
                DisposedValue = true;
            }
        }

        // Este código se agrega para implementar correctamente el patrón descartable.
        public void Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
            Dispose(true);
        }
        #endregion

        #region Obsoletos
        [Obsolete("Usar metodo sin parametro de log")]
        public DataTable Consultar(IDbCommand Cmd, System.Reflection.MethodBase Metodo) => Consultar(Cmd, Metodo.DeclaringType.Name, Metodo.Name);

        [Obsolete("Usar metodo sin parametro de log")]
        public void EjecutarSPNonQuery(IDbCommand Cmd, System.Reflection.MethodBase Metodo) => EjecutarNonQuery(Cmd, Metodo.DeclaringType.Name, Metodo.Name);

        [Obsolete("Usar metodo sin parametro de log")]
        public DataTable EjecutarSPConsulta(IDbCommand Cmd, string TablaTemporal, System.Reflection.MethodBase Metodo) => throw new NotImplementedException();

        [Obsolete("Usar metodo con nuevo QueryBD")]
        public DataTable Consultar<T>(Query<T> Query, string Tabla, System.Reflection.MethodBase Metodo) => Consultar(Query, Tabla, Metodo.DeclaringType.Name, Metodo.Name);

        [Obsolete("Usar metodo con nuevo QueryBD")]
        public DataTable Consultar<T>(Query<T> Query, string Tabla, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "")
        {
            return Consultar(ArmaConsulta(Query, Tabla), CallerMethod, FileCaller);
        }

        #endregion

        public DataTable Consultar(IDbCommand Cmd, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "")
        {
            DataTable dt = new DataTable();

            bool conecteLocalmente = false;
            if (Conectado == false) // Conectar solo si estoy desconectado
            {
                Conectar();
                conecteLocalmente = true;
            }

            try
            {
                Cmd.Connection = ConexionSql;
                Cmd.Transaction = TransaccionSQL;

                IDbDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = Cmd;

                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables.Count > 0) dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                Exception nEx = new Exception("Error en Consultar SqlServer", new Exception(SqlLoggerService.GetLogCmd(Cmd), ex));
                throw nEx;
            }

            if (conecteLocalmente) // Desconectar solo si conecté localmente
            {
                Desconectar();
            }

            return dt;
        }

        public DataTable Consultar<T>(QueryBD<T> Query, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "") => Consultar(ArmaConsulta(Query), CallerMethod, FileCaller);

        public void EjecutarSPNonQuery(IDbCommand Cmd, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "") => EjecutarNonQuery(Cmd, CallerMethod, FileCaller);

        public void EjecutarNonQuery(IDbCommand Cmd, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "")
        {
            bool conecteLocalmente = false;
            if (Conectado == false) // Conectar solo si estoy desconectado
            {
                Conectar();
                conecteLocalmente = true;
            }

            try
            {
                Cmd.Connection = ConexionSql;
                Cmd.Transaction = TransaccionSQL;

                SqlCommand sqlCmd = Cmd as SqlCommand;
                sqlCmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Exception nEx = new Exception("Error en Consultar SqlServer", new Exception(SqlLoggerService.GetLogCmd(Cmd), ex));
                throw nEx;
            }

            if (conecteLocalmente) // Desconectar solo si conecté localmente
            {
                Desconectar();
            }
        }

        public DataTable EjecutarSPConsulta(IDbCommand Cmd, string TablaTemporal, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "")
        {
            throw new NotImplementedException();
        }

        private static SqlDbType GetSqlDbType(object ObjetoPrimitivo)
        {
            if (ObjetoPrimitivo.GetType() == typeof(Guid))
            {
                return SqlDbType.UniqueIdentifier;
            }

            switch (Type.GetTypeCode(ObjetoPrimitivo.GetType()))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                    return SqlDbType.Int;
                case TypeCode.Int64:
                    return SqlDbType.BigInt;
                case TypeCode.String:
                    return SqlDbType.VarChar;
                case TypeCode.Decimal:
                    return SqlDbType.Decimal;
                case TypeCode.DateTime:
                    return SqlDbType.DateTime;
                case TypeCode.Boolean:
                    return SqlDbType.Bit;
                default:
                    throw new Exception("El objeto no es de un tipo primitivo o es nulo");
            }
        }

        private IDbCommand ArmaConsulta<T>(Query<T> Query, string Tabla)
        {
            if (Query.ComandoDefinidoPorUsuario != null) return Query.ComandoDefinidoPorUsuario as SqlCommand;

            if (Query.WithNoLock == true)
            {
                Tabla += " with (nolock) ";
            }

            string v_strCmdText = $"Select * FROM {Tabla} WHERE ";
            if (Query.SoloTop1)
            {
                v_strCmdText = $"Select top 1 * FROM {Tabla} WHERE ";
            }

            List<SqlParameter> parametrosSql = new List<SqlParameter>();

            foreach (KeyValuePair<T, CondicionBusqueda> condicion in Query.CondicionesDeBusqueda)
            {
                string campo = condicion.Key.ToString();
                CondicionBusqueda v_clsCondicion = condicion.Value;

                if (v_clsCondicion.ObjetoPrimitivo == null)
                {
                    v_strCmdText += $"{campo} {v_clsCondicion.Operacion} AND ";
                }
                else
                {
                    v_strCmdText += $"{campo} {v_clsCondicion.Operacion} @{campo} AND ";

                    parametrosSql.Add(new SqlParameter
                    {
                        ParameterName = campo,
                        SqlDbType = GetSqlDbType(v_clsCondicion.ObjetoPrimitivo),
                        Value = v_clsCondicion.ObjetoPrimitivo
                    });
                }
            }

            v_strCmdText += " 1=1 ";

            if (string.IsNullOrWhiteSpace(Query.OrderBy) == false) v_strCmdText += $" ORDER BY {Query.OrderBy}";

            SqlCommand cmd = new SqlCommand
            {
                CommandText = v_strCmdText
            };
            cmd.Parameters.AddRange(parametrosSql.ToArray());
            return cmd;
        }
        private IDbCommand ArmaConsulta<T>(QueryBD<T> Query)
        {
            if (Query.ComandoDefinidoPorUsuario != null) return Query.ComandoDefinidoPorUsuario as SqlCommand;

            string sql = $"Select {(Query.SoloTop1 ? "top 1" : "")}* FROM {Query.Tabla}{(Query.WithNoLock ? " with (nolock) " : "")} WHERE ";

            List<SqlParameter> parametros = new List<SqlParameter>();

            foreach (KeyValuePair<T, CondicionBusqueda> condicion in Query.CondicionesDeBusqueda)
            {
                CondicionBusqueda condicionbusqueda = condicion.Value;

                if (condicionbusqueda.ObjetoPrimitivo == null)
                {
                    sql += $"{condicion.Key} {condicionbusqueda.Operacion} AND ";
                }
                else
                {
                    sql += $"{condicion.Key} {condicionbusqueda.Operacion} @{condicion.Key} AND ";

                    parametros.Add(new SqlParameter
                    {
                        ParameterName = condicion.Key.ToString(),
                        SqlDbType = GetSqlDbType(condicionbusqueda.ObjetoPrimitivo),
                        Value = condicionbusqueda.ObjetoPrimitivo
                    });
                }
            }

            sql += " 1=1 ";

            if (string.IsNullOrWhiteSpace(Query.OrderBy) == false) sql += $" ORDER BY {Query.OrderBy}";

            SqlCommand cmd = new SqlCommand
            {
                CommandText = sql
            };
            cmd.Parameters.AddRange(parametros.ToArray());
            return cmd;
        }

        #region Async
        public async Task ConectarAsync()
        {
            if (Conectado) return; // Ya estoy conectado

            ConexionSql = new SqlConnection(ConnectionString);
            await ConexionSql.OpenAsync();
            Conectado = true;
        }

        public async Task EjecutarSPNonQueryAsync(IDbCommand Cmd, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "")
        {
            await EjecutarNonQueryAsync(Cmd, CallerMethod, FileCaller);
        }

        public async Task EjecutarNonQueryAsync(IDbCommand Cmd, [CallerMemberName] string CallerMethod = "", [CallerFilePath] string FileCaller = "")
        {
            bool conecteLocalmente = false;
            if (Conectado == false) // Conectar solo si estoy desconectado
            {
                Conectar();
                conecteLocalmente = true;
            }

            try
            {
                Cmd.Connection = ConexionSql;
                Cmd.Transaction = TransaccionSQL;

                SqlCommand cmd = Cmd as SqlCommand;
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Exception nEx = new Exception("Error en Consultar SqlServer", new Exception(SqlLoggerService.GetLogCmd(Cmd), ex));
                throw nEx;
            }

            if (conecteLocalmente) // Desconectar solo si conecté localmente
            {
                Desconectar();
            }
        }

        #endregion
    }
}