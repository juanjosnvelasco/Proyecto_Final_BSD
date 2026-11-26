using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    internal static class LectorExtensiones
    {
        // Texto de la columna, o null si el valor en MySQL es NULL.
        public static string? TextoONulo(this MySqlDataReader rd, int indice)
        {
            return rd.IsDBNull(indice) ? null : rd.GetString(indice);
        }
    }

    internal static class Sql
    {
        // Convierte null de C# en NULL de MySQL para los parámetros.
        public static object Valor(string? texto)
        {
            return (object?)texto ?? DBNull.Value;
        }
    }
}
