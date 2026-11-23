using ArteUrbanoCali.Entidades;
using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    public class ArtistaDatos
    {
        public List<Artista> Listar()
        {
            const string sql = @"
                SELECT a.idArtista, a.nombreArtistico, a.nombre1Real, a.nombre2Real,
                       a.apellido1Real, a.apellido2Real, a.biografia, a.perfilRedSocial,
                       a.estadoArtista, a.idPaisFK, p.nombrePais
                  FROM ARTISTAS a
                  JOIN PAISES p ON p.idPais = a.idPaisFK
                 ORDER BY a.nombreArtistico";

            var lista = new List<Artista>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Artista
                {
                    IdArtista = rd.GetString(0),
                    NombreArtistico = rd.GetString(1),
                    Nombre1Real = rd.GetString(2),
                    Nombre2Real = rd.TextoONulo(3),
                    Apellido1Real = rd.GetString(4),
                    Apellido2Real = rd.TextoONulo(5),
                    Biografia = rd.TextoONulo(6),
                    PerfilRedSocial = rd.TextoONulo(7),
                    Estado = rd.GetBoolean(8),
                    IdPais = rd.GetString(9),
                    NombrePais = rd.GetString(10)
                });
            }
            return lista;
        }

        // ¿Ya existe otro artista con ese nombre artístico? (excluye al que se está editando)
        public bool ExisteNombre(string nombreArtistico, string? excluirId)
        {
            const string sql = "SELECT COUNT(*) FROM ARTISTAS WHERE nombreArtistico = @nombre AND idArtista <> @id";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@nombre", nombreArtistico);
            cmd.Parameters.AddWithValue("@id", excluirId ?? "");
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public void Insertar(Artista a)
        {
            const string sql = @"
                INSERT INTO ARTISTAS
                  (idArtista, nombreArtistico, nombre1Real, nombre2Real, apellido1Real, apellido2Real,
                   biografia, perfilRedSocial, estadoArtista, idPaisFK)
                VALUES
                  (@id, @nombreArtistico, @nombre1, @nombre2, @apellido1, @apellido2,
                   @biografia, @perfil, @estado, @pais)";
            Ejecutar(sql, a);
        }

        public void Actualizar(Artista a)
        {
            const string sql = @"
                UPDATE ARTISTAS
                   SET nombreArtistico = @nombreArtistico, nombre1Real = @nombre1, nombre2Real = @nombre2,
                       apellido1Real = @apellido1, apellido2Real = @apellido2, biografia = @biografia,
                       perfilRedSocial = @perfil, estadoArtista = @estado, idPaisFK = @pais
                 WHERE idArtista = @id";
            Ejecutar(sql, a);
        }

        public void CambiarEstado(string idArtista, bool estado)
        {
            const string sql = "UPDATE ARTISTAS SET estadoArtista = @estado WHERE idArtista = @id";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@estado", estado);
            cmd.Parameters.AddWithValue("@id", idArtista);
            cmd.ExecuteNonQuery();
        }

        private static void Ejecutar(string sql, Artista a)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", a.IdArtista);
            cmd.Parameters.AddWithValue("@nombreArtistico", a.NombreArtistico);
            cmd.Parameters.AddWithValue("@nombre1", a.Nombre1Real);
            cmd.Parameters.AddWithValue("@nombre2", Sql.Valor(a.Nombre2Real));
            cmd.Parameters.AddWithValue("@apellido1", a.Apellido1Real);
            cmd.Parameters.AddWithValue("@apellido2", Sql.Valor(a.Apellido2Real));
            cmd.Parameters.AddWithValue("@biografia", Sql.Valor(a.Biografia));
            cmd.Parameters.AddWithValue("@perfil", Sql.Valor(a.PerfilRedSocial));
            cmd.Parameters.AddWithValue("@estado", a.Estado);
            cmd.Parameters.AddWithValue("@pais", a.IdPais);
            cmd.ExecuteNonQuery();
        }
    }
}
