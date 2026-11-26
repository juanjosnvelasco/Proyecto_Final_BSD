using ArteUrbanoCali.Entidades;
using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    
    public class UsuarioDatos
    {
        
        public Usuario? ObtenerPorCorreo(string correo)
        {
            const string sql = @"
                SELECT u.idUsuario, u.nombre1Usuario, u.apellido1Usuario, u.correoUsuario,
                       u.contrasenaHash, u.estadoUsuario, t.nombreTipoUsuario
                  FROM USUARIOS u
                  JOIN TIPOS_USUARIOS t ON t.idTipoUsuario = u.idTipoUsuarioFK
                 WHERE u.correoUsuario = @correo";

            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@correo", correo);   
            using var rd = cmd.ExecuteReader();

            if (!rd.Read()) return null;

            return new Usuario
            {
                IdUsuario = rd.GetString(0),
                Nombre1 = rd.GetString(1),
                Apellido1 = rd.GetString(2),
                Correo = rd.GetString(3),
                ContrasenaHash = rd.GetString(4),
                Estado = rd.GetBoolean(5),
                TipoUsuario = rd.GetString(6)
            };
        }
    }
}
