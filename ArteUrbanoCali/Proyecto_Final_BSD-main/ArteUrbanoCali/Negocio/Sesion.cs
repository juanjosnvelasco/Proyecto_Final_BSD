using ArteUrbanoCali.Entidades;

namespace ArteUrbanoCali.Negocio
{
    
    public static class Sesion
    {
        public static Usuario? UsuarioActual { get; set; }
        public static bool EsAdministrador => UsuarioActual?.EsAdministrador ?? false;
    }
}
