namespace ArteUrbanoCali.Entidades
{
    /// <summary>Archivo digital asociado a una obra (tabla RECURSOS_MULTIMEDIAS). La ruta es relativa a RecursosMultimedia.</summary>
    public class RecursoMultimedia
    {
        public string IdRecurso { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Tipo { get; set; } = "";          // IMAGEN, VIDEO, AUDIO, MODELO3D, AR (viene de TIPOS_RECURSOS)
        public string? Descripcion { get; set; }
        public string RutaArchivo { get; set; } = "";   // siempre relativa, con "/"
        public string Formato { get; set; } = "";       // extensión (viene de FORMATOS_RECURSOS)
        public string IdFormato { get; set; } = "";
        public decimal TamanoKb { get; set; }           // la base guarda el tamaño en KB
        public int? DuracionSegundos { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public string IdObra { get; set; } = "";
        public string NombreObra { get; set; } = "";

        public string TamanoTexto =>
            TamanoKb >= 1024 ? $"{TamanoKb / 1024:0.0} MB" : $"{TamanoKb:0} KB";

        public string DuracionTexto =>
            DuracionSegundos == null ? "" : TimeSpan.FromSeconds(DuracionSegundos.Value).ToString(@"m\:ss");
    }
}
