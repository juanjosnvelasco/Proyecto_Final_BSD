using ArteUrbanoCali.Negocio;

namespace ArteUrbanoCali.UI
{
    public class FrmLogin : Form
    {
        private readonly TextBox txtCorreo = new TextBox();
        private readonly TextBox txtContrasena = new TextBox();
        private readonly Button btnIngresar = new Button();

        public FrmLogin()
        {
            Text = "Arte Urbano Cali - Inicio de sesión";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            ClientSize = new Size(380, 250);

            var lblTitulo = new Label
            {
                Text = "Galería de Arte Urbano de Cali",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            var lblCorreo = new Label { Text = "Correo", AutoSize = true, Location = new Point(20, 70) };
            txtCorreo.Location = new Point(20, 92);
            txtCorreo.Width = 340;

            var lblContrasena = new Label { Text = "Contraseña", AutoSize = true, Location = new Point(20, 130) };
            txtContrasena.Location = new Point(20, 152);
            txtContrasena.Width = 340;
            txtContrasena.UseSystemPasswordChar = true;

            btnIngresar.Text = "Ingresar";
            btnIngresar.Location = new Point(20, 195);
            btnIngresar.Size = new Size(340, 34);
            btnIngresar.Click += Ingresar;

            Controls.AddRange(new Control[] { lblTitulo, lblCorreo, txtCorreo, lblContrasena, txtContrasena, btnIngresar });
            AcceptButton = btnIngresar;   // Enter = Ingresar
        }

        private void Ingresar(object? sender, EventArgs e)
        {
            try
            {
                Sesion.UsuarioActual = new UsuarioNegocio().Autenticar(txtCorreo.Text, txtContrasena.Text);

                var principal = new FrmPrincipal();
                principal.FormClosed += (s, args) => Close();
                Hide();
                principal.Show();
            }
            catch (ValidacionException ex)
            {
                MessageBox.Show(ex.Message, "Inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtContrasena.Clear();
                txtContrasena.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo conectar con la base de datos.\n\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
