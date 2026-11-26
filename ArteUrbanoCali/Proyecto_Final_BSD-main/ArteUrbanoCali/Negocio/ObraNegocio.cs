using ArteUrbanoCali.Datos;
using ArteUrbanoCali.Entidades;

namespace ArteUrbanoCali.Negocio
{
    public class ObraNegocio
    {
        public static readonly string[] EstadosConservacion = { "bueno", "regular", "deteriorado" };

        private readonly ObraDatos _datos = new ObraDatos();

        public List<Obra> Listar() => _datos.Listar();
        public List<ItemCombo> ListarArtistas() => _datos.ListarArtistasCombo();
        public List<ItemCombo> ListarCategorias() => _datos.ListarCategoriasCombo();
        public List<ItemCombo> ListarUbicaciones() => _datos.ListarUbicacionesCombo();

        // Si IdObra viene vacío se registra una obra nueva; si trae valor, se modifica.
        public void Guardar(Obra o)
        {
            Validador.ExigirAdministrador();
            bool esNuevo = string.IsNullOrEmpty(o.IdObra);

            o.Nombre = Validador.Obligatorio(o.Nombre, "nombre de la obra", 100);
            o.Descripcion = Validador.Opcional(o.Descripcion, "descripción", 500);

            if (o.FechaCreacion.Date > DateTime.Today)
                throw new ValidacionException("La fecha de creación no puede ser futura.");

            if (!EstadosConservacion.Contains(o.EstadoConservacion))
                throw new ValidacionException("Seleccione el estado de conservación.");

            // Toda obra debe tener artista, categoría y ubicación.
            if (string.IsNullOrEmpty(o.IdArtista))
                throw new ValidacionException("Seleccione el artista.");
            if (string.IsNullOrEmpty(o.IdCategoria))
                throw new ValidacionException("Seleccione la categoría.");
            if (string.IsNullOrEmpty(o.IdUbicacion))
                throw new ValidacionException("Seleccione la ubicación.");

            if (_datos.ExisteDuplicado(o.Nombre, o.IdArtista, esNuevo ? null : o.IdObra))
                throw new ValidacionException("Este artista ya tiene una obra con ese nombre.");

            if (esNuevo)
            {
                // Una obra nueva solo puede asignarse a artistas y categorías activos.
                if (!_datos.ArtistaActivo(o.IdArtista))
                    throw new ValidacionException("El artista seleccionado está inactivo.");
                if (!_datos.CategoriaActiva(o.IdCategoria))
                    throw new ValidacionException("La categoría seleccionada está inactiva.");

                o.IdObra = Guid.NewGuid().ToString();
                o.IdUsuario = Sesion.UsuarioActual!.IdUsuario;   // quién registró la obra
                _datos.Insertar(o);
            }
            else
            {
                _datos.Actualizar(o);
            }
        }
    }
}
