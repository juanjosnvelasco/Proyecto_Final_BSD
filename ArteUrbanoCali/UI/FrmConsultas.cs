using ArteUrbanoCali.Entidades;
using ArteUrbanoCali.Negocio;

namespace ArteUrbanoCali.UI
{
    /// <summary>Consultas que combinan varias tablas y estadísticas con agregaciones.</summary>
    public class FrmConsultas : Form
    {
        private readonly ConsultaNegocio _negocio = new ConsultaNegocio();
        private readonly ComboBox cmbConsulta = new ComboBox { Location = new Point(15, 30), Width = 430, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox cmbValor = new ComboBox { Location = new Point(465, 30), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtValor = Controles.Caja(465, 30, 300);
        private readonly Label lblValor = Controles.Etiqueta("Parámetro", 465, 10);
        private readonly Label lblTotal = new Label { Dock = DockStyle.Bottom, Height = 28, TextAlign = ContentAlignment.MiddleLeft };
        private readonly DataGridView grid;

        public FrmConsultas()
        {
            Text = "Consultas y estadísticas";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1100, 620);
            MinimumSize = new Size(1000, 560);

            grid = Controles.Tabla();
            grid.AutoGenerateColumns = true;

            cmbConsulta.DataSource = ConsultaNegocio.Catalogo.Select(c => c.Nombre).ToList();
            cmbConsulta.SelectedIndex = -1;
            cmbConsulta.SelectedIndexChanged += (s, e) => PrepararParametro();

            var pnlSuperior = new Panel { Dock = DockStyle.Top, Height = 75 };
            pnlSuperior.Controls.AddRange(new Control[]
            {
                Controles.Etiqueta("Consulta", 15, 10), cmbConsulta,
                lblValor, cmbValor, txtValor,
                Controles.Boton("Ejecutar", 785, 27, 100, Ejecutar)
            });
            cmbValor.Visible = false;
            txtValor.Visible = false;
            lblValor.Visible = false;

            Controls.Add(grid);
            Controls.Add(lblTotal);
            Controls.Add(pnlSuperior);
        }

        private Parametro ParametroActual =>
            cmbConsulta.SelectedIndex < 0 ? Parametro.Ninguno : ConsultaNegocio.Catalogo[cmbConsulta.SelectedIndex].Parametro;

        private void PrepararParametro()
        {
            Parametro p = ParametroActual;
            bool usaCombo = p is Parametro.Obra or Parametro.Artista or Parametro.Categoria or Parametro.Ruta;
            bool usaTexto = p is Parametro.Texto or Parametro.Numero;

            cmbValor.Visible = usaCombo;
            txtValor.Visible = usaTexto;
            lblValor.Visible = usaCombo || usaTexto;
            lblValor.Text = p switch
            {
                Parametro.Obra => "Obra",
                Parametro.Artista => "Artista",
                Parametro.Categoria => "Categoría",
                Parametro.Ruta => "Ruta",
                Parametro.Texto => "Sector, barrio o comuna",
                Parametro.Numero => "N (mínimo de obras, exclusivo)",
                _ => ""
            };
            txtValor.Clear();

            if (usaCombo)
            {
                Controles.Ejecutar(() =>
                {
                    cmbValor.DisplayMember = nameof(ItemCombo.Texto);
                    cmbValor.ValueMember = nameof(ItemCombo.Id);
                    cmbValor.DataSource = _negocio.ListarItems(p);
                    cmbValor.SelectedIndex = -1;
                });
            }
        }

        private void Ejecutar()
        {
            if (cmbConsulta.SelectedIndex < 0) { Controles.Aviso("Seleccione una consulta."); return; }

            Parametro p = ParametroActual;
            string valor = p switch
            {
                Parametro.Obra or Parametro.Artista or Parametro.Categoria or Parametro.Ruta => cmbValor.SelectedValue as string ?? "",
                Parametro.Texto or Parametro.Numero => txtValor.Text.Trim(),
                _ => ""
            };

            Controles.Ejecutar(() =>
            {
                var tabla = _negocio.Ejecutar(cmbConsulta.SelectedIndex, valor);
                grid.DataSource = tabla;
                lblTotal.Text = tabla.Rows.Count == 0
                    ? "  Sin resultados."
                    : $"  {tabla.Rows.Count} resultado(s).";
            });
        }
    }
}
