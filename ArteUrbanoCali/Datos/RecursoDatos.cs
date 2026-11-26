using ArteUrbanoCali.Entidades;
using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    public class RecursoDatos
    {
        private const string SelectBase = @"
            SELECT r.idRecurso, r.nombreRecurso, t.nombreTipoRecurso, r.descripcionRecurso, r.rutaArchivo,
                   f.extension, r.idFormatoFK, r.tamanio, r.duracion, r.fechaRegistroRecurso,
                   r.idObraFK, o.nombreObra
              FROM recursos_multimedias r
              JOIN formatos_recursos f ON f.idFormato = r.idFormatoFK
              JOIN tipos_recursos t    ON t.idTipoRecurso = f.idTipoRecursoFK
              JOIN obras o             ON o.idObra = r.idObraFK";

        /// <summary>Formatos permitidos con su tipo y carpeta (catálogo de la base de datos).</summary>
        public List<FormatoRecurso> ListarFormatos()
        {
            const string sql = @"
                SELECT f.idFormato, f.extension, t.idTipoRecurso, t.nombreTipoRecurso, t.carpetaBase
                  FROM formatos_recursos f
                  JOIN tipos_recursos t ON t.idTipoRecurso = f.idTipoRecursoFK
                 ORDER BY t.nombreTipoRecurso, f.extension";
            var lista = new List<FormatoRecurso>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new FormatoRecurso
                {
                    IdFormato = rd.GetString(0),
                    Extension = rd.GetString(1),
                    IdTipo = rd.GetString(2),
                    Tipo = rd.GetString(3),
                    Carpeta = rd.GetString(4)
                });
            }
            return lista;
        }

        /// <summary>Recursos de una obra (todos los recursos si idObra es null).</summary>
        public List<RecursoMultimedia> Listar(string? idObra)
        {
            string sql = SelectBase + (idObra == null ? "" : " WHERE r.idObraFK = @obra") +
                         " ORDER BY o.nombreObra, t.nombreTipoRecurso, r.nombreRecurso";
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
                    IdFormato = rd.GetString(6),
                    TamanoKb = rd.GetDecimal(7),
                    DuracionSegundos = rd.IsDBNull(8) ? null : rd.GetInt32(8),
                    FechaRegistro = rd.GetDateTime(9),
                    IdObra = rd.GetString(10),
                    NombreObra = rd.GetString(11)
                });
            }
            return lista;
        }

        public bool ExisteRuta(string rutaArchivo, string? excluirId)
        {
            const string sql = "SELECT COUNT(*) FROM recursos_multimedias WHERE rutaArchivo = @ruta AND idRecurso <> @id";
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
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM obras WHERE idObra = @id", cn);
            cmd.Parameters.AddWithValue("@id", idObra);
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public void Insertar(RecursoMultimedia r)
        {
            const string sql = @"
                INSERT INTO recursos_multimedias
                  (idRecurso, nombreRecurso, descripcionRecurso, rutaArchivo, tamanio, duracion, idFormatoFK, idObraFK)
                VALUES (@id, @nombre, @descripcion, @ruta, @tamano, @duracion, @formato, @obra)";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", r.IdRecurso);
            cmd.Parameters.AddWithValue("@nombre", r.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", Sql.Valor(r.Descripcion));
            cmd.Parameters.AddWithValue("@ruta", r.RutaArchivo);
            cmd.Parameters.AddWithValue("@tamano", r.TamanoKb);
            cmd.Parameters.AddWithValue("@duracion", (object?)r.DuracionSegundos ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@formato", r.IdFormato);
            cmd.Parameters.AddWithValue("@obra", r.IdObra);
            cmd.ExecuteNonQuery();
        }

        // Al modificar solo cambian los datos descriptivos; el archivo y la obra no se tocan.
        public void Actualizar(RecursoMultimedia r)
        {
            const string sql = @"
                UPDATE recursos_multimedias
                   SET nombreRecurso = @nombre, descripcionRecurso = @descripcion, duracion = @duracion
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
            using var cmd = new MySqlCommand("DELETE FROM recursos_multimedias WHERE idRecurso = @id", cn);
            cmd.Parameters.AddWithValue("@id", idRecurso);
            cmd.ExecuteNonQuery();
        }
    }
}
