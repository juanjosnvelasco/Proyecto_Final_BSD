using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    public static class Conexion
    {
        
        public static string CadenaConexion =>
            "Server=localhost;Port=3306;Database=arte_urbano_cali;" +
            "User ID=root;Password=root;CharSet=utf8mb4;";

        public static MySqlConnection Crear()
        {
            return new MySqlConnection(CadenaConexion);
        }
    }
}