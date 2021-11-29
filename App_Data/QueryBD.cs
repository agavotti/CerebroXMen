using System.Data;
using System.Collections.Generic;

namespace CerebroXMenAPI.app_data
{
    public sealed class QueryBD<T>
    {
        public QueryBD(string Tabla)
        {
            this.Tabla = Tabla;
            CondicionesDeBusqueda = new Dictionary<T, CondicionBusqueda>();
            OrderBy = "";
        }

        public QueryBD(IDbCommand cmd)
        {
            ComandoDefinidoPorUsuario = cmd;
        }

        public string Tabla { get; set; }
        public string OrderBy { get; set; }
        public bool TieneOrderBy() => string.IsNullOrWhiteSpace(OrderBy) == false;
        public bool SoloTop1 { get; set; }
        public bool WithNoLock { get; set; }

        public IDbCommand ComandoDefinidoPorUsuario { get; set; }

        /// <param name="CondicionBusqueda"></param>
        /// <param name="Operacion"> < <= = > >= <> como string</param>
        /// <param name="ObjetoPrimitivo">Un objeto primitivo. Null para ignorar.</param>
        public void AgregarCondicion(T CondicionBusqueda, string Operacion, object ObjetoPrimitivo)
        {
            CondicionBusqueda condicion = new CondicionBusqueda(Operacion, ObjetoPrimitivo);
            CondicionesDeBusqueda.Add(CondicionBusqueda, condicion);
        }

        public Dictionary<T, CondicionBusqueda> CondicionesDeBusqueda { get; }
    }
}
