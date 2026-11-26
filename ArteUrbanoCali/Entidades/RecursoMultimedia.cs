namespace ArteUrbanoCali.Entidades
{
    /// <summary>Archivo digital asociado a una obra (tabla RECURSOS_MULTIMEDIAS). La ruta es relativa a RecursosMultimedia.</summary>
    public class RecursoMultimedia
    {
        public string IdRecurso { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Tipo { get; set; } = "";          // IMAGEN, VIDEO, AUDIO, MODELO3D, AR
        public string? Descripcion { get; set; }
        public string RutaArchivo { get; set; } = "";   // siempre relativa, con "/"
        public string Formato { get; set; } = "";       // JPG, PNG, AVIF, MP4, MP3, GLB, AR
        public long TamanoBytes { get; set; }
        public int? DuracionSegundos { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public string IdObra { get; set; } = "";
        public string NombreObra { get; set; } = "";

        public string TamanoTexto =>
            TamanoBytes >= 1024 * 1024 ? $"{TamanoBytes / 1024d / 1024d:0.0} MB"
            : TamanoBytes >= 1024 ? $"{TamanoBytes / 1024d:0} KB"
            : $"{TamanoBytes} B";

        public string DuracionTexto =>
            DuracionSegundos == null ? "" : TimeSpan.FromSeconds(DuracionSegundos.Value).ToString(@"m\:ss");
    }
}
