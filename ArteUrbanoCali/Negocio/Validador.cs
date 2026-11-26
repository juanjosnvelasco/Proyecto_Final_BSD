using System.Globalization;

namespace ArteUrbanoCali.Negocio
{
    /// <summary>Validaciones reutilizables. Lanzan ValidacionException con un mensaje claro.</summary>
    public static class Validador
    {
        public static string Obligatorio(string? valor, string campo, int maximo)
        {
            string texto = (valor ?? "").Trim();
            if (texto.Length == 0)
                throw new ValidacionException($"El campo {campo} es obligatorio.");
            if (texto.Length > maximo)
                throw new ValidacionException($"El campo {campo} no puede superar {maximo} caracteres.");
            return texto;
        }

        public static string? Opcional(string? valor, string campo, int maximo)
        {
            string texto = (valor ?? "").Trim();
            if (texto.Length == 0) return null;
            if (texto.Length > maximo)
                throw new ValidacionException($"El campo {campo} no puede superar {maximo} caracteres.");
            return texto;
        }

        public static void ExigirAdministrador()
        {
            if (!Sesion.EsAdministrador)
                throw new ValidacionException("Su perfil no tiene permiso para modificar información.");
        }

        // Acepta punto o coma como separador decimal.
        public static decimal Coordenada(string? texto, string campo, decimal minimo, decimal maximo)
        {
            string t = (texto ?? "").Trim().Replace(',', '.');
            if (t.Length == 0)
                throw new ValidacionException($"El campo {campo} es obligatorio.");
            if (!decimal.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal valor))
                throw new ValidacionException($"El campo {campo} debe ser un número (ejemplo: 3.4516).");
            if (valor < minimo || valor > maximo)
                throw new ValidacionException($"El campo {campo} debe estar entre {minimo} y {maximo}.");
            return Math.Round(valor, 6);
        }
    }
}
