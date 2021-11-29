using System.Data;

namespace CerebroXMenAPI.app_data
{
    public static class SqlLoggerService
    {
        public static string GetLogCmd(IDbCommand Cmd)
        {
            string basedatos = DBNameToString(Cmd);
            string logcmd = $"Conexion: {basedatos} SQL: {Cmd.CommandText}; PARAMETERS: ";

            foreach (IDbDataParameter p in Cmd.Parameters)
            {
                logcmd += SQLParameterToString(p);
            }

            return logcmd;
        }

        private static string DBNameToString(IDbCommand Cmd)
        {
            return Cmd?.Connection?.Database ?? "Desconocida";
        }

        private static string SQLParameterToString(IDbDataParameter Param)
        {
            string valor = Param?.Value?.ToString() ?? "null";
            return $"{Param.ParameterName} = {valor} ({Param.DbType}); ";
        }
    }
}