using System.Data;
using ArteUrbanoCali.Datos;
using ArteUrbanoCali.Entidades;

namespace ArteUrbanoCali.Negocio
{
    public enum Parametro { Ninguno, Obra, Artista, Categoria, Ruta, Texto, Numero }

    public class ConsultaNegocio
    {
        private readonly ConsultaDatos _datos = new ConsultaDatos();
        private readonly ObraDatos _obras = new ObraDatos();
        private readonly RutaDatos _rutas = new RutaDatos();

        /// <summary>Consultas disponibles: nombre, tipo de parámetro que piden.</summary>
        public static readonly (string Nombre, Parametro Parametro)[] Catalogo =
        {
            ("Consulta de obra (artista, categoría, ubicación y recursos)", Parametro.Obra),
            ("Obras por artista",                                          Parametro.Artista),
            ("Obras por categoría",                                        Parametro.Categoria),
            ("Obras por ubicación (sector, barrio o comuna)",              Parametro.Texto),
            ("Consulta de ruta (obras, orden y ubicación)",                Parametro.Ruta),
            ("Recursos multimedia de una obra",                            Parametro.Obra),
            ("Estadística: obras por comuna",                              Parametro.Ninguno),
            ("Estadística: obras por categoría",                           Parametro.Ninguno),
            ("Estadística: artistas con más de N obras",                   Parametro.Numero),
            ("Estadística: recursos multimedia por tipo",                  Parametro.Ninguno)
        };

        public List<ItemCombo> ListarItems(Parametro p) => p switch
        {
            Parametro.Obra => _rutas.ListarObrasCombo(),
            Parametro.Artista => _obras.ListarArtistasCombo(),
            Parametro.Categoria => _obras.ListarCategoriasCombo(),
            Parametro.Ruta => _rutas.ListarRutasCombo(),
            _ => new List<ItemCombo>()
        };

        public DataTable Ejecutar(int indice, string valor)
        {
            switch (indice)
            {
                case 0: return _datos.ConsultaObra(Exigir(valor, "obra"));
                case 1: return _datos.ObrasPorArtista(Exigir(valor, "artista"));
                case 2: return _datos.ObrasPorCategoria(Exigir(valor, "categoría"));
                case 3: return _datos.ObrasPorUbicacion(Validador.Obligatorio(valor, "sector, barrio o comuna", 100));
                case 4: return _datos.ConsultaRuta(Exigir(valor, "ruta"));
                case 5: return _datos.RecursosDeObra(Exigir(valor, "obra"));
                case 6: return _datos.ObrasPorComuna();
                case 7: return _datos.ObrasPorCategoria();
                case 8:
                    if (!int.TryParse(valor, out int n) || n < 0)
                        throw new ValidacionException("Ingrese un número entero mayor o igual a cero.");
                    return _datos.ArtistasConMasDe(n);
                case 9: return _datos.RecursosPorTipo();
                default: throw new ValidacionException("Seleccione una consulta.");
            }
        }

        private static string Exigir(string valor, string que)
        {
            if (string.IsNullOrEmpty(valor))
                throw new ValidacionException($"Seleccione {que}.");
            return valor;
        }
    }
}
