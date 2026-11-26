using ArteUrbanoCali.Datos;
using ArteUrbanoCali.Entidades;

namespace ArteUrbanoCali.Negocio
{
    public class RutaNegocio
    {
        public const int MinimoObrasRutaActiva = 2;

        private readonly RutaDatos _datos = new RutaDatos();

        public List<Ruta> Listar() => _datos.Listar();
        public List<ItemCombo> ListarObras() => _datos.ListarObrasCombo();
        public List<RutaObra> ListarObras(string idRuta) => _datos.ListarObras(idRuta);
        public int SiguienteOrden(string idRuta) => _datos.SiguienteOrden(idRuta);

        public void Guardar(Ruta r)
        {
            Validador.ExigirAdministrador();
            bool esNueva = string.IsNullOrEmpty(r.IdRuta);

            r.Nombre = Validador.Obligatorio(r.Nombre, "nombre de la ruta", 100);
            r.Descripcion = Validador.Opcional(r.Descripcion, "descripción", 500);
            if (r.DuracionMinutos <= 0)
                throw new ValidacionException("La duración estimada debe ser mayor a cero.");
            if (r.DistanciaKm <= 0)
                throw new ValidacionException("La distancia estimada debe ser mayor a cero.");
            if (_datos.ExisteNombre(r.Nombre, esNueva ? null : r.IdRuta))
                throw new ValidacionException("Ya existe una ruta con ese nombre.");

            if (esNueva)
            {
                // Una ruta debe contener varias obras: nace inactiva hasta que se le agreguen.
                r.IdRuta = Guid.NewGuid().ToString();
                r.Estado = false;
                _datos.Insertar(r);
            }
            else
            {
                if (r.Estado && _datos.ContarObras(r.IdRuta) < MinimoObrasRutaActiva)
                    throw new ValidacionException($"Una ruta activa debe tener al menos {MinimoObrasRutaActiva} obras.");
                _datos.Actualizar(r);
            }
        }

        public void AgregarObra(string idRuta, string idObra, int orden, string? recomendacion, int tiempoMinutos)
        {
            Validador.ExigirAdministrador();
            if (string.IsNullOrEmpty(idRuta)) throw new ValidacionException("Seleccione o guarde primero la ruta.");
            if (string.IsNullOrEmpty(idObra)) throw new ValidacionException("Seleccione la obra.");
            if (orden <= 0) throw new ValidacionException("El orden de visita debe ser mayor a cero.");
            if (tiempoMinutos <= 0) throw new ValidacionException("El tiempo de permanencia debe ser mayor a cero.");

            string? rec = Validador.Opcional(recomendacion, "recomendación", 300);

            if (_datos.ObraEnRuta(idRuta, idObra))
                throw new ValidacionException("Esa obra ya está en la ruta.");
            if (_datos.OrdenUsado(idRuta, orden))
                throw new ValidacionException($"El orden {orden} ya está ocupado en esta ruta.");

            _datos.AgregarObra(new RutaObra
            {
                IdRutaObra = Guid.NewGuid().ToString(),
                IdRuta = idRuta,
                IdObra = idObra,
                Orden = orden,
                Recomendacion = rec,
                TiempoMinutos = tiempoMinutos
            });
        }

        public void RetirarObra(Ruta ruta, RutaObra obra)
        {
            Validador.ExigirAdministrador();
            if (ruta.Estado && _datos.ContarObras(ruta.IdRuta) - 1 < MinimoObrasRutaActiva)
                throw new ValidacionException(
                    $"La ruta está activa y debe conservar al menos {MinimoObrasRutaActiva} obras. Desactívela primero.");
            _datos.RetirarObra(obra.IdRutaObra);
        }

        /// <summary>Sube (-1) o baja (+1) una obra dentro del recorrido.</summary>
        public void Mover(List<RutaObra> obras, RutaObra obra, int direccion)
        {
            Validador.ExigirAdministrador();
            var ordenadas = obras.OrderBy(o => o.Orden).ToList();
            int indice = ordenadas.FindIndex(o => o.IdRutaObra == obra.IdRutaObra);
            int vecino = indice + direccion;
            if (indice < 0 || vecino < 0 || vecino >= ordenadas.Count) return;

            RutaObra otra = ordenadas[vecino];
            _datos.IntercambiarOrden(obra.IdRutaObra, obra.Orden, otra.IdRutaObra, otra.Orden);
        }
    }
}
