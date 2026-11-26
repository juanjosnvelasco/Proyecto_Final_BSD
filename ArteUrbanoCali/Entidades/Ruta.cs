namespace ArteUrbanoCali.Entidades
{
    /// <summary>Recorrido compuesto por varias obras (tabla RUTAS).</summary>
    public class Ruta
    {
        public string IdRuta { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }
        public int DuracionMinutos { get; set; }
        public decimal DistanciaKm { get; set; }
        public bool Estado { get; set; }
        public int TotalObras { get; set; }

        public string EstadoTexto => Estado ? "Activa" : "Inactiva";
    }
}
