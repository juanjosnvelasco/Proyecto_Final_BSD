namespace ArteUrbanoCali.Negocio
{
    /// <summary>
    /// Manejo de la carpeta RecursosMultimedia. En la base de datos solo se guarda la ruta RELATIVA
    /// (ej. "Imagenes/arte1.jpg"); aquí se convierte a ruta física y se copian los archivos a la carpeta.
    /// </summary>
    public static class ArchivosMultimedia
    {
        public const string NombreCarpeta = "RecursosMultimedia";

        public static readonly string[] Tipos = { "IMAGEN", "VIDEO", "AUDIO", "MODELO3D", "AR" };

        public static readonly Dictionary<string, string[]> FormatosPorTipo = new()
        {
            ["IMAGEN"] = new[] { "JPG", "PNG", "AVIF" },
            ["VIDEO"] = new[] { "MP4" },
            ["AUDIO"] = new[] { "MP3" },
            ["MODELO3D"] = new[] { "GLB" },
            ["AR"] = new[] { "AR" }
        };

        public static readonly Dictionary<string, string> SubcarpetaPorTipo = new()
        {
            ["IMAGEN"] = "Imagenes",
            ["VIDEO"] = "Videos",
            ["AUDIO"] = "Audios",
            ["MODELO3D"] = "Modelos3D",
            ["AR"] = "AR"
        };

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

        /// <summary>Formato (en mayúsculas) según la extensión; JPEG se normaliza a JPG.</summary>
        public static string FormatoDeArchivo(string ruta)
        {
            string ext = Path.GetExtension(ruta).TrimStart('.').ToUpperInvariant();
            return ext == "JPEG" ? "JPG" : ext;
        }

        public static string FiltroDialogo(string tipo) => tipo switch
        {
            "IMAGEN" => "Imágenes (*.jpg;*.jpeg;*.png;*.avif)|*.jpg;*.jpeg;*.png;*.avif",
            "VIDEO" => "Videos (*.mp4)|*.mp4",
            "AUDIO" => "Audios (*.mp3)|*.mp3",
            "MODELO3D" => "Modelos 3D (*.glb)|*.glb",
            _ => "Todos los archivos (*.*)|*.*"
        };

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
        public static string ImportarArchivo(string origen, string tipo)
        {
            string baseFull = Path.GetFullPath(CarpetaBase);
            string origenFull = Path.GetFullPath(origen);

            // Si ya está dentro de RecursosMultimedia no se copia.
            if (origenFull.StartsWith(baseFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return Path.GetRelativePath(baseFull, origenFull).Replace('\\', '/');

            string carpeta = Path.Combine(baseFull, SubcarpetaPorTipo[tipo]);
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

        /// <summary>Para AR: copia la carpeta completa dentro de RecursosMultimedia/AR y devuelve la ruta relativa.</summary>
        public static string ImportarCarpetaAR(string origen)
        {
            string baseFull = Path.GetFullPath(CarpetaBase);
            string origenFull = Path.GetFullPath(origen.TrimEnd(Path.DirectorySeparatorChar));
            if (origenFull.StartsWith(baseFull + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return Path.GetRelativePath(baseFull, origenFull).Replace('\\', '/');

            string destinoRaiz = Path.Combine(baseFull, SubcarpetaPorTipo["AR"]);
            Directory.CreateDirectory(destinoRaiz);
            string destino = Path.Combine(destinoRaiz, new DirectoryInfo(origenFull).Name);
            int n = 1;
            string baseDestino = destino;
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

        public static long Tamano(string relativa)
        {
            string fisica = ResolverRuta(relativa);
            if (File.Exists(fisica)) return new FileInfo(fisica).Length;
            if (Directory.Exists(fisica))
                return new DirectoryInfo(fisica).EnumerateFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);
            return 0;
        }
    }
}
