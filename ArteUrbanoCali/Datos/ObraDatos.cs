using ArteUrbanoCali.Entidades;
using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    public class ObraDatos
    {
        public List<Obra> Listar()
        {
            const string sql = @"
                SELECT o.idObra, o.nombreObra, o.descripcionObra, o.fechaCreacion, o.estadoConservacion,
                       o.idArtistaFK, o.idCategoriaFK, o.idUbicacionFK, o.idUsuarioFK,
                       a.nombreArtistico, c.nombreCategoria, u.nombreLugar, b.nombreBarrio, cm.numeroComuna,
                       (SELECT COUNT(*) FROM RECURSOS_MULTIMEDIAS r WHERE r.idObraFK = o.idObra) AS totalRecursos
                  FROM OBRAS o
                  JOIN ARTISTAS a    ON a.idArtista   = o.idArtistaFK
                  JOIN CATEGORIAS c  ON c.idCategoria = o.idCategoriaFK
                  JOIN UBICACIONES u ON u.idUbicacion = o.idUbicacionFK
                  JOIN BARRIOS b     ON b.idBarrio    = u.idBarrioFK
                  JOIN COMUNAS cm    ON cm.idComuna   = b.idComunaFK
                 ORDER BY o.nombreObra";

            var lista = new List<Obra>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Obra
                {
                    IdObra = rd.GetString(0),
                    Nombre = rd.GetString(1),
                    Descripcion = rd.TextoONulo(2),
                    FechaCreacion = rd.GetDateTime(3),
                    EstadoConservacion = rd.GetString(4),
                    IdArtista = rd.GetString(5),
                    IdCategoria = rd.GetString(6),
                    IdUbicacion = rd.GetString(7),
                    IdUsuario = rd.GetString(8),
                    NombreArtista = rd.GetString(9),
                    NombreCategoria = rd.GetString(10),
                    NombreLugar = rd.GetString(11),
                    NombreBarrio = rd.GetString(12),
                    NumeroComuna = rd.GetInt32(13),
                    TotalRecursos = Convert.ToInt32(rd.GetValue(14))
                });
            }
            return lista;
        }

        // Listas para los combos. Los artistas y categorías inactivos se marcan en el texto.
        public List<ItemCombo> ListarArtistasCombo() => ListarItems(@"
            SELECT idArtista,
                   IF(estadoArtista = 1, nombreArtistico, CONCAT(nombreArtistico, ' (inactivo)'))
              FROM ARTISTAS ORDER BY nombreArtistico");

        public List<ItemCombo> ListarCategoriasCombo() => ListarItems(@"
            SELECT idCategoria,
                   IF(estadoCategoria = 1, nombreCategoria, CONCAT(nombreCategoria, ' (inactiva)'))
              FROM CATEGORIAS ORDER BY nombreCategoria");

        public List<ItemCombo> ListarUbicacionesCombo() => ListarItems(@"
            SELECT u.idUbicacion, CONCAT(u.nombreLugar, ' - ', b.nombreBarrio)
              FROM UBICACIONES u
              JOIN BARRIOS b ON b.idBarrio = u.idBarrioFK
             ORDER BY u.nombreLugar");

        private static List<ItemCombo> ListarItems(string sql)
        {
            var lista = new List<ItemCombo>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(new ItemCombo { Id = rd.GetString(0), Texto = rd.GetString(1) });
            return lista;
        }

        public bool ArtistaActivo(string idArtista) =>
            Contar("SELECT COUNT(*) FROM ARTISTAS WHERE idArtista = @id AND estadoArtista = 1", idArtista) > 0;

        public bool CategoriaActiva(string idCategoria) =>
            Contar("SELECT COUNT(*) FROM CATEGORIAS WHERE idCategoria = @id AND estadoCategoria = 1", idCategoria) > 0;

        private static long Contar(string sql, string id)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", id);
            return Convert.ToInt64(cmd.ExecuteScalar());
        }

        // Una obra se considera duplicada si el mismo artista ya tiene otra con ese nombre.
        public bool ExisteDuplicado(string nombre, string idArtista, string? excluirId)
        {
            const string sql = @"
                SELECT COUNT(*) FROM OBRAS
                 WHERE nombreObra = @nombre AND idArtistaFK = @artista AND idObra <> @id";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@artista", idArtista);
            cmd.Parameters.AddWithValue("@id", excluirId ?? "");
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public void Insertar(Obra o)
        {
            const string sql = @"
                INSERT INTO OBRAS
                  (idObra, nombreObra, descripcionObra, fechaCreacion, estadoConservacion,
                   idArtistaFK, idCategoriaFK, idUbicacionFK, idUsuarioFK)
                VALUES
                  (@id, @nombre, @descripcion, @fecha, @estado, @artista, @categoria, @ubicacion, @usuario)";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            AgregarParametros(cmd, o);
            cmd.Parameters.AddWithValue("@usuario", o.IdUsuario);
            cmd.ExecuteNonQuery();
        }

        // Al modificar no se cambia el usuario que registró la obra.
        public void Actualizar(Obra o)
        {
            const string sql = @"
                UPDATE OBRAS
                   SET nombreObra = @nombre, descripcionObra = @descripcion, fechaCreacion = @fecha,
                       estadoConservacion = @estado, idArtistaFK = @artista,
                       idCategoriaFK = @categoria, idUbicacionFK = @ubicacion
                 WHERE idObra = @id";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            AgregarParametros(cmd, o);
            cmd.ExecuteNonQuery();
        }

        private static void AgregarParametros(MySqlCommand cmd, Obra o)
        {
            cmd.Parameters.AddWithValue("@id", o.IdObra);
            cmd.Parameters.AddWithValue("@nombre", o.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", Sql.Valor(o.Descripcion));
            cmd.Parameters.AddWithValue("@fecha", o.FechaCreacion.Date);
            cmd.Parameters.AddWithValue("@estado", o.EstadoConservacion);
            cmd.Parameters.AddWithValue("@artista", o.IdArtista);
            cmd.Parameters.AddWithValue("@categoria", o.IdCategoria);
            cmd.Parameters.AddWithValue("@ubicacion", o.IdUbicacion);
        }
    }
}
