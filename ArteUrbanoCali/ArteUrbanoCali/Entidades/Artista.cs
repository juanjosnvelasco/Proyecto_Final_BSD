namespace ArteUrbanoCali.Entidades
{
    /// <summary>Artista responsable de una o varias obras (tabla ARTISTAS).</summary>
    public class Artista
    {
        public string IdArtista { get; set; } = "";
        public string NombreArtistico { get; set; } = "";
        public string Nombre1Real { get; set; } = "";
        public string? Nombre2Real { get; set; }
        public string Apellido1Real { get; set; } = "";
        public string? Apellido2Real { get; set; }
        public string? Biografia { get; set; }
        public string? PerfilRedSocial { get; set; }
        public bool Estado { get; set; } = true;
        public string IdPais { get; set; } = "";
        public string NombrePais { get; set; } = "";

        public string EstadoTexto => Estado ? "Activo" : "Inactivo";

        public string NombreReal =>
            string.Join(" ", new[] { Nombre1Real, Nombre2Real, Apellido1Real, Apellido2Real }
                              .Where(p => !string.IsNullOrWhiteSpace(p)));
    }
}
