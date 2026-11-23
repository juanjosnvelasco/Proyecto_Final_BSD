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

            // Agrega un módulo al menú. Si todavía no existe su formulario, avisa que está en construcción.
            void Agregar(string nombre, Func<Form>? crearFormulario)
            {
                var item = new ToolStripMenuItem(nombre);
                item.Click += (s, e) =>
                {
                    if (crearFormulario == null)
                    {
                        MessageBox.Show("Módulo de " + nombre + " en construcción.",
                                        "Arte Urbano Cali", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    using Form formulario = crearFormulario();
                    formulario.ShowDialog(this);
                };
                menu.Items.Add(item);
            }

            Agregar("Artistas", () => new FrmArtistas());
            Agregar("Categorías", () => new FrmCategorias());
            Agregar("Ubicaciones", () => new FrmUbicaciones());
            Agregar("Obras", null);
            Agregar("Multimedia", null);
            Agregar("Rutas", null);
            Agregar("Consultas", null);

            var salir = new ToolStripMenuItem("Salir");
            salir.Click += (s, e) => Close();
            menu.Items.Add(salir);

            Controls.Add(bienvenida);
            Controls.Add(menu);
            MainMenuStrip = menu;
        }
    }
}
