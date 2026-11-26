using ArteUrbanoCali.Negocio;

namespace ArteUrbanoCali.UI
{
    /// <summary>Ayudas para construir formularios y mostrar mensajes de forma uniforme.</summary>
    internal static class Controles
    {
        public static Label Etiqueta(string texto, int x, int y)
        {
            return new Label { Text = texto, AutoSize = true, Location = new Point(x, y) };
        }

        public static TextBox Caja(int x, int y, int ancho)
        {
            return new TextBox { Location = new Point(x, y), Width = ancho };
        }

        public static Button Boton(string texto, int x, int y, int ancho, Action alHacerClic)
        {
            var boton = new Button { Text = texto, Location = new Point(x, y), Size = new Size(ancho, 32) };
            boton.Click += (s, e) => alHacerClic();
            return boton;
        }

        public static DataGridView Tabla()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                BackgroundColor = SystemColors.Window,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
        }

        public static void Columna(DataGridView tabla, string titulo, string propiedad, int peso)
        {
            tabla.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = titulo,
                DataPropertyName = propiedad,
                FillWeight = peso
            });
        }

        public static void Aviso(string mensaje)
        {
            MessageBox.Show(mensaje, "Arte Urbano Cali", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool Confirmar(string pregunta)
        {
            return MessageBox.Show(pregunta, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>Ejecuta una acción y muestra el error si falla. Devuelve true si todo salió bien.</summary>
        public static bool Ejecutar(Action accion)
        {
            try
            {
                accion();
                return true;
            }
            catch (ValidacionException ex)
            {
                MessageBox.Show(ex.Message, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error con la base de datos o la aplicación.\n\n" + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static Label AvisoModoConsulta(int x, int y)
        {
            return new Label
            {
                Text = "Modo consulta: su perfil no permite modificar información.",
                AutoSize = true,
                ForeColor = Color.Firebrick,
                Location = new Point(x, y)
            };
        }
    }
}
