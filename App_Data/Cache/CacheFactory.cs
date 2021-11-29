namespace CerebroXMenAPI.app_data
{
    public static class CacheFactory
    {
        public static ICache Instancia { get; set; }

        public static ICache GetDefaultCache()
        {
            if (Instancia != null)
            {
                return Instancia;
            }

            //Refrescar el valor por las dudas que cambie el parametro
            bool v_bolCacheActiva = true; //StorageContextFactory.StoragePorDefecto.ParametroWebRepositorio.GetParametro("CacheActiva", false).Valor;

            switch (v_bolCacheActiva)
            {
                case true:
                    Instancia = new MemoryCacher();
                    break;
                case false:
                    Instancia = new NoneCacher();
                    break;
            }

            return Instancia;
        }
    }
}
