using ArteUrbanoCali.Datos;
using ArteUrbanoCali.Entidades;
using System.Diagnostics;

namespace ArteUrbanoCali.Negocio
{
    public class RecursoNegocio
    {
        private readonly RecursoDatos _datos = new RecursoDatos();

        public List<RecursoMultimedia> Listar(string? idObra) => _datos.Listar(idObra);

        public List<ItemCombo> ListarObras() => new RutaDatos().ListarObrasCombo();

        // ---- Catálogo de tipos y formatos (viene de la base de datos) ----

        public List<FormatoRecurso> ListarFormatos() => _datos.ListarFormatos();

        public List<string> ListarTipos() =>
            _datos.ListarFormatos().Select(f => f.Tipo).Distinct().ToList();

        /// <summary>Filtro del cuadro de diálogo con las extensiones que la base permite para ese tipo.</summary>
        public string FiltroDialogo(string tipo) =>
            ArchivosMultimedia.FiltroDialogo(_datos.ListarFormatos().Where(f => f.Tipo == tipo).Select(f => f.Extension));

        /// <summary>
        /// Registra un recurso nuevo (origen = archivo o carpeta AR elegido) o modifica uno existente (origen = null).
        /// </summary>
        public void Guardar(RecursoMultimedia r, string? origen)
        {
            Validador.ExigirAdministrador();
            bool esNuevo = string.IsNullOrEmpty(r.IdRecurso);

            r.Nombre = Validador.Obligatorio(r.Nombre, "nombre del recurso", 100);
            r.Descripcion = Validador.Opcional(r.Descripcion, "descripción", 255);
            if (r.DuracionSegundos is <= 0)
                throw new ValidacionException("La duración debe ser mayor a cero (o dejarse vacía).");

            if (!esNuevo)
            {
                _datos.Actualizar(r);
                return;
            }

            if (string.IsNullOrEmpty(r.IdObra) || !_datos.ExisteObra(r.IdObra))
                throw new ValidacionException("Seleccione una obra existente para asociar el recurso.");

            var formatosDelTipo = _datos.ListarFormatos().Where(f => f.Tipo == r.Tipo).ToList();
            if (formatosDelTipo.Count == 0)
                throw new ValidacionException("Seleccione el tipo de recurso.");
            if (string.IsNullOrWhiteSpace(origen))
                throw new ValidacionException("Seleccione el archivo del recurso.");

            FormatoRecurso formato;
            if (r.Tipo == "AR")
            {
                // La realidad aumentada se registra como referencia a una carpeta.
                if (!Directory.Exists(origen))
                    throw new ValidacionException("La carpeta de realidad aumentada no existe.");
                formato = formatosDelTipo[0];
                r.RutaArchivo = ArchivosMultimedia.ImportarCarpetaAR(origen, formato.Carpeta);
            }
            else
            {
                if (!File.Exists(origen))
                    throw new ValidacionException("El archivo seleccionado no existe.");

                string ext = ArchivosMultimedia.FormatoDeArchivo(origen);
                FormatoRecurso? encontrado = formatosDelTipo.FirstOrDefault(f => f.Extension == ext);
                if (encontrado == null)
                    throw new ValidacionException(
                        $"El formato {ext} no es válido para {r.Tipo}. Permitidos: " +
                        string.Join(", ", formatosDelTipo.Select(f => f.Extension)) + ".");

                if (new FileInfo(origen).Length > ArchivosMultimedia.TamanoMaximoBytes)
                    throw new ValidacionException("El archivo supera el tamaño máximo permitido (200 MB).");

                formato = encontrado;
                r.RutaArchivo = ArchivosMultimedia.ImportarArchivo(origen, formato.Carpeta);
            }

            if (_datos.ExisteRuta(r.RutaArchivo, null))
                throw new ValidacionException("Ese archivo ya está registrado como recurso multimedia.");

            r.IdFormato = formato.IdFormato;
            r.Formato = formato.Extension;
            r.TamanoKb = ArchivosMultimedia.TamanoKb(r.RutaArchivo);
            r.IdRecurso = Guid.NewGuid().ToString();
            _datos.Insertar(r);
        }

        /// <summary>Quita el registro de la base de datos. El archivo físico se conserva en la carpeta.</summary>
        public void Eliminar(string idRecurso)
        {
            Validador.ExigirAdministrador();
            _datos.Eliminar(idRecurso);
        }

        /// <summary>Abre el recurso con el programa predeterminado (reproductor, visor 3D o carpeta AR).</summary>
        public void Abrir(RecursoMultimedia r)
        {
            if (!ArchivosMultimedia.Existe(r.RutaArchivo))
                throw new ValidacionException("El archivo no se encuentra en RecursosMultimedia: " + r.RutaArchivo);

            Process.Start(new ProcessStartInfo(ArchivosMultimedia.ResolverRuta(r.RutaArchivo)) { UseShellExecute = true });
        }

        /// <summary>Carga una imagen para vista previa sin bloquear el archivo. Devuelve null si no se puede mostrar.</summary>
        public Image? CargarImagen(RecursoMultimedia r)
        {
            if (r.Tipo != "IMAGEN" || (r.Formato != "JPG" && r.Formato != "PNG")) return null;
            if (!ArchivosMultimedia.Existe(r.RutaArchivo)) return null;
            try
            {
                using var fs = File.OpenRead(ArchivosMultimedia.ResolverRuta(r.RutaArchivo));
                using var ms = new MemoryStream();
                fs.CopyTo(ms);
                ms.Position = 0;
                using var original = Image.FromStream(ms);
                return new Bitmap(original);   // copia independiente del stream
            }
            catch
            {
                return null;
            }
        }
    }
}
