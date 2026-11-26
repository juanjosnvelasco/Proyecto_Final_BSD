namespace ArteUrbanoCali.Entidades
{
    /// <summary>Mural, grafiti o intervención urbana (tabla OBRAS) con los nombres de sus relaciones.</summary>
    public class Obra
    {
        public string IdObra { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Today;
        public string EstadoConservacion { get; set; } = "";
        public string IdArtista { get; set; } = "";
        public string IdCategoria { get; set; } = "";
        public string IdUbicacion { get; set; } = "";
        public string IdUsuario { get; set; } = "";

        // Datos de lectura, tomados de las tablas relacionadas
        public string NombreArtista { get; set; } = "";
        public string NombreCategoria { get; set; } = "";
        public string NombreLugar { get; set; } = "";
        public string NombreBarrio { get; set; } = "";
        public int NumeroComuna { get; set; }
        public int TotalRecursos { get; set; }

        public string FechaTexto => FechaCreacion.ToString("yyyy-MM-dd");
    }
}
