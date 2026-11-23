using ArteUrbanoCali.Entidades;
using MySqlConnector;

namespace ArteUrbanoCali.Datos
{
    /// <summary>Listas de apoyo para los combos: países y barrios.</summary>
    public class CatalogoDatos
    {
        public List<Pais> ListarPaises()
        {
            const string sql = "SELECT idPais, nombrePais FROM PAISES ORDER BY nombrePais";
            var lista = new List<Pais>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(new Pais { IdPais = rd.GetString(0), Nombre = rd.GetString(1) });
            return lista;
        }

        public List<Barrio> ListarBarrios()
        {
            const string sql = @"
                SELECT b.idBarrio, b.nombreBarrio, c.numeroComuna
                  FROM BARRIOS b
                  JOIN COMUNAS c ON c.idComuna = b.idComunaFK
                 ORDER BY c.numeroComuna, b.nombreBarrio";
            var lista = new List<Barrio>();
            using var cn = Conexion.Crear();
            cn.Open();
            using var cmd = new MySqlCommand(sql, cn);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                lista.Add(new Barrio { IdBarrio = rd.GetString(0), Nombre = rd.GetString(1), NumeroComuna = rd.GetInt32(2) });
            return lista;
        }
    }
}
