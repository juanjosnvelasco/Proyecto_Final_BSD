using System.Globalization;
using ArteUrbanoCali.Entidades;
using ArteUrbanoCali.Negocio;

namespace ArteUrbanoCali.UI
{
    public class FrmUbicaciones : Form
    {
        private readonly UbicacionNegocio _negocio = new UbicacionNegocio();
        private readonly DataGridView grid = Controles.Tabla();
        private readonly TextBox txtBuscar = Controles.Caja(70, 10, 300);
        private readonly TextBox txtLugar = Controles.Caja(15, 30, 300);
        private readonly TextBox txtDireccion = Controles.Caja(335, 30, 300);
        private readonly ComboBox cmbBarrio = new ComboBox { Location = new Point(655, 30), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtLatitud = Controles.Caja(15, 90, 150);
        private readonly TextBox txtLongitud = Controles.Caja(185, 90, 150);
        private readonly Button btnNuevo;
        private readonly Button btnGuardar;

        private List<Ubicacion> _todas = new List<Ubicacion>();
        private Ubicacion? _seleccionada;

        public FrmUbicaciones()
        {
            Text = "Ubicaciones";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1000, 600);
            MinimumSize = new Size(1020, 560);

            Controles.Columna(grid, "Lugar", nameof(Ubicacion.NombreLugar), 28);
            Controles.Columna(grid, "Dirección", nameof(Ubicacion.Direccion), 26);
            Controles.Columna(grid, "Barrio", nameof(Ubicacion.NombreBarrio), 18);
            Controles.Columna(grid, "Comuna", nameof(Ubicacion.NumeroComuna), 8);
            Controles.Columna(grid, "Latitud", nameof(Ubicacion.Latitud), 10);
            Controles.Columna(grid, "Longitud", nameof(Ubicacion.Longitud), 10);
            grid.SelectionChanged += (s, e) => MostrarSeleccionada();

            var pnlBusqueda = new Panel { Dock = DockStyle.Top, Height = 42 };
            pnlBusqueda.Controls.Add(Controles.Etiqueta("Buscar:", 15, 13));
            pnlBusqueda.Controls.Add(txtBuscar);
            txtBuscar.TextChanged += (s, e) => Filtrar();

            btnNuevo = Controles.Boton("Nuevo", 15, 140, 90, Nuevo);
            btnGuardar = Controles.Boton("Guardar", 115, 140, 90, Guardar);

            var pnlCampos = new Panel { Dock = DockStyle.Bottom, Height = 195 };
            pnlCampos.Controls.AddRange(new Control[]
            {
                Controles.Etiqueta("Nombre del lugar *", 15, 10), txtLugar,
                Controles.Etiqueta("Dirección *", 335, 10), txtDireccion,
                Controles.Etiqueta("Barrio *", 655, 10), cmbBarrio,
                Controles.Etiqueta("Latitud *", 15, 70), txtLatitud,
                Controles.Etiqueta("Longitud *", 185, 70), txtLongitud,
                Controles.Etiqueta("Ejemplo para Cali: latitud 3.4516 y longitud -76.5320", 355, 93),
                btnNuevo, btnGuardar
            });

            if (!Sesion.EsAdministrador)
            {
                pnlCampos.Enabled = false;
                pnlBusqueda.Controls.Add(Controles.AvisoModoConsulta(420, 13));
            }

            Controls.Add(grid);
            Controls.Add(pnlCampos);
            Controls.Add(pnlBusqueda);

            Load += (s, e) => { CargarBarrios(); Cargar(); };
        }

        private void CargarBarrios()
        {
            Controles.Ejecutar(() =>
            {
                cmbBarrio.DisplayMember = nameof(Barrio.Etiqueta);
                cmbBarrio.ValueMember = nameof(Barrio.IdBarrio);
                cmbBarrio.DataSource = _negocio.ListarBarrios();
                cmbBarrio.SelectedIndex = -1;
            });
        }

        private void Cargar()
        {
            Controles.Ejecutar(() =>
            {
                _todas = _negocio.Listar();
                Filtrar();
            });
        }

        private void Filtrar()
        {
            string texto = txtBuscar.Text.Trim();
            List<Ubicacion> lista = texto.Length == 0
                ? _todas
                : _todas.Where(u => u.NombreLugar.Contains(texto, StringComparison.OrdinalIgnoreCase)
                                 || u.NombreBarrio.Contains(texto, StringComparison.OrdinalIgnoreCase)
                                 || u.NumeroComuna.ToString() == texto).ToList();
            grid.DataSource = lista;
        }

        private void MostrarSeleccionada()
        {
            _seleccionada = grid.CurrentRow?.DataBoundItem as Ubicacion;
            if (_seleccionada == null) return;

            txtLugar.Text = _seleccionada.NombreLugar;
            txtDireccion.Text = _seleccionada.Direccion;
            txtLatitud.Text = _seleccionada.Latitud.ToString(CultureInfo.InvariantCulture);
            txtLongitud.Text = _seleccionada.Longitud.ToString(CultureInfo.InvariantCulture);
            cmbBarrio.SelectedValue = _seleccionada.IdBarrio;
        }

        private void Nuevo()
        {
            grid.CurrentCell = null;
            _seleccionada = null;
            foreach (TextBox caja in new[] { txtLugar, txtDireccion, txtLatitud, txtLongitud })
                caja.Clear();
            cmbBarrio.SelectedIndex = -1;
            txtLugar.Focus();
        }

        private void Guardar()
        {
            var ubicacion = new Ubicacion
            {
                IdUbicacion = _seleccionada?.IdUbicacion ?? "",
                NombreLugar = txtLugar.Text,
                Direccion = txtDireccion.Text,
                IdBarrio = cmbBarrio.SelectedValue as string ?? ""
            };

            if (Controles.Ejecutar(() => _negocio.Guardar(ubicacion, txtLatitud.Text, txtLongitud.Text)))
            {
                Controles.Aviso("Ubicación guardada correctamente.");
                Cargar();
                SeleccionarPorId(ubicacion.IdUbicacion);
            }
        }

        private void SeleccionarPorId(string id)
        {
            foreach (DataGridViewRow fila in grid.Rows)
            {
                if (fila.DataBoundItem is Ubicacion u && u.IdUbicacion == id)
                {
                    grid.CurrentCell = fila.Cells[0];
                    fila.Selected = true;
                    return;
                }
            }
        }
    }
}
