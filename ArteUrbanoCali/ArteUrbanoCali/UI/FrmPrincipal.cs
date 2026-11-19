using ArteUrbanoCali.Negocio;

namespace ArteUrbanoCali.UI
{
    public class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            var usuario = Sesion.UsuarioActual!;

            Text = "Galería de Arte Urbano de Cali";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(900, 560);

            var bienvenida = new Label
            {
                Text = $"Bienvenido(a), {usuario.NombreCompleto}\nPerfil: {usuario.TipoUsuario}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16f)
            };

            var menu = new MenuStrip();
            foreach (string modulo in new[] { "Artistas", "Categorías", "Ubicaciones", "Obras", "Multimedia", "Rutas", "Consultas" })
            {
                var item = new ToolStripMenuItem(modulo);
                string nombre = modulo;
                item.Click += (s, e) => MessageBox.Show("Módulo de " + nombre + " en construcción.",
                                                        "Arte Urbano Cali", MessageBoxButtons.OK, MessageBoxIcon.Information);
                menu.Items.Add(item);
            }

            var salir = new ToolStripMenuItem("Salir");
            salir.Click += (s, e) => Close();
            menu.Items.Add(salir);

            Controls.Add(bienvenida);
            Controls.Add(menu);
            MainMenuStrip = menu;
        }
    }
}
