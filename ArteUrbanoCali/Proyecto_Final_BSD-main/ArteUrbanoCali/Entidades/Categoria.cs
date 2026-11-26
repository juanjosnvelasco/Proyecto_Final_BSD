namespace ArteUrbanoCali.Entidades
{
    /// <summary>Clasificación temática o técnica de las obras (tabla CATEGORIAS).</summary>
    public class Categoria
    {
        public string IdCategoria { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }
        public bool Estado { get; set; } = true;

        public string EstadoTexto => Estado ? "Activa" : "Inactiva";
    }
}
