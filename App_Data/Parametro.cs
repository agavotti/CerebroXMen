using System;
using System.Globalization;

namespace CerebroXMenAPI.app_data
{
    public class Parametro<T> : IParametro<T>
    {
        public string Nombre { get; set; }
        public T Valor { get; set; }

        public Parametro(string pNombreParametro, T pValorParametro)
        {
            Nombre = pNombreParametro;
            Valor = pValorParametro;
        }

        public Parametro(string pNombreParametro, object value, T pValorPorDefecto, CultureInfo cultureInfo)
        {
            Nombre = pNombreParametro;
            Valor = ConvertirValorParametro(value, pValorPorDefecto, cultureInfo);
        }

        private T ConvertirValorParametro(object value, T pValorPorDefecto, CultureInfo cultureInfo)
        {
            var toType = typeof(T);

            if (value == null) return pValorPorDefecto;

            if (value is string)
            {
                if (toType == typeof(Guid))
                {
                    return ConvertirValorParametro(new Guid(Convert.ToString(value, cultureInfo)), pValorPorDefecto, cultureInfo);
                }

                if (string.IsNullOrEmpty((string)value) && toType != typeof(string))
                {
                    return ConvertirValorParametro(null, pValorPorDefecto, cultureInfo);
                }
            }
            else
            {
                if (typeof(T) == typeof(string))
                {
                    return ConvertirValorParametro(Convert.ToString(value, cultureInfo), pValorPorDefecto, cultureInfo);
                }
            }

            if (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                toType = Nullable.GetUnderlyingType(toType);
            }

            var canConvert = toType is IConvertible || (toType.IsValueType && !toType.IsEnum);
            if (canConvert)
            {
                return (T)Convert.ChangeType(value, toType, cultureInfo);
            }

            return (T)value;
        }
    }
}
