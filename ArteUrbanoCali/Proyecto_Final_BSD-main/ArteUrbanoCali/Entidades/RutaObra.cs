namespace ArteUrbanoCali.Entidades
{
    /// <summary>Entidad asociativa Ruta–Obra (tabla RUTAS_OBRAS): guarda orden, recomendación y tiempo de permanencia.</summary>
    public class RutaObra
    {
        public string IdRutaObra { get; set; } = "";
        public string IdRuta { get; set; } = "";
        public string IdObra { get; set; } = "";
        public int Orden { get; set; }
        public string? Recomendacion { get; set; }
        public int TiempoMinutos { get; set; }

        // Datos de lectura
        public string NombreObra { get; set; } = "";
        public string NombreLugar { get; set; } = "";
        public string NombreBarrio { get; set; } = "";
        public string Direccion { get; set; } = "";
    }
}
