using System.Data;
using System.Collections.Generic;

namespace CerebroXMenAPI.app_data
{
    public sealed class Query<T>
    {
        public Query()
        {

        }

        public Query(IDbCommand pComando)
        {
            ComandoDefinidoPorUsuario = pComando;
        }

        private string m_strOrderBy = "";

        public string OrderBy
        {
            get { return m_strOrderBy.Trim(); }
            set { m_strOrderBy = value; }
        }
        public bool SoloTop1 { get; set; }
        public bool WithNoLock { get; set; }
        public IDbCommand ComandoDefinidoPorUsuario { get; set; }

        /// <param name="pCondicionBusqueda"></param>
        /// <param name="pOperacion"> < <= = > >= <> como string</param>
        /// <param name="pObjetoPrimitivo">Un objeto primitivo. Null para ignorar.</param>
        public void AgregarCondicion(T pCondicionBusqueda, string pOperacion, object pObjetoPrimitivo)
        {
            CondicionBusqueda v_clsCondicionBusqueda = new CondicionBusqueda(pOperacion, pObjetoPrimitivo);
            CondicionesDeBusqueda.Add(pCondicionBusqueda, v_clsCondicionBusqueda);
        }

        public Dictionary<T, CondicionBusqueda> CondicionesDeBusqueda { get; } = new Dictionary<T, CondicionBusqueda>();
    }
}
