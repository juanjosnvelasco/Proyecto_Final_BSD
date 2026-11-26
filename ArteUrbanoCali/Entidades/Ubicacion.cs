namespace ArteUrbanoCali.Entidades
{
    /// <summary>Lugar físico donde se encuentra una obra (tabla UBICACIONES).</summary>
    public class Ubicacion
    {
        public string IdUbicacion { get; set; } = "";
        public string NombreLugar { get; set; } = "";
        public string Direccion { get; set; } = "";
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public string IdBarrio { get; set; } = "";
        public string NombreBarrio { get; set; } = "";
        public int NumeroComuna { get; set; }
    }
}
