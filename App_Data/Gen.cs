using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;

/// La clase persiste en SQLServer. 

namespace CerebroXMenAPI.app_data
{
    using Clase = Gen;
    // Definir Alias:    
    using CustomCondicionBusqueda = GenCondicionBusqueda;

    // Mapeo con la BD. Los nombres deben coincidir con los de la tabla.
    // Solo incluir los campos que se utilizarán como filtros de búsqueda.
    public enum GenCondicionBusqueda { GenID, Activo, PorDefecto, RequiereSeguimiento }

    public class Gen : ClasePersistente
    {
        private const string c_strTabla = "dbo.Genes";
        private const string c_strNombreClase = "Gen";
        private const string c_strNombreParaMostrar = "Gen";

        private int _id;
        private string _adn;
        private bool _esMutante;
        private DateTime _fechaAlta;

        public Gen() { }
        public Gen(DataRow dr) => CargaDatos(dr);
        public Gen(bool pInstanciada) { Instanciada = pInstanciada; }

        public Gen(int pGenID, bool pInstanciada)
        {
            Instanciada = pInstanciada;
            _id = pGenID;
        }

        #region Atributos

        public static string Tabla => c_strTabla;

        public static string NombreClase => c_strNombreClase;

        public static string NombreParaMostrar => c_strNombreParaMostrar;

        public override string ClaveStr => _id.ToString();

        // Clave
        public int ID
        {
            get => _id;
            set => _id = value;
        }

        public string ADN
        {
            get
            {
                if (Instanciada == false) Abrir();
                return _adn;
            }
            set => _adn = value;
        }

        public bool EsMutante
        {
            get
            {
                if (Instanciada == false) Abrir();
                return _esMutante;
            }
            set => _esMutante = value;
        }

        public DateTime FechaAlta
        {
            get
            {
                if (Instanciada == false) Abrir();
                return _fechaAlta;
            }
            set => _fechaAlta = value;
        }
        #endregion

        #region ABM

