using ArteUrbanoCali.Entidades;
using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    public class RecursoDatos
    {
        private const string SelectBase = @"
            SELECT r.idRecurso, r.nombreRecurso, r.tipoRecurso, r.descripcionRecurso, r.rutaArchivo,
                   r.formatoRecurso, r.tamanoBytes, r.duracionSegundos, r.fechaRegistro,
                   r.idObraFK, o.nombreObra
              FROM RECURSOS_MULTIMEDIAS r
              JOIN OBRAS o ON o.idObra = r.idObraFK";

        /// <summary>Recursos de una obra (todos los recursos si idObra es null).</summary>
        public List<RecursoMultimedia> Listar(string? idObra)
        {
            string sql = SelectBase + (idObra == null ? "" : " WHERE r.idObraFK = @obra") +
                         " ORDER BY o.nombreObra, r.tipoRecurso, r.nombreRecurso";
            var lista = new List<RecursoMultimedia>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            if (idObra != null) cmd.Parameters.AddWithValue("@obra", idObra);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new RecursoMultimedia
                {
                    IdRecurso = rd.GetString(0),
                    Nombre = rd.GetString(1),
                    Tipo = rd.GetString(2),
                    Descripcion = rd.TextoONulo(3),
                    RutaArchivo = rd.GetString(4),
                    Formato = rd.GetString(5),
                    TamanoBytes = rd.GetInt64(6),
                    DuracionSegundos = rd.IsDBNull(7) ? null : rd.GetInt32(7),
                    FechaRegistro = rd.GetDateTime(8),
                    IdObra = rd.GetString(9),
                    NombreObra = rd.GetString(10)
                });
            }
            return lista;
        }

        public bool ExisteRuta(string rutaArchivo, string? excluirId)
        {
            const string sql = "SELECT COUNT(*) FROM RECURSOS_MULTIMEDIAS WHERE rutaArchivo = @ruta AND idRecurso <> @id";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@ruta", rutaArchivo);
            cmd.Parameters.AddWithValue("@id", excluirId ?? "");
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public bool ExisteObra(string idObra)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM OBRAS WHERE idObra = @id", cn);
            cmd.Parameters.AddWithValue("@id", idObra);
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public void Insertar(RecursoMultimedia r)
        {
            const string sql = @"
                INSERT INTO RECURSOS_MULTIMEDIAS
                  (idRecurso, nombreRecurso, tipoRecurso, descripcionRecurso, rutaArchivo,
                   formatoRecurso, tamanoBytes, duracionSegundos, idObraFK)
                VALUES (@id, @nombre, @tipo, @descripcion, @ruta, @formato, @tamano, @duracion, @obra)";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", r.IdRecurso);
            cmd.Parameters.AddWithValue("@nombre", r.Nombre);
            cmd.Parameters.AddWithValue("@tipo", r.Tipo);
            cmd.Parameters.AddWithValue("@descripcion", Sql.Valor(r.Descripcion));
            cmd.Parameters.AddWithValue("@ruta", r.RutaArchivo);
            cmd.Parameters.AddWithValue("@formato", r.Formato);
            cmd.Parameters.AddWithValue("@tamano", r.TamanoBytes);
            cmd.Parameters.AddWithValue("@duracion", (object?)r.DuracionSegundos ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@obra", r.IdObra);
            cmd.ExecuteNonQuery();
        }

        // Al modificar solo cambian los datos descriptivos; el archivo y la obra no se tocan.
        public void Actualizar(RecursoMultimedia r)
        {
            const string sql = @"
                UPDATE RECURSOS_MULTIMEDIAS
                   SET nombreRecurso = @nombre, descripcionRecurso = @descripcion, duracionSegundos = @duracion
                 WHERE idRecurso = @id";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", r.IdRecurso);
            cmd.Parameters.AddWithValue("@nombre", r.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", Sql.Valor(r.Descripcion));
            cmd.Parameters.AddWithValue("@duracion", (object?)r.DuracionSegundos ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(string idRecurso)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand("DELETE FROM RECURSOS_MULTIMEDIAS WHERE idRecurso = @id", cn);
            cmd.Parameters.AddWithValue("@id", idRecurso);
            cmd.ExecuteNonQuery();
        }
    }
}
