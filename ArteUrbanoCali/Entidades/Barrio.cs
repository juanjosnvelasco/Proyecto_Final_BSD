namespace ArteUrbanoCali.Entidades
{
    /// <summary>Barrio con el número de su comuna (tablas BARRIOS y COMUNAS).</summary>
    public class Barrio
    {
        public string IdBarrio { get; set; } = "";
        public string Nombre { get; set; } = "";
        public int NumeroComuna { get; set; }

        public string Etiqueta => $"{Nombre} (Comuna {NumeroComuna})";
    }
}