        public override void Abrir()
        {
            Instanciada = true;

            QueryBD<CustomCondicionBusqueda> q = new QueryBD<CustomCondicionBusqueda>(Tabla);

            q.AgregarCondicion(CustomCondicionBusqueda.GenID, "=", ID);

            DataTable dt = ConexionSQLServerAutogestionWeb.New().Consultar(q);

            try
            {
                if (dt.Rows.Count > 0)
                {
                    CargaDatos(dt.Rows[0]);
                }
                else
                {
                    throw new Exception("Se está intentando abrir un objeto que no existe en la BD");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al intentar abrir el objeto {NombreClase}. Clave: {ClaveStr}", ex);
            }
        }


        private void CargaDatos(DataRow pDataRow)
        {
            Instanciada = true;

            _id = Convert.ToInt32(pDataRow["ID"]);
            _adn = Convert.ToString(pDataRow["ADN"]);
            _esMutante = Convert.ToBoolean(pDataRow["EsMutante"]);
            _fechaAlta = Convert.ToDateTime(pDataRow["FechaAlta"]);

        }
        public static List<Clase> Recuperar<T>(ConexionSQLServerAutogestionWeb Cn, QueryBD<T> Query)
        {
            DataTable dt = Cn.Consultar(Query);

            List<Clase> objetos = new List<Clase>();

            try
            {
                objetos.AddRange(from DataRow dr in dt.Rows
                                 select new Clase(dr));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al recuperar consulta de objetos {NombreClase}", ex);
            }

            return objetos;
        }



        #endregion



        public void Insertar(ConexionSQLServerAutogestionWeb Cn) => EjecutaABM(Cn, TipoABM.Alta);

        public void Actualizar(ConexionSQLServerAutogestionWeb Cn) => EjecutaABM(Cn, TipoABM.Modificacion);

        public void Eliminar(ConexionSQLServerAutogestionWeb Cn) => EjecutaABM(Cn, TipoABM.Baja);

        private void EjecutaABM(ConexionSQLServerAutogestionWeb Cn, TipoABM pTipoABM)
        {
            string v_strOperacion = pTipoABM.ToString().Substring(0, 1);

            SqlCommand v_cmd = new SqlCommand();

            try
            {
                // Si es un alta, generar el id 
                if (pTipoABM == TipoABM.Alta)
                {
                    _id = 0;
                }

                SqlParameter param = new SqlParameter("@Op", SqlDbType.Char)
                {
                    Value = v_strOperacion,
                    Direction = ParameterDirection.Input
                };
                v_cmd.Parameters.Add(param);

                param = new SqlParameter("@GenID", SqlDbType.Int)
                {
                    Value = _id,
                    Direction = ParameterDirection.InputOutput
                };
                v_cmd.Parameters.Add(param);

                param = new SqlParameter("@ADN", SqlDbType.VarChar)
                {
                    Value = ADN,
                    Direction = ParameterDirection.Input
                };
                v_cmd.Parameters.Add(param);

                param = new SqlParameter("@EsMutante", SqlDbType.Bit)
                {
                    Value = EsMutante,
                    Direction = ParameterDirection.Input
                };
                v_cmd.Parameters.Add(param);

                param = new SqlParameter("@FechaAlta", SqlDbType.DateTime)
                {
                    Value = FechaAlta,
                    Direction = ParameterDirection.Input
                };
                v_cmd.Parameters.Add(param);

                param = new SqlParameter("@Error", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                v_cmd.Parameters.Add(param);

                v_cmd.CommandType = CommandType.StoredProcedure;
                v_cmd.CommandText = "dbo.spABMGenes";
                Cn.EjecutarSPNonQuery(v_cmd);

                // Recuperar el id del objeto ingresado
                _id = Convert.ToInt32(v_cmd.Parameters["@GenID"].Value);

                AgregarGenAlCache();
            }
            catch (Exception ex)
            {
                Cn.FinalizarTransaccion(false);
                Cn.Desconectar();

                throw new Exception($"Error al ejecutar {pTipoABM} en EjecutaABM {NombreClase}", ex);
            }
        }

        private void AgregarGenAlCache()
        {
            List<Clase> genes = CacheFactory.GetDefaultCache().ObtenerValor<List<Clase>>("LstGenes");
            Clase gen = new Clase(true)
            {
                ID = _id,
                ADN = _adn,
                EsMutante = _esMutante,
                FechaAlta = _fechaAlta
            };
            genes.Add(gen);
            CacheFactory.GetDefaultCache().Borrar("LstGenes");
            CacheFactory.GetDefaultCache().Agregar("LstGenes", genes, TimeSpan.FromMinutes(5));
        }

        public static Clase Get(ConexionSQLServerAutogestionWeb Cn, int ID)
        {
            List<Clase> genes = CacheFactory.GetDefaultCache().ObtenerValor<List<Clase>>("LstGenes");
            if (genes == null)
            {
                var q = new QueryBD<CustomCondicionBusqueda>(Tabla);
                genes = Recuperar(Cn, q);

                CacheFactory.GetDefaultCache().Agregar("LstGenes", genes, TimeSpan.FromMinutes(30));
            }

            Clase gen = genes.Find(g => g.ID == ID);
            return gen;
        }

        public static List<Clase> LstGenes(ConexionSQLServerAutogestionWeb Cn)
        {
            List<Clase> genes = CacheFactory.GetDefaultCache().ObtenerValor<List<Clase>>("LstGenes");
            if (genes != null) return genes;

            var q = new QueryBD<CustomCondicionBusqueda>(Tabla);
            genes = Recuperar(Cn, q);

            CacheFactory.GetDefaultCache().Agregar("LstGenes", genes, TimeSpan.FromMinutes(30));
            return genes;
        }

        public static bool ExisteGen(ConexionSQLServerAutogestionWeb Cn, string ADN)
        {
            List<Clase> genes = LstGenes(Cn);
            Clase gen = genes.Find(p => p.ADN == ADN);
            if (gen == null)
            {
                return false;
            }
            return true;
        }
    }
}
