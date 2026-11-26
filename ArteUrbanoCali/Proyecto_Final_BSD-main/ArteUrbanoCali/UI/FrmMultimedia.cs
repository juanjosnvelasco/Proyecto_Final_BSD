using ArteUrbanoCali.Entidades;
using ArteUrbanoCali.Negocio;

namespace ArteUrbanoCali.UI
{
    /// <summary>Gestión de recursos multimedia: registrar, asociar a una obra, ver y reproducir.</summary>
    public class FrmMultimedia : Form
    {
        private readonly RecursoNegocio _negocio = new RecursoNegocio();
        private readonly DataGridView grid = Controles.Tabla();
        private readonly ComboBox cmbObraFiltro = new ComboBox { Location = new Point(70, 10), Width = 420, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox cmbObra = new ComboBox { Location = new Point(15, 30), Width = 330, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox cmbTipo = new ComboBox { Location = new Point(365, 30), Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtNombre = Controles.Caja(525, 30, 250);
        private readonly TextBox txtArchivo = new TextBox { Location = new Point(15, 90), Width = 430, ReadOnly = true };
        private readonly TextBox txtDuracion = Controles.Caja(555, 90, 90);
        private readonly TextBox txtDescripcion = new TextBox { Location = new Point(15, 150), Size = new Size(630, 50), Multiline = true };
        private readonly PictureBox picVista = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Gainsboro };
        private readonly Label lblVista = new Label { Dock = DockStyle.Bottom, Height = 40, TextAlign = ContentAlignment.MiddleCenter };

        private List<RecursoMultimedia> _recursos = new List<RecursoMultimedia>();
        private RecursoMultimedia? _seleccionado;
        private string? _origen;   // archivo o carpeta AR elegido y aún sin registrar

        public FrmMultimedia()
        {
            Text = "Recursos multimedia";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1100, 700);
            MinimumSize = new Size(1100, 700);

            Controles.Columna(grid, "Recurso", nameof(RecursoMultimedia.Nombre), 20);
            Controles.Columna(grid, "Obra", nameof(RecursoMultimedia.NombreObra), 20);
            Controles.Columna(grid, "Tipo", nameof(RecursoMultimedia.Tipo), 10);
            Controles.Columna(grid, "Formato", nameof(RecursoMultimedia.Formato), 8);
            Controles.Columna(grid, "Ruta relativa", nameof(RecursoMultimedia.RutaArchivo), 28);
            Controles.Columna(grid, "Tamaño", nameof(RecursoMultimedia.TamanoTexto), 9);
            Controles.Columna(grid, "Duración", nameof(RecursoMultimedia.DuracionTexto), 8);
            grid.SelectionChanged += (s, e) => MostrarSeleccionado();

            var pnlFiltro = new Panel { Dock = DockStyle.Top, Height = 42 };
            pnlFiltro.Controls.Add(Controles.Etiqueta("Obra:", 15, 13));
            pnlFiltro.Controls.Add(cmbObraFiltro);
            pnlFiltro.Controls.Add(Controles.Boton("Ver todos", 505, 7, 90, () => { cmbObraFiltro.SelectedIndex = -1; Cargar(); }));
            cmbObraFiltro.SelectionChangeCommitted += (s, e) => Cargar();

            cmbTipo.SelectedIndex = -1;
            cmbTipo.SelectedIndexChanged += (s, e) => { _origen = null; txtArchivo.Clear(); };

            var pnlCampos = new Panel { Dock = DockStyle.Bottom, Height = 250 };
            pnlCampos.Controls.AddRange(new Control[]
            {
                Controles.Etiqueta("Obra asociada *", 15, 10), cmbObra,
                Controles.Etiqueta("Tipo *", 365, 10), cmbTipo,
                Controles.Etiqueta("Nombre del recurso *", 525, 10), txtNombre,
                Controles.Etiqueta("Archivo (o carpeta, si es AR) *", 15, 70), txtArchivo,
                Controles.Boton("Examinar...", 455, 88, 90, Examinar),
                Controles.Etiqueta("Duración (segundos)", 555, 70), txtDuracion,
                Controles.Etiqueta("Descripción", 15, 130), txtDescripcion,
                Controles.Boton("Nuevo", 15, 210, 90, Nuevo),
                Controles.Boton("Guardar", 115, 210, 90, Guardar),
                Controles.Boton("Quitar registro", 215, 210, 120, Quitar)
            });

            var pnlAcciones = new Panel { Dock = DockStyle.Bottom, Height = 46 };
            pnlAcciones.Controls.Add(Controles.Boton("Ver / Reproducir", 5, 7, 140, Abrir));

            if (!Sesion.EsAdministrador)
            {
                pnlCampos.Enabled = false;
                pnlFiltro.Controls.Add(Controles.AvisoModoConsulta(620, 13));
            }

            var pnlVista = new Panel { Dock = DockStyle.Right, Width = 340 };
            pnlVista.Controls.Add(picVista);
            pnlVista.Controls.Add(lblVista);
            pnlVista.Controls.Add(pnlAcciones);

            Controls.Add(grid);
            Controls.Add(pnlVista);
            Controls.Add(pnlCampos);
            Controls.Add(pnlFiltro);

            Load += (s, e) => { CargarCombos(); Cargar(); };
        }

        private void CargarCombos()
        {
            Controles.Ejecutar(() =>
            {
                cmbTipo.DataSource = _negocio.ListarTipos();   // tipos permitidos según la base de datos
                cmbTipo.SelectedIndex = -1;
                var obras = _negocio.ListarObras();
                foreach (var combo in new[] { cmbObraFiltro, cmbObra })
                {
                    combo.DisplayMember = nameof(ItemCombo.Texto);
                    combo.ValueMember = nameof(ItemCombo.Id);
                    combo.DataSource = obras.ToList();
                    combo.SelectedIndex = -1;
                }
            });
        }

        private void Cargar()
        {
            Controles.Ejecutar(() =>
            {
                string? idObra = cmbObraFiltro.SelectedValue as string;
                _recursos = _negocio.Listar(idObra);
                grid.DataSource = _recursos;
            });
        }

        private void MostrarSeleccionado()
        {
            _seleccionado = grid.CurrentRow?.DataBoundItem as RecursoMultimedia;
            LimpiarVista();
            if (_seleccionado == null) return;

            cmbObra.SelectedValue = _seleccionado.IdObra;
            cmbTipo.SelectedItem = _seleccionado.Tipo;
            _origen = null;
            txtArchivo.Text = _seleccionado.RutaArchivo;
            txtNombre.Text = _seleccionado.Nombre;
            txtDescripcion.Text = _seleccionado.Descripcion ?? "";
            txtDuracion.Text = _seleccionado.DuracionSegundos?.ToString() ?? "";

            Image? imagen = null;
            Controles.Ejecutar(() => imagen = _negocio.CargarImagen(_seleccionado));
            if (imagen != null)
            {
                picVista.Image = imagen;
                lblVista.Text = _seleccionado.Nombre;
            }
            else
            {
                lblVista.Text = _seleccionado.Tipo switch
                {
                    "VIDEO" => "Video: use «Ver / Reproducir»",
                    "AUDIO" => "Audio: use «Ver / Reproducir»",
                    "MODELO3D" => "Modelo 3D: use «Ver / Reproducir»",
                    "AR" => "Referencia AR: se abre la carpeta del recurso",
                    _ => "Vista previa no disponible: use «Ver / Reproducir»"
                };
            }
        }

        private void LimpiarVista()
        {
            picVista.Image?.Dispose();
            picVista.Image = null;
            lblVista.Text = "";
        }

        private void Examinar()
        {
            string tipo = cmbTipo.SelectedItem as string ?? "";
            if (tipo.Length == 0) { Controles.Aviso("Primero seleccione el tipo de recurso."); return; }

            if (tipo == "AR")
            {
                using var dlg = new FolderBrowserDialog { Description = "Seleccione la carpeta de la referencia AR" };
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                _origen = dlg.SelectedPath;
            }
            else
            {
                string filtro = "";
                Controles.Ejecutar(() => filtro = _negocio.FiltroDialogo(tipo));
                using var dlg = new OpenFileDialog { Filter = filtro };
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                _origen = dlg.FileName;
            }
            txtArchivo.Text = _origen;
            if (txtNombre.Text.Trim().Length == 0)
                txtNombre.Text = Path.GetFileNameWithoutExtension(_origen.TrimEnd(Path.DirectorySeparatorChar));
        }

        private void Nuevo()
        {
            grid.CurrentCell = null;
            _seleccionado = null;
            _origen = null;
            LimpiarVista();
            txtNombre.Clear();
            txtArchivo.Clear();
            txtDescripcion.Clear();
            txtDuracion.Clear();
            cmbTipo.SelectedIndex = -1;
            cmbObra.SelectedValue = cmbObraFiltro.SelectedValue ?? "";
            txtNombre.Focus();
        }

        private void Guardar()
        {
            int? duracion = null;
            if (txtDuracion.Text.Trim().Length > 0)
            {
                if (!int.TryParse(txtDuracion.Text.Trim(), out int d))
                {
                    Controles.Aviso("La duración debe ser un número entero de segundos.");
                    return;
                }
                duracion = d;
            }

            var r = new RecursoMultimedia
            {
                IdRecurso = _seleccionado?.IdRecurso ?? "",
                IdObra = cmbObra.SelectedValue as string ?? "",
                Tipo = cmbTipo.SelectedItem as string ?? "",
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                DuracionSegundos = duracion
            };

            if (Controles.Ejecutar(() => _negocio.Guardar(r, _origen)))
            {
                Controles.Aviso("Recurso guardado correctamente.");
                Cargar();
            }
        }

        private void Quitar()
        {
            if (_seleccionado == null) { Controles.Aviso("Seleccione un recurso."); return; }
            if (!Controles.Confirmar("¿Quitar el registro de «" + _seleccionado.Nombre + "»?\nEl archivo no se borra de la carpeta."))
                return;
            if (Controles.Ejecutar(() => _negocio.Eliminar(_seleccionado.IdRecurso)))
                Cargar();
        }

        private void Abrir()
        {
            if (_seleccionado == null) { Controles.Aviso("Seleccione un recurso."); return; }
            Controles.Ejecutar(() => _negocio.Abrir(_seleccionado));
        }
    }
}
