using System;
using System.Data;

namespace CerebroXMenAPI.app_data
{
    public enum Genero { Masculino, Femenino }

    public enum TipoABM { Alta, Baja, Modificacion }

    [Serializable]
    public abstract class ClasePersistente
    {
        protected ClasePersistente()
        {
        }

        protected ClasePersistente(bool Instanciada)
        {
            this.Instanciada = Instanciada;
        }


        public bool Instanciada { get; set; } = false;

        public virtual bool ClaveCompleta() => string.IsNullOrWhiteSpace(ClaveStr) == false;

        public virtual string ClaveStr
        {
            get { return ""; }
        }

        public virtual void Abrir()
        { }

        public virtual void Abrir(IConexion pCnn)
        { }

        /// <summary>
        /// Devuelve true si pClaveStr está definida. 
        /// Devuelve false si es de la forma "", "0", "|0", "|", "|#", etc.
        /// Osea, cualquier clave que no represente un objeto en memoria.
        /// </summary>
        public static bool ClaveStrDefinida(string ClaveStr)
        {
            // Quito todos los caracteres separadores de clave
            string s = ClaveStr.Replace('|', ' ').Replace('#', ' ');
            // Quito los 0s
            s = s.Replace('0', ' ');

            // Si queda algún caracter, significa que la clave era válida
            return (string.IsNullOrWhiteSpace(s) == false);
        }

        protected void CargaDatosAuditoria(DataRow dr)
        {
        }

        protected void CargaDatosAuditoriaADABAS(DataRow dr)
        {
        }

        protected static string GetLetraGenero(Genero EnumGenero)
        {
            switch (EnumGenero)
            {
                case Genero.Masculino:
                    return "o";
                default:
                    return "a";
            }
        }
    }
}