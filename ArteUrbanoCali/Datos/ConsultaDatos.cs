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
                   COALESCE(GROUP_CONCAT(CONCAT(r.tipoRecurso, ': ', r.nombreRecurso) ORDER BY r.tipoRecurso SEPARATOR ' | '), '') AS DetalleRecursos
              FROM OBRAS o
              JOIN ARTISTAS a    ON a.idArtista   = o.idArtistaFK
              JOIN CATEGORIAS c  ON c.idCategoria = o.idCategoriaFK
              JOIN UBICACIONES u ON u.idUbicacion = o.idUbicacionFK
              JOIN BARRIOS b     ON b.idBarrio    = u.idBarrioFK
              JOIN COMUNAS cm    ON cm.idComuna   = b.idComunaFK
              LEFT JOIN RECURSOS_MULTIMEDIAS r ON r.idObraFK = o.idObra";

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

        public DataTable ConsultaRuta(string idRuta) =>
            Ejecutar(@"
                SELECT ru.nombreRuta AS Ruta, ro.ordenVisita AS Orden, o.nombreObra AS Obra,
                       u.nombreLugar AS Lugar, u.direccion AS Direccion, b.nombreBarrio AS Barrio,
                       cm.numeroComuna AS Comuna, ro.tiempoPermanenciaMin AS MinutosPermanencia,
                       ro.recomendacion AS Recomendacion
                  FROM RUTAS ru
                  JOIN RUTAS_OBRAS ro ON ro.idRutaFK = ru.idRuta
                  JOIN OBRAS o        ON o.idObra = ro.idObraFK
                  JOIN UBICACIONES u  ON u.idUbicacion = o.idUbicacionFK
                  JOIN BARRIOS b      ON b.idBarrio = u.idBarrioFK
                  JOIN COMUNAS cm     ON cm.idComuna = b.idComunaFK
                 WHERE ru.idRuta = @p
                 ORDER BY ro.ordenVisita", ("@p", idRuta));

        public DataTable RecursosDeObra(string idObra) =>
            Ejecutar(@"
                SELECT o.nombreObra AS Obra, r.nombreRecurso AS Recurso, r.tipoRecurso AS Tipo,
                       r.formatoRecurso AS Formato, r.rutaArchivo AS Ruta, r.tamanoBytes AS TamanoBytes,
                       r.duracionSegundos AS DuracionSegundos
                  FROM RECURSOS_MULTIMEDIAS r
                  JOIN OBRAS o ON o.idObra = r.idObraFK
                 WHERE o.idObra = @p
                 ORDER BY r.tipoRecurso, r.nombreRecurso", ("@p", idObra));

        // ---------- Estadísticas (agregaciones) ----------

        public DataTable ObrasPorComuna() =>
            Ejecutar(@"
                SELECT cm.numeroComuna AS Comuna, COUNT(o.idObra) AS TotalObras
                  FROM COMUNAS cm
                  JOIN BARRIOS b      ON b.idComunaFK = cm.idComuna
                  JOIN UBICACIONES u  ON u.idBarrioFK = b.idBarrio
                  JOIN OBRAS o        ON o.idUbicacionFK = u.idUbicacion
                 GROUP BY cm.numeroComuna
                 ORDER BY TotalObras DESC, cm.numeroComuna");

        public DataTable ObrasPorCategoria() =>
            Ejecutar(@"
                SELECT c.nombreCategoria AS Categoria, COUNT(o.idObra) AS TotalObras
                  FROM CATEGORIAS c
                  LEFT JOIN OBRAS o ON o.idCategoriaFK = c.idCategoria
                 GROUP BY c.idCategoria, c.nombreCategoria
                 ORDER BY TotalObras DESC, c.nombreCategoria");

        public DataTable ArtistasConMasDe(int n) =>
            Ejecutar(@"
                SELECT a.nombreArtistico AS Artista, COUNT(o.idObra) AS TotalObras
                  FROM ARTISTAS a
                  JOIN OBRAS o ON o.idArtistaFK = a.idArtista
                 GROUP BY a.idArtista, a.nombreArtistico
                HAVING COUNT(o.idObra) > @n
                 ORDER BY TotalObras DESC, a.nombreArtistico", ("@n", n));

        public DataTable RecursosPorTipo() =>
            Ejecutar(@"
                SELECT r.tipoRecurso AS Tipo, COUNT(*) AS TotalRecursos,
                       ROUND(SUM(r.tamanoBytes) / 1048576, 2) AS TotalMB
                  FROM RECURSOS_MULTIMEDIAS r
                 GROUP BY r.tipoRecurso
                 ORDER BY TotalRecursos DESC");
    }
}
