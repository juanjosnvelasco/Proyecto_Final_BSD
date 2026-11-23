using ArteUrbanoCali.Datos;
using ArteUrbanoCali.Entidades;

namespace ArteUrbanoCali.Negocio
{
    public class CategoriaNegocio
    {
        private readonly CategoriaDatos _datos = new CategoriaDatos();

        public List<Categoria> Listar() => _datos.Listar();

        public void Guardar(Categoria c)
        {
            Validador.ExigirAdministrador();
            bool esNuevo = string.IsNullOrEmpty(c.IdCategoria);

            c.Nombre = Validador.Obligatorio(c.Nombre, "nombre", 60);
            c.Descripcion = Validador.Opcional(c.Descripcion, "descripción", 255);

            if (_datos.ExisteNombre(c.Nombre, esNuevo ? null : c.IdCategoria))
                throw new ValidacionException("Ya existe una categoría con ese nombre.");

            if (esNuevo)
            {
                c.IdCategoria = Guid.NewGuid().ToString();
                c.Estado = true;
                _datos.Insertar(c);
            }
            else
            {
                _datos.Actualizar(c);
            }
        }

        public void CambiarEstado(string idCategoria, bool estado)
        {
            Validador.ExigirAdministrador();
            _datos.CambiarEstado(idCategoria, estado);
        }
    }
}
