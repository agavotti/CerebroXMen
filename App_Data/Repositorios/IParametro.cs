namespace CerebroXMenAPI.app_data
{
    public interface IParametro<T>
    {
        string Nombre { get; set; }
        T Valor { get; set; }
    }
}