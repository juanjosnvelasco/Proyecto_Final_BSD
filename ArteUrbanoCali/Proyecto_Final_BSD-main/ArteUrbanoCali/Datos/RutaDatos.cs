using ArteUrbanoCali.Entidades;
using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    public class RutaDatos
    {
        public List<Ruta> Listar()
        {
            const string sql = @"
                SELECT r.idRuta, r.nombreRuta, r.descripcionRuta, r.duracionEstimada,
                       r.distanciaEstimada, r.estadoRuta,
                       (SELECT COUNT(*) FROM rutas_obras ro WHERE ro.idRutaFK = r.idRuta) AS totalObras
                  FROM rutas r
                 ORDER BY r.nombreRuta";
            var lista = new List<Ruta>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new Ruta
                {
                    IdRuta = rd.GetString(0),
                    Nombre = rd.GetString(1),
                    Descripcion = rd.TextoONulo(2),
                    DuracionMinutos = rd.GetInt32(3),
                    DistanciaKm = rd.GetDecimal(4),
                    Estado = rd.GetBoolean(5),
                    TotalObras = Convert.ToInt32(rd.GetValue(6))
                });
            }
            return lista;
        }

        public List<ItemCombo> ListarRutasCombo()
        {
            return ListarItems("SELECT idRuta, nombreRuta FROM rutas ORDER BY nombreRuta");
        }

        public List<ItemCombo> ListarObrasCombo()
        {
            return ListarItems(@"
                SELECT o.idObra, CONCAT(o.nombreObra, ' - ', a.nombreArtistico)
                  FROM obras o JOIN artistas a ON a.idArtista = o.idArtistaFK
                 ORDER BY o.nombreObra");
        }

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

        public bool ExisteNombre(string nombre, string? excluirId)
        {
            const string sql = "SELECT COUNT(*) FROM rutas WHERE nombreRuta = @nombre AND idRuta <> @id";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@id", excluirId ?? "");
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public int ContarObras(string idRuta)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM rutas_obras WHERE idRutaFK = @ruta", cn);
            cmd.Parameters.AddWithValue("@ruta", idRuta);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void Insertar(Ruta r)
        {
            const string sql = @"
                INSERT INTO rutas (idRuta, nombreRuta, descripcionRuta, duracionEstimada, distanciaEstimada, estadoRuta)
                VALUES (@id, @nombre, @descripcion, @duracion, @distancia, @estado)";
            Ejecutar(sql, r);
        }

        public void Actualizar(Ruta r)
        {
            const string sql = @"
                UPDATE rutas
                   SET nombreRuta = @nombre, descripcionRuta = @descripcion, duracionEstimada = @duracion,
                       distanciaEstimada = @distancia, estadoRuta = @estado
                 WHERE idRuta = @id";
            Ejecutar(sql, r);
        }

        private static void Ejecutar(string sql, Ruta r)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", r.IdRuta);
            cmd.Parameters.AddWithValue("@nombre", r.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", Sql.Valor(r.Descripcion));
            cmd.Parameters.AddWithValue("@duracion", r.DuracionMinutos);
            cmd.Parameters.AddWithValue("@distancia", r.DistanciaKm);
            cmd.Parameters.AddWithValue("@estado", r.Estado);
            cmd.ExecuteNonQuery();
        }

        // ---------- Obras de una ruta (tabla asociativa rutas_obras) ----------

        public List<RutaObra> ListarObras(string idRuta)
        {
            const string sql = @"
                SELECT ro.idRutaObra, ro.idRutaFK, ro.idObraFK, ro.ordenVisita, ro.recomendacion,
                       ro.tiempoPermanencia, o.nombreObra, u.nombreLugar, b.nombreBarrio, u.direccion
                  FROM rutas_obras ro
                  JOIN obras o       ON o.idObra      = ro.idObraFK
                  JOIN ubicaciones u ON u.idUbicacion = o.idUbicacionFK
                  JOIN barrios b     ON b.idBarrio    = u.idBarrioFK
                 WHERE ro.idRutaFK = @ruta
                 ORDER BY ro.ordenVisita";
            var lista = new List<RutaObra>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@ruta", idRuta);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                lista.Add(new RutaObra
                {
                    IdRutaObra = rd.GetString(0),
                    IdRuta = rd.GetString(1),
                    IdObra = rd.GetString(2),
                    Orden = rd.GetInt32(3),
                    Recomendacion = rd.TextoONulo(4),
                    TiempoMinutos = rd.GetInt32(5),
                    NombreObra = rd.GetString(6),
                    NombreLugar = rd.GetString(7),
                    NombreBarrio = rd.GetString(8),
                    Direccion = rd.GetString(9)
                });
            }
            return lista;
        }

        public bool ObraEnRuta(string idRuta, string idObra)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM rutas_obras WHERE idRutaFK = @ruta AND idObraFK = @obra", cn);
            cmd.Parameters.AddWithValue("@ruta", idRuta);
            cmd.Parameters.AddWithValue("@obra", idObra);
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public bool OrdenUsado(string idRuta, int orden)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM rutas_obras WHERE idRutaFK = @ruta AND ordenVisita = @orden", cn);
            cmd.Parameters.AddWithValue("@ruta", idRuta);
            cmd.Parameters.AddWithValue("@orden", orden);
            return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
        }

        public int SiguienteOrden(string idRuta)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(
                "SELECT COALESCE(MAX(ordenVisita), 0) + 1 FROM rutas_obras WHERE idRutaFK = @ruta", cn);
            cmd.Parameters.AddWithValue("@ruta", idRuta);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void AgregarObra(RutaObra ro)
        {
            const string sql = @"
                INSERT INTO rutas_obras (idRutaObra, idRutaFK, idObraFK, ordenVisita, recomendacion, tiempoPermanencia)
                VALUES (@id, @ruta, @obra, @orden, @recomendacion, @tiempo)";
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", ro.IdRutaObra);
            cmd.Parameters.AddWithValue("@ruta", ro.IdRuta);
            cmd.Parameters.AddWithValue("@obra", ro.IdObra);
            cmd.Parameters.AddWithValue("@orden", ro.Orden);
            cmd.Parameters.AddWithValue("@recomendacion", Sql.Valor(ro.Recomendacion));
            cmd.Parameters.AddWithValue("@tiempo", ro.TiempoMinutos);
            cmd.ExecuteNonQuery();
        }

        // Retira la obra y vuelve a numerar las siguientes para que el orden no tenga huecos.
        public void RetirarObra(string idRutaObra)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var tx = cn.BeginTransaction();

            string idRuta;
            int orden;
            using (var cmd = new MySqlCommand(
                "SELECT idRutaFK, ordenVisita FROM rutas_obras WHERE idRutaObra = @id", cn, tx))
            {
                cmd.Parameters.AddWithValue("@id", idRutaObra);
                using var rd = cmd.ExecuteReader();
                if (!rd.Read()) { return; }
                idRuta = rd.GetString(0);
                orden = rd.GetInt32(1);
            }

            using (var cmd = new MySqlCommand("DELETE FROM rutas_obras WHERE idRutaObra = @id", cn, tx))
            {
                cmd.Parameters.AddWithValue("@id", idRutaObra);
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new MySqlCommand(@"
                UPDATE rutas_obras SET ordenVisita = ordenVisita - 1
                 WHERE idRutaFK = @ruta AND ordenVisita > @orden
                 ORDER BY ordenVisita", cn, tx))
            {
                cmd.Parameters.AddWithValue("@ruta", idRuta);
                cmd.Parameters.AddWithValue("@orden", orden);
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }

        // Intercambia el orden de dos obras vecinas usando un valor temporal por la restricción UNIQUE (ruta, orden).
        public void IntercambiarOrden(string idRutaObraA, int ordenA, string idRutaObraB, int ordenB)
        {
            using var cn = Conexion.Crear();
            cn.Open();
            using var tx = cn.BeginTransaction();
            Actualizar(cn, tx, idRutaObraA, 1000000);
            Actualizar(cn, tx, idRutaObraB, ordenA);
            Actualizar(cn, tx, idRutaObraA, ordenB);
            tx.Commit();
        }

        private static void Actualizar(MySqlConnection cn, MySqlTransaction tx, string id, int orden)
        {
            using var cmd = new MySqlCommand("UPDATE rutas_obras SET ordenVisita = @orden WHERE idRutaObra = @id", cn, tx);
            cmd.Parameters.AddWithValue("@orden", orden);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
