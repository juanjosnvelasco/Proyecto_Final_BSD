namespace ArteUrbanoCali.Negocio
{
    /// <summary>
    /// Manejo de la carpeta RecursosMultimedia. En la base de datos solo se guarda la ruta RELATIVA
    /// (ej. "Imagenes/arte1.jpg"); aquí se convierte a ruta física y se copian los archivos a la carpeta.
    /// Los tipos, formatos y subcarpetas permitidos vienen de la base (TIPOS_RECURSOS / FORMATOS_RECURSOS).
    /// </summary>
    public static class ArchivosMultimedia
    {
        public const string NombreCarpeta = "RecursosMultimedia";
        public const long TamanoMaximoBytes = 200L * 1024 * 1024;

        private static string? _base;

        /// <summary>Busca RecursosMultimedia junto al ejecutable o subiendo por las carpetas del proyecto.</summary>
        public static string CarpetaBase
        {
            get
            {
                if (_base != null) return _base;
                var dir = new DirectoryInfo(AppContext.BaseDirectory);
                while (dir != null)
                {
                    string candidata = Path.Combine(dir.FullName, NombreCarpeta);
                    if (Directory.Exists(candidata)) return _base = candidata;
                    dir = dir.Parent;
                }
                _base = Path.Combine(AppContext.BaseDirectory, NombreCarpeta);
                Directory.CreateDirectory(_base);
                return _base;
            }
        }

        /// <summary>Extensión en mayúsculas; JPEG se normaliza a JPG.</summary>
        public static string FormatoDeArchivo(string ruta)
        {
            string ext = Path.GetExtension(ruta).TrimStart('.').ToUpperInvariant();
            return ext == "JPEG" ? "JPG" : ext;
        }

        /// <summary>Filtro para el cuadro de diálogo a partir de las extensiones permitidas.</summary>
        public static string FiltroDialogo(IEnumerable<string> extensiones)
        {
            var patrones = extensiones
                .SelectMany(e => e == "JPG" ? new[] { "*.jpg", "*.jpeg" } : new[] { "*." + e.ToLowerInvariant() })
                .ToList();
            if (patrones.Count == 0) return "Todos los archivos (*.*)|*.*";
            string lista = string.Join(";", patrones);
            return $"Archivos permitidos ({lista})|{lista}";
        }

        /// <summary>Convierte una ruta relativa de la BD a ruta física, impidiendo salir de RecursosMultimedia.</summary>
        public static string ResolverRuta(string relativa)
        {
            string baseFull = Path.GetFullPath(CarpetaBase);
            string completa = Path.GetFullPath(Path.Combine(baseFull, relativa.Replace('/', Path.DirectorySeparatorChar)));
            if (!completa.StartsWith(baseFull, StringComparison.OrdinalIgnoreCase))
                throw new ValidacionException("La ruta del recurso no es válida.");
            return completa;
        }

        public static bool Existe(string relativa)
        {
            string fisica = ResolverRuta(relativa);
            return File.Exists(fisica) || Directory.Exists(fisica);
        }

        /// <summary>Copia el archivo a su subcarpeta (si no está ya dentro) y devuelve la ruta relativa con "/".</summary>
        public static string ImportarArchivo(string origen, string carpetaTipo)
        {
            string baseFull = Path.GetFullPath(CarpetaBase);
            string origenFull = Path.GetFullPath(origen);

            // Si ya está dentro de RecursosMultimedia no se copia.
            if (origenFull.StartsWith(baseFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return Path.GetRelativePath(baseFull, origenFull).Replace('\\', '/');

            string carpeta = Path.Combine(baseFull, carpetaTipo.Trim('/', '\\'));
            Directory.CreateDirectory(carpeta);
            string destino = Path.Combine(carpeta, Path.GetFileName(origenFull));
            int n = 1;
            while (File.Exists(destino))
            {
                destino = Path.Combine(carpeta, $"{Path.GetFileNameWithoutExtension(origenFull)}_{n++}{Path.GetExtension(origenFull)}");
            }
            File.Copy(origenFull, destino);
            return Path.GetRelativePath(baseFull, destino).Replace('\\', '/');
        }

        /// <summary>Para AR: copia la carpeta completa dentro de la carpeta del tipo y devuelve la ruta relativa.</summary>
        public static string ImportarCarpetaAR(string origen, string carpetaTipo)
        {
            string baseFull = Path.GetFullPath(CarpetaBase);
            string origenFull = Path.GetFullPath(origen.TrimEnd(Path.DirectorySeparatorChar));
            if (origenFull.StartsWith(baseFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return Path.GetRelativePath(baseFull, origenFull).Replace('\\', '/');

            string destinoRaiz = Path.Combine(baseFull, carpetaTipo.Trim('/', '\\'));
            Directory.CreateDirectory(destinoRaiz);
            string baseDestino = Path.Combine(destinoRaiz, new DirectoryInfo(origenFull).Name);
            string destino = baseDestino;
            int n = 1;
            while (Directory.Exists(destino)) destino = baseDestino + "_" + n++;
            CopiarCarpeta(origenFull, destino);
            return Path.GetRelativePath(baseFull, destino).Replace('\\', '/');
        }

        private static void CopiarCarpeta(string origen, string destino)
        {
            Directory.CreateDirectory(destino);
            foreach (string f in Directory.GetFiles(origen))
                File.Copy(f, Path.Combine(destino, Path.GetFileName(f)));
            foreach (string d in Directory.GetDirectories(origen))
                CopiarCarpeta(d, Path.Combine(destino, new DirectoryInfo(d).Name));
        }

        /// <summary>Tamaño en KB (la base guarda el tamaño en KB).</summary>
        public static decimal TamanoKb(string relativa)
        {
            string fisica = ResolverRuta(relativa);
            long bytes = 0;
            if (File.Exists(fisica)) bytes = new FileInfo(fisica).Length;
            else if (Directory.Exists(fisica))
                bytes = new DirectoryInfo(fisica).EnumerateFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
            return Math.Round(bytes / 1024m, 2);
        }
    }
}
