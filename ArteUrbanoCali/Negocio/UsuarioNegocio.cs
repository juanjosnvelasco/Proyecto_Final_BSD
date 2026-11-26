using ArteUrbanoCali.Datos;
using ArteUrbanoCali.Entidades;

namespace ArteUrbanoCali.Negocio
{
    
    public class UsuarioNegocio
    {
        private readonly UsuarioDatos _datos = new UsuarioDatos();

        public Usuario Autenticar(string correo, string contrasena)
        {
            correo = (correo ?? "").Trim();
            contrasena ??= "";

            if (correo.Length == 0 || contrasena.Length == 0)
                throw new ValidacionException("Ingrese su correo y su contraseña.");

            if (!correo.Contains('@'))
                throw new ValidacionException("El correo no tiene un formato válido.");

            Usuario? usuario = _datos.ObtenerPorCorreo(correo);

            
            if (usuario == null || !Seguridad.Verificar(contrasena, usuario.ContrasenaHash))
                throw new ValidacionException("Correo o contraseña incorrectos.");

            if (!usuario.Estado)
                throw new ValidacionException("Su usuario está inactivo. Contacte al administrador.");

            return usuario;
        }
    }
}
