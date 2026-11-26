using ArteUrbanoCali.Entidades;
using ArteUrbanoCali.Negocio;

namespace ArteUrbanoCali.UI
{
    public class FrmObras : Form
    {
        private readonly ObraNegocio _negocio = new ObraNegocio();
        private readonly DataGridView grid = Controles.Tabla();
        private readonly TextBox txtBuscar = Controles.Caja(70, 10, 300);
        private readonly TextBox txtNombre = Controles.Caja(15, 30, 300);
        private readonly ComboBox cmbArtista = CrearCombo(335, 30, 300);
        private readonly ComboBox cmbCategoria = CrearCombo(655, 30, 320);
        private readonly ComboBox cmbUbicacion = CrearCombo(15, 90, 300);
        private readonly DateTimePicker dtpFecha = new DateTimePicker
        {
            Location = new Point(335, 90),
            Width = 150,
            Format = DateTimePickerFormat.Short,
            MinDate = new DateTime(1900, 1, 1),
            MaxDate = DateTime.Today
        };
        private readonly ComboBox cmbEstado = CrearCombo(505, 90, 200);
        private readonly TextBox txtDescripcion = new TextBox { Location = new Point(15, 150), Size = new Size(640, 70), Multiline = true, ScrollBars = ScrollBars.Vertical };
        private readonly Button btnNuevo;
        private readonly Button btnGuardar;

        private List<Obra> _todas = new List<Obra>();
        private Obra? _seleccionada;

        public FrmObras()
        {
            Text = "Obras";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1000, 660);
            MinimumSize = new Size(1020, 620);

            Controles.Columna(grid, "Obra", nameof(Obra.Nombre), 22);
            Controles.Columna(grid, "Artista", nameof(Obra.NombreArtista), 15);
            Controles.Columna(grid, "Categoría", nameof(Obra.NombreCategoria), 13);
            Controles.Columna(grid, "Lugar", nameof(Obra.NombreLugar), 18);
            Controles.Columna(grid, "Barrio", nameof(Obra.NombreBarrio), 12);
            Controles.Columna(grid, "Comuna", nameof(Obra.NumeroComuna), 7);
            Controles.Columna(grid, "Creada", nameof(Obra.FechaTexto), 10);
            Controles.Columna(grid, "Conservación", nameof(Obra.EstadoConservacion), 12);
            Controles.Columna(grid, "Recursos", nameof(Obra.TotalRecursos), 8);
            grid.SelectionChanged += (s, e) => MostrarSeleccionada();

            var pnlBusqueda = new Panel { Dock = DockStyle.Top, Height = 42 };
            pnlBusqueda.Controls.Add(Controles.Etiqueta("Buscar:", 15, 13));
            pnlBusqueda.Controls.Add(txtBuscar);
            txtBuscar.TextChanged += (s, e) => Filtrar();

            cmbEstado.DataSource = ObraNegocio.EstadosConservacion.ToList();
            cmbEstado.SelectedIndex = -1;

            btnNuevo = Controles.Boton("Nuevo", 670, 150, 90, Nuevo);
            btnGuardar = Controles.Boton("Guardar", 770, 150, 90, Guardar);

