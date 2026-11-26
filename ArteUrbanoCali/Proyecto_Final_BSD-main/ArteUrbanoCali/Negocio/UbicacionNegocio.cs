using ArteUrbanoCali.Datos;
using ArteUrbanoCali.Entidades;

namespace ArteUrbanoCali.Negocio
{
    public class UbicacionNegocio
    {
        private readonly UbicacionDatos _datos = new UbicacionDatos();
        private readonly CatalogoDatos _catalogo = new CatalogoDatos();

        public List<Ubicacion> Listar() => _datos.Listar();
        public List<Barrio> ListarBarrios() => _catalogo.ListarBarrios();

        // La latitud y la longitud llegan como texto desde el formulario y se validan aquí.
        public void Guardar(Ubicacion u, string latitudTexto, string longitudTexto)
        {
            Validador.ExigirAdministrador();
            bool esNuevo = string.IsNullOrEmpty(u.IdUbicacion);

            u.NombreLugar = Validador.Obligatorio(u.NombreLugar, "nombre del lugar", 100);
            u.Direccion = Validador.Obligatorio(u.Direccion, "dirección", 150);
            u.Latitud = Validador.Coordenada(latitudTexto, "latitud", -90, 90);
            u.Longitud = Validador.Coordenada(longitudTexto, "longitud", -180, 180);

            if (string.IsNullOrEmpty(u.IdBarrio))
                throw new ValidacionException("Seleccione el barrio.");

            if (esNuevo)
            {
                u.IdUbicacion = Guid.NewGuid().ToString();
                _datos.Insertar(u);
            }
            else
            {
                _datos.Actualizar(u);
            }
        }
    }
}
