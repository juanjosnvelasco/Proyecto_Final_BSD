using ArteUrbanoCali.Entidades;
using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    public class CategoriaDatos
    {
        public List<Categoria> Listar()
        {
            const string sql = @"
                SELECT idCategoria, nombreCategoria, descripcionCategoria, estadoCategoria
                  FROM CATEGORIAS
                 ORDER BY nombreCategoria";

            var lista = new List<Categoria>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Categoria
                {
                    IdCategoria = rd.GetString(0),
                    Nombre = rd.GetString(1),
                    Descripcion = rd.TextoONulo(2),
                    Estado = rd.GetBoolean(3)
                });
            }
            return lista;
        }

        public bool ExisteNombre(string nombre, string? excluirId)
        {
            const string sql = "SELECT COUNT(*) FROM CATEGORIAS WHERE nombreCategoria = @nombre AND idCategoria <> @id";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@id", excluirId ?? "");
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public void Insertar(Categoria c)
        {
            const string sql = @"
                INSERT INTO CATEGORIAS (idCategoria, nombreCategoria, descripcionCategoria, estadoCategoria)
                VALUES (@id, @nombre, @descripcion, @estado)";
            Ejecutar(sql, c);
        }

        public void Actualizar(Categoria c)
        {
            const string sql = @"
                UPDATE CATEGORIAS
                   SET nombreCategoria = @nombre, descripcionCategoria = @descripcion, estadoCategoria = @estado
                 WHERE idCategoria = @id";
            Ejecutar(sql, c);
        }

        public void CambiarEstado(string idCategoria, bool estado)
        {
            const string sql = "UPDATE CATEGORIAS SET estadoCategoria = @estado WHERE idCategoria = @id";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@estado", estado);
            cmd.Parameters.AddWithValue("@id", idCategoria);
            cmd.ExecuteNonQuery();
        }

        private static void Ejecutar(string sql, Categoria c)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", c.IdCategoria);
            cmd.Parameters.AddWithValue("@nombre", c.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", Sql.Valor(c.Descripcion));
            cmd.Parameters.AddWithValue("@estado", c.Estado);
            cmd.ExecuteNonQuery();
        }
    }
}