            var pnlCampos = new Panel { Dock = DockStyle.Bottom, Height = 235 };
            pnlCampos.Controls.AddRange(new Control[]
            {
                Controles.Etiqueta("Nombre de la obra *", 15, 10), txtNombre,
                Controles.Etiqueta("Artista *", 335, 10), cmbArtista,
                Controles.Etiqueta("Categoría *", 655, 10), cmbCategoria,
                Controles.Etiqueta("Ubicación *", 15, 70), cmbUbicacion,
                Controles.Etiqueta("Fecha de creación *", 335, 70), dtpFecha,
                Controles.Etiqueta("Estado de conservación *", 505, 70), cmbEstado,
                Controles.Etiqueta("Descripción", 15, 130), txtDescripcion,
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

            Load += (s, e) => { CargarCombos(); Cargar(); };
        }

        private static ComboBox CrearCombo(int x, int y, int ancho)
        {
            return new ComboBox { Location = new Point(x, y), Width = ancho, DropDownStyle = ComboBoxStyle.DropDownList };
        }

        private static void LlenarCombo(ComboBox combo, List<ItemCombo> items)
        {
            combo.DisplayMember = nameof(ItemCombo.Texto);
            combo.ValueMember = nameof(ItemCombo.Id);
            combo.DataSource = items;
            combo.SelectedIndex = -1;
        }

        private void CargarCombos()
        {
            Controles.Ejecutar(() =>
            {
                LlenarCombo(cmbArtista, _negocio.ListarArtistas());
                LlenarCombo(cmbCategoria, _negocio.ListarCategorias());
                LlenarCombo(cmbUbicacion, _negocio.ListarUbicaciones());
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
            List<Obra> lista = texto.Length == 0
                ? _todas
                : _todas.Where(o => o.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase)
                                 || o.NombreArtista.Contains(texto, StringComparison.OrdinalIgnoreCase)
                                 || o.NombreCategoria.Contains(texto, StringComparison.OrdinalIgnoreCase)
                                 || o.NombreBarrio.Contains(texto, StringComparison.OrdinalIgnoreCase)).ToList();
            grid.DataSource = lista;
        }

        private void MostrarSeleccionada()
        {
            _seleccionada = grid.CurrentRow?.DataBoundItem as Obra;
            if (_seleccionada == null) return;

            txtNombre.Text = _seleccionada.Nombre;
            txtDescripcion.Text = _seleccionada.Descripcion ?? "";
            cmbArtista.SelectedValue = _seleccionada.IdArtista;
            cmbCategoria.SelectedValue = _seleccionada.IdCategoria;
            cmbUbicacion.SelectedValue = _seleccionada.IdUbicacion;
            cmbEstado.SelectedItem = _seleccionada.EstadoConservacion;

            DateTime fecha = _seleccionada.FechaCreacion;
            if (fecha < dtpFecha.MinDate) fecha = dtpFecha.MinDate;
            if (fecha > dtpFecha.MaxDate) fecha = dtpFecha.MaxDate;
            dtpFecha.Value = fecha;
        }

        private void Nuevo()
        {
            grid.CurrentCell = null;
            _seleccionada = null;
            txtNombre.Clear();
            txtDescripcion.Clear();
            cmbArtista.SelectedIndex = -1;
            cmbCategoria.SelectedIndex = -1;
            cmbUbicacion.SelectedIndex = -1;
            cmbEstado.SelectedIndex = -1;
            dtpFecha.Value = DateTime.Today;
            txtNombre.Focus();
        }

        private void Guardar()
        {
            var obra = new Obra
            {
                IdObra = _seleccionada?.IdObra ?? "",
                IdUsuario = _seleccionada?.IdUsuario ?? "",
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                FechaCreacion = dtpFecha.Value.Date,
                EstadoConservacion = cmbEstado.SelectedItem as string ?? "",
                IdArtista = cmbArtista.SelectedValue as string ?? "",
                IdCategoria = cmbCategoria.SelectedValue as string ?? "",
                IdUbicacion = cmbUbicacion.SelectedValue as string ?? ""
            };

            if (Controles.Ejecutar(() => _negocio.Guardar(obra)))
            {
                Controles.Aviso("Obra guardada correctamente.");
                Cargar();
                SeleccionarPorId(obra.IdObra);
            }
        }

        private void SeleccionarPorId(string id)
        {
            foreach (DataGridViewRow fila in grid.Rows)
            {
                if (fila.DataBoundItem is Obra o && o.IdObra == id)
                {
                    grid.CurrentCell = fila.Cells[0];
                    fila.Selected = true;
                    return;
                }
            }
        }
    }
}
