namespace ArteUrbanoCali.Entidades
{
    
    public class Usuario
    {
        public string IdUsuario { get; set; } = "";
        public string Nombre1 { get; set; } = "";
        public string Apellido1 { get; set; } = "";
        public string Correo { get; set; } = "";
        public string ContrasenaHash { get; set; } = "";
        public string TipoUsuario { get; set; } = "";  
        public bool Estado { get; set; }

        public string NombreCompleto => $"{Nombre1} {Apellido1}";
        public bool EsAdministrador => TipoUsuario == "Administrador";
    }
}
