namespace ArteUrbanoCali.Entidades
{
    /// <summary>Formato permitido (tabla FORMATOS_RECURSOS) con su tipo y carpeta (tabla TIPOS_RECURSOS).</summary>
    public class FormatoRecurso
    {
        public string IdFormato { get; set; } = "";
        public string Extension { get; set; } = "";     // JPG, PNG, MP4, MP3, GLB, AR
        public string IdTipo { get; set; } = "";
        public string Tipo { get; set; } = "";         // IMAGEN, VIDEO, AUDIO, MODELO3D, AR
        public string Carpeta { get; set; } = "";      // ej. "Imagenes/"
    }
}
