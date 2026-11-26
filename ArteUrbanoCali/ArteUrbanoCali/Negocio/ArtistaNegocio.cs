using ArteUrbanoCali.Datos;
using ArteUrbanoCali.Entidades;

namespace ArteUrbanoCali.Negocio
{
    public class ArtistaNegocio
    {
        private readonly ArtistaDatos _datos = new ArtistaDatos();
        private readonly CatalogoDatos _catalogo = new CatalogoDatos();

        public List<Artista> Listar() => _datos.Listar();
        public List<Pais> ListarPaises() => _catalogo.ListarPaises();

        
        public void Guardar(Artista a)
        {
            Validador.ExigirAdministrador();
            bool esNuevo = string.IsNullOrEmpty(a.IdArtista);

            a.NombreArtistico = Validador.Obligatorio(a.NombreArtistico, "nombre artístico", 100);
            a.Nombre1Real = Validador.Obligatorio(a.Nombre1Real, "primer nombre", 50);
            a.Nombre2Real = Validador.Opcional(a.Nombre2Real, "segundo nombre", 50);
            a.Apellido1Real = Validador.Obligatorio(a.Apellido1Real, "primer apellido", 50);
            a.Apellido2Real = Validador.Opcional(a.Apellido2Real, "segundo apellido", 50);
            a.PerfilRedSocial = Validador.Opcional(a.PerfilRedSocial, "perfil o red social", 255);
            a.Biografia = Validador.Opcional(a.Biografia, "biografía", 5000);

            if (string.IsNullOrEmpty(a.IdPais))
                throw new ValidacionException("Seleccione el país de origen.");

            if (_datos.ExisteNombre(a.NombreArtistico, esNuevo ? null : a.IdArtista))
                throw new ValidacionException("Ya existe un artista con ese nombre artístico.");

            if (esNuevo)
            {
                a.IdArtista = Guid.NewGuid().ToString();
                a.Estado = true;
                _datos.Insertar(a);
            }
            else
            {
                _datos.Actualizar(a);
            }
        }

        
        public void CambiarEstado(string idArtista, bool estado)
        {
            Validador.ExigirAdministrador();
            _datos.CambiarEstado(idArtista, estado);
        }
    }
}
