namespace CerebroXMenAPI.app_data
{
    public class CondicionBusqueda
    {

        /// <summary>
        /// Inicializa la condición de busqueda para el objeto enviado como parámetro
        /// tomando "=" como operación por defecto
        /// </summary>
        /// <param name="ObjetoPrimitivo">Debe ser un tipo primitivo: int, decimal, string, bool</param>
        public CondicionBusqueda(object ObjetoPrimitivo) : this("=", ObjetoPrimitivo)
        {
        }

        /// <summary>
        /// Inicializa la condición de busqueda para el objeto enviado como parámetro
        /// </summary>
        /// <param name="ObjetoPrimitivo">Debe ser un tipo primitivo: int, decimal, string, bool</param>
        /// <param name="Operacion">Un string de la forma: = <= >= < > <> </param>
        public CondicionBusqueda(string Operacion, object ObjetoPrimitivo)
        {
            this.ObjetoPrimitivo = ObjetoPrimitivo;
            this.Operacion = Operacion;
        }

        /// <summary>
        /// Debe ser un tipo primitivo: int, decimal, string, bool
        /// </summary>
        public object ObjetoPrimitivo { get; set; }

        /// <summary>
        /// Un string de la forma: = <= >= < > <>
        /// </summary>
        public string Operacion { get; set; }
    }
}
