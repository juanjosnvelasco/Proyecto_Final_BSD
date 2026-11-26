using ArteUrbanoCali.Datos;
using ArteUrbanoCali.Entidades;
using System.Diagnostics;

namespace ArteUrbanoCali.Negocio
{
    public class RecursoNegocio
    {
        private readonly RecursoDatos _datos = new RecursoDatos();
        private readonly ObraDatos _obras = new ObraDatos();

        public List<RecursoMultimedia> Listar(string? idObra) => _datos.Listar(idObra);

        public List<ItemCombo> ListarObras() => new RutaDatos().ListarObrasCombo();

        /// <summary>
        /// Registra un recurso nuevo (origen = archivo o carpeta AR elegido) o modifica uno existente (origen = null).
        /// </summary>
        public void Guardar(RecursoMultimedia r, string? origen)
        {
            Validador.ExigirAdministrador();
            bool esNuevo = string.IsNullOrEmpty(r.IdRecurso);

            r.Nombre = Validador.Obligatorio(r.Nombre, "nombre del recurso", 100);
            r.Descripcion = Validador.Opcional(r.Descripcion, "descripción", 300);
            if (r.DuracionSegundos is < 0)
                throw new ValidacionException("La duración no puede ser negativa.");

            if (!esNuevo)
            {
                _datos.Actualizar(r);
                return;
            }

            if (string.IsNullOrEmpty(r.IdObra) || !_datos.ExisteObra(r.IdObra))
                throw new ValidacionException("Seleccione una obra existente para asociar el recurso.");
            if (!ArchivosMultimedia.FormatosPorTipo.ContainsKey(r.Tipo))
                throw new ValidacionException("Seleccione el tipo de recurso.");
            if (string.IsNullOrWhiteSpace(origen))
                throw new ValidacionException("Seleccione el archivo del recurso.");

            if (r.Tipo == "AR")
            {
                if (!Directory.Exists(origen))
                    throw new ValidacionException("La carpeta de realidad aumentada no existe.");
                r.Formato = "AR";
                r.RutaArchivo = ArchivosMultimedia.ImportarCarpetaAR(origen);
            }
            else
            {
                if (!File.Exists(origen))
                    throw new ValidacionException("El archivo seleccionado no existe.");

                string formato = ArchivosMultimedia.FormatoDeArchivo(origen);
                if (!ArchivosMultimedia.FormatosPorTipo[r.Tipo].Contains(formato))
                    throw new ValidacionException(
                        $"El formato {formato} no es válido para {r.Tipo}. Permitidos: " +
                        string.Join(", ", ArchivosMultimedia.FormatosPorTipo[r.Tipo]) + ".");

                if (new FileInfo(origen).Length > ArchivosMultimedia.TamanoMaximoBytes)
                    throw new ValidacionException("El archivo supera el tamaño máximo permitido (200 MB).");

                r.Formato = formato;
                r.RutaArchivo = ArchivosMultimedia.ImportarArchivo(origen, r.Tipo);
            }

            if (_datos.ExisteRuta(r.RutaArchivo, null))
                throw new ValidacionException("Ese archivo ya está registrado como recurso multimedia.");

            r.TamanoBytes = ArchivosMultimedia.Tamano(r.RutaArchivo);
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

        /// <summary>Carga una imagen para vista previa sin bloquear el archivo. Devuelve null si no se puede mostrar (ej. AVIF).</summary>
        public Image? CargarImagen(RecursoMultimedia r)
        {
            if (r.Tipo != "IMAGEN" || r.Formato == "AVIF") return null;
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
