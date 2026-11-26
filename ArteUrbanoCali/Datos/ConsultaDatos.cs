using System.Data;
using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    /// <summary>Consultas que combinan varias tablas. Todas están parametrizadas.</summary>
    public class ConsultaDatos
    {
        private static DataTable Ejecutar(string sql, params (string nombre, object valor)[] parametros)
        {
            var tabla = new DataTable();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            foreach (var (nombre, valor) in parametros)
                cmd.Parameters.AddWithValue(nombre, valor);
            using var rd = cmd.ExecuteReader();
            tabla.Load(rd);
            return tabla;
        }

        private const string ObraConRelaciones = @"
            SELECT o.nombreObra AS Obra, a.nombreArtistico AS Artista, c.nombreCategoria AS Categoria,
                   u.nombreLugar AS Lugar, b.nombreBarrio AS Barrio, cm.numeroComuna AS Comuna,
                   o.estadoConservacion AS Conservacion,
                   COUNT(r.idRecurso) AS Recursos,
                   COALESCE(GROUP_CONCAT(CONCAT(t.nombreTipoRecurso, ': ', r.nombreRecurso) ORDER BY t.nombreTipoRecurso SEPARATOR ' | '), '') AS DetalleRecursos
              FROM obras o
              JOIN artistas a    ON a.idArtista   = o.idArtistaFK
              JOIN categorias c  ON c.idCategoria = o.idCategoriaFK
              JOIN ubicaciones u ON u.idUbicacion = o.idUbicacionFK
              JOIN barrios b     ON b.idBarrio    = u.idBarrioFK
              JOIN comunas cm    ON cm.idComuna   = b.idComunaFK
              LEFT JOIN recursos_multimedias r ON r.idObraFK = o.idObra
              LEFT JOIN formatos_recursos f    ON f.idFormato = r.idFormatoFK
              LEFT JOIN tipos_recursos t       ON t.idTipoRecurso = f.idTipoRecursoFK";

        private const string AgruparObra = @"
             GROUP BY o.idObra, o.nombreObra, a.nombreArtistico, c.nombreCategoria, u.nombreLugar,
                      b.nombreBarrio, cm.numeroComuna, o.estadoConservacion
             ORDER BY o.nombreObra";

        public DataTable ConsultaObra(string idObra) =>
            Ejecutar(ObraConRelaciones + " WHERE o.idObra = @p" + AgruparObra, ("@p", idObra));

        public DataTable ObrasPorArtista(string idArtista) =>
            Ejecutar(ObraConRelaciones + " WHERE o.idArtistaFK = @p" + AgruparObra, ("@p", idArtista));

        public DataTable ObrasPorCategoria(string idCategoria) =>
            Ejecutar(ObraConRelaciones + " WHERE o.idCategoriaFK = @p" + AgruparObra, ("@p", idCategoria));

        // Busca por sector/lugar, barrio o número de comuna.
        public DataTable ObrasPorUbicacion(string texto) =>
            Ejecutar(ObraConRelaciones + @"
                WHERE u.nombreLugar LIKE @like OR b.nombreBarrio LIKE @like
                   OR CAST(cm.numeroComuna AS CHAR) = @exacto" + AgruparObra,
                ("@like", "%" + texto + "%"), ("@exacto", texto));

        // Usa la vista vw_ruta_detalle que ya existe en la base de datos.
        public DataTable ConsultaRuta(string idRuta) =>
            Ejecutar(@"
                SELECT nombreRuta AS Ruta, ordenVisita AS Orden, nombreObra AS Obra,
                       nombreLugar AS Lugar, direccion AS Direccion, nombreBarrio AS Barrio,
                       numeroComuna AS Comuna, tiempoPermanencia AS MinutosPermanencia,
                       recomendacion AS Recomendacion
                  FROM vw_ruta_detalle
                 WHERE idRuta = @p
                 ORDER BY ordenVisita", ("@p", idRuta));

        public DataTable RecursosDeObra(string idObra) =>
            Ejecutar(@"
                SELECT o.nombreObra AS Obra, r.nombreRecurso AS Recurso, t.nombreTipoRecurso AS Tipo,
                       f.extension AS Formato, r.rutaArchivo AS Ruta, r.tamanio AS TamanoKB,
                       r.duracion AS DuracionSeg
                  FROM recursos_multimedias r
                  JOIN formatos_recursos f ON f.idFormato = r.idFormatoFK
                  JOIN tipos_recursos t    ON t.idTipoRecurso = f.idTipoRecursoFK
                  JOIN obras o             ON o.idObra = r.idObraFK
                 WHERE o.idObra = @p
                 ORDER BY t.nombreTipoRecurso, r.nombreRecurso", ("@p", idObra));

        // ---------- Estadísticas (agregaciones) ----------

        public DataTable ObrasPorComuna() =>
            Ejecutar(@"
                SELECT cm.numeroComuna AS Comuna, COUNT(o.idObra) AS TotalObras
                  FROM comunas cm
                  JOIN barrios b      ON b.idComunaFK = cm.idComuna
                  JOIN ubicaciones u  ON u.idBarrioFK = b.idBarrio
                  JOIN obras o        ON o.idUbicacionFK = u.idUbicacion
                 GROUP BY cm.numeroComuna
                 ORDER BY TotalObras DESC, cm.numeroComuna");

        public DataTable ObrasPorCategoria() =>
            Ejecutar(@"
                SELECT c.nombreCategoria AS Categoria, COUNT(o.idObra) AS TotalObras
                  FROM categorias c
                  LEFT JOIN obras o ON o.idCategoriaFK = c.idCategoria
                 GROUP BY c.idCategoria, c.nombreCategoria
                 ORDER BY TotalObras DESC, c.nombreCategoria");

        public DataTable ArtistasConMasDe(int n) =>
            Ejecutar(@"
                SELECT a.nombreArtistico AS Artista, COUNT(o.idObra) AS TotalObras
                  FROM artistas a
                  JOIN obras o ON o.idArtistaFK = a.idArtista
                 GROUP BY a.idArtista, a.nombreArtistico
                HAVING COUNT(o.idObra) > @n
                 ORDER BY TotalObras DESC, a.nombreArtistico", ("@n", n));

        public DataTable RecursosPorTipo() =>
            Ejecutar(@"
                SELECT t.nombreTipoRecurso AS Tipo, COUNT(r.idRecurso) AS TotalRecursos,
                       ROUND(COALESCE(SUM(r.tamanio), 0) / 1024, 2) AS TotalMB
                  FROM tipos_recursos t
                  LEFT JOIN formatos_recursos f    ON f.idTipoRecursoFK = t.idTipoRecurso
                  LEFT JOIN recursos_multimedias r ON r.idFormatoFK = f.idFormato
                 GROUP BY t.idTipoRecurso, t.nombreTipoRecurso
                 ORDER BY TotalRecursos DESC, t.nombreTipoRecurso");
    }
}
