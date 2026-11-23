using ArteUrbanoCali.Entidades;
using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    public class UbicacionDatos
    {
        public List<Ubicacion> Listar()
        {
            const string sql = @"
                SELECT u.idUbicacion, u.nombreLugar, u.direccion, u.latitud, u.longitud,
                       u.idBarrioFK, b.nombreBarrio, c.numeroComuna
                  FROM UBICACIONES u
                  JOIN BARRIOS b ON b.idBarrio = u.idBarrioFK
                  JOIN COMUNAS c ON c.idComuna = b.idComunaFK
                 ORDER BY c.numeroComuna, b.nombreBarrio, u.nombreLugar";

            var lista = new List<Ubicacion>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Ubicacion
                {
                    IdUbicacion = rd.GetString(0),
                    NombreLugar = rd.GetString(1),
                    Direccion = rd.GetString(2),
                    Latitud = rd.GetDecimal(3),
                    Longitud = rd.GetDecimal(4),
                    IdBarrio = rd.GetString(5),
                    NombreBarrio = rd.GetString(6),
                    NumeroComuna = rd.GetInt32(7)
                });
            }
            return lista;
        }

        public void Insertar(Ubicacion u)
        {
            const string sql = @"
                INSERT INTO UBICACIONES (idUbicacion, nombreLugar, direccion, latitud, longitud, idBarrioFK)
                VALUES (@id, @lugar, @direccion, @latitud, @longitud, @barrio)";
            Ejecutar(sql, u);
        }

        public void Actualizar(Ubicacion u)
        {
            const string sql = @"
                UPDATE UBICACIONES
                   SET nombreLugar = @lugar, direccion = @direccion, latitud = @latitud,
                       longitud = @longitud, idBarrioFK = @barrio
                 WHERE idUbicacion = @id";
            Ejecutar(sql, u);
        }

        private static void Ejecutar(string sql, Ubicacion u)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", u.IdUbicacion);
            cmd.Parameters.AddWithValue("@lugar", u.NombreLugar);
            cmd.Parameters.AddWithValue("@direccion", u.Direccion);
            cmd.Parameters.AddWithValue("@latitud", u.Latitud);
            cmd.Parameters.AddWithValue("@longitud", u.Longitud);
            cmd.Parameters.AddWithValue("@barrio", u.IdBarrio);
            cmd.ExecuteNonQuery();
        }
    }
}
