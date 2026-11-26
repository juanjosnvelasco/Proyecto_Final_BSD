using ArteUrbanoCali.Entidades;
using ArteUrbanoCali.Negocio;

namespace ArteUrbanoCali.UI
{
    /// <summary>Rutas de arte urbano: crear rutas, agregar/retirar obras y definir el orden del recorrido.</summary>
    public class FrmRutas : Form
    {
        private readonly RutaNegocio _negocio = new RutaNegocio();
        private readonly DataGridView gridRutas = Controles.Tabla();
        private readonly DataGridView gridObras = Controles.Tabla();

        private readonly TextBox txtNombre = Controles.Caja(15, 30, 430);
        private readonly TextBox txtDuracion = Controles.Caja(15, 90, 100);
        private readonly TextBox txtDistancia = Controles.Caja(135, 90, 100);
        private readonly CheckBox chkActiva = new CheckBox { Text = "Ruta activa", Location = new Point(260, 92), AutoSize = true };
        private readonly TextBox txtDescripcion = new TextBox { Location = new Point(15, 150), Size = new Size(430, 60), Multiline = true };

        private readonly ComboBox cmbObra = new ComboBox { Location = new Point(15, 30), Width = 330, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtOrden = Controles.Caja(365, 30, 60);
        private readonly TextBox txtTiempo = Controles.Caja(445, 30, 60);
        private readonly TextBox txtRecomendacion = Controles.Caja(15, 90, 490);

        private List<Ruta> _rutas = new List<Ruta>();
        private List<RutaObra> _obras = new List<RutaObra>();
        private Ruta? _ruta;

        public FrmRutas()
        {
            Text = "Rutas de arte urbano";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1100, 700);
            MinimumSize = new Size(1100, 700);

            Controles.Columna(gridRutas, "Ruta", nameof(Ruta.Nombre), 40);
            Controles.Columna(gridRutas, "Min", nameof(Ruta.DuracionMinutos), 12);
            Controles.Columna(gridRutas, "Km", nameof(Ruta.DistanciaKm), 12);
            Controles.Columna(gridRutas, "Obras", nameof(Ruta.TotalObras), 12);
            Controles.Columna(gridRutas, "Estado", nameof(Ruta.EstadoTexto), 16);
            gridRutas.SelectionChanged += (s, e) => MostrarRuta();

            Controles.Columna(gridObras, "Orden", nameof(RutaObra.Orden), 8);
            Controles.Columna(gridObras, "Obra", nameof(RutaObra.NombreObra), 24);
            Controles.Columna(gridObras, "Lugar", nameof(RutaObra.NombreLugar), 20);
            Controles.Columna(gridObras, "Barrio", nameof(RutaObra.NombreBarrio), 14);
            Controles.Columna(gridObras, "Minutos", nameof(RutaObra.TiempoMinutos), 9);
            Controles.Columna(gridObras, "Recomendación", nameof(RutaObra.Recomendacion), 25);

            // ----- Panel izquierdo: rutas -----
            var pnlCamposRuta = new Panel { Dock = DockStyle.Bottom, Height = 260 };
            pnlCamposRuta.Controls.AddRange(new Control[]
            {
                Controles.Etiqueta("Nombre de la ruta *", 15, 10), txtNombre,
                Controles.Etiqueta("Duración estimada (min) *", 15, 70), txtDuracion,
                Controles.Etiqueta("Distancia (km) *", 135, 70), txtDistancia, chkActiva,
                Controles.Etiqueta("Descripción", 15, 130), txtDescripcion,
                Controles.Boton("Nueva", 15, 220, 90, NuevaRuta),
                Controles.Boton("Guardar", 115, 220, 90, GuardarRuta),
                Controles.Etiqueta("Una ruta activa necesita al menos 2 obras.", 215, 226)
            });

            var pnlIzq = new Panel { Dock = DockStyle.Left, Width = 470 };
            pnlIzq.Controls.Add(gridRutas);
            pnlIzq.Controls.Add(pnlCamposRuta);

            // ----- Panel derecho: obras de la ruta -----
            var pnlAgregar = new Panel { Dock = DockStyle.Bottom, Height = 200 };
            pnlAgregar.Controls.AddRange(new Control[]
            {
                Controles.Etiqueta("Obra *", 15, 10), cmbObra,
                Controles.Etiqueta("Orden *", 365, 10), txtOrden,
                Controles.Etiqueta("Minutos *", 445, 10), txtTiempo,
                Controles.Etiqueta("Recomendación", 15, 70), txtRecomendacion,
                Controles.Boton("Agregar a la ruta", 15, 135, 140, AgregarObra),
                Controles.Boton("Retirar obra", 165, 135, 110, RetirarObra),
                Controles.Boton("Subir", 285, 135, 70, () => Mover(-1)),
                Controles.Boton("Bajar", 365, 135, 70, () => Mover(1))
            });

            var pnlTituloObras = new Label
            {
                Dock = DockStyle.Top, Height = 30, Text = "  Obras de la ruta seleccionada (en orden de recorrido)",
                TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            var pnlDer = new Panel { Dock = DockStyle.Fill };
            pnlDer.Controls.Add(gridObras);
            pnlDer.Controls.Add(pnlAgregar);
            pnlDer.Controls.Add(pnlTituloObras);

            if (!Sesion.EsAdministrador)
            {
                pnlCamposRuta.Enabled = false;
                pnlAgregar.Enabled = false;
                pnlTituloObras.Text += "   —   Modo consulta";
            }

            Controls.Add(pnlDer);
            Controls.Add(pnlIzq);

            Load += (s, e) => { CargarObrasCombo(); CargarRutas(); };
        }

        private void CargarObrasCombo()
        {
            Controles.Ejecutar(() =>
            {
                cmbObra.DisplayMember = nameof(ItemCombo.Texto);
                cmbObra.ValueMember = nameof(ItemCombo.Id);
                cmbObra.DataSource = _negocio.ListarObras();
                cmbObra.SelectedIndex = -1;
            });
        }

        private void CargarRutas(string? seleccionarId = null)
        {
            Controles.Ejecutar(() =>
            {
                _rutas = _negocio.Listar();
                gridRutas.DataSource = _rutas;
            });
            if (seleccionarId != null)
            {
                foreach (DataGridViewRow fila in gridRutas.Rows)
                {
                    if (fila.DataBoundItem is Ruta r && r.IdRuta == seleccionarId)
                    {
                        gridRutas.CurrentCell = fila.Cells[0];
                        fila.Selected = true;
                        break;
                    }
                }
            }
        }

        private void MostrarRuta()
        {
            _ruta = gridRutas.CurrentRow?.DataBoundItem as Ruta;
            if (_ruta == null) { gridObras.DataSource = null; return; }

            txtNombre.Text = _ruta.Nombre;
            txtDescripcion.Text = _ruta.Descripcion ?? "";
            txtDuracion.Text = _ruta.DuracionMinutos.ToString();
            txtDistancia.Text = _ruta.DistanciaKm.ToString(System.Globalization.CultureInfo.InvariantCulture);
            chkActiva.Checked = _ruta.Estado;
            CargarObrasRuta();
        }

        private void CargarObrasRuta()
        {
            if (_ruta == null) return;
            Controles.Ejecutar(() =>
            {
                _obras = _negocio.ListarObras(_ruta.IdRuta);
                gridObras.DataSource = _obras;
                txtOrden.Text = _negocio.SiguienteOrden(_ruta.IdRuta).ToString();
            });
        }

        private void NuevaRuta()
        {
            gridRutas.CurrentCell = null;
            _ruta = null;
            gridObras.DataSource = null;
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtDuracion.Clear();
            txtDistancia.Clear();
            chkActiva.Checked = false;
            txtNombre.Focus();
        }

        private void GuardarRuta()
        {
            if (!int.TryParse(txtDuracion.Text.Trim(), out int duracion))
            {
                Controles.Aviso("La duración debe ser un número entero de minutos.");
                return;
            }
            string d = txtDistancia.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(d, System.Globalization.NumberStyles.Float,
                                  System.Globalization.CultureInfo.InvariantCulture, out decimal distancia))
            {
                Controles.Aviso("La distancia debe ser un número (ejemplo: 3.5).");
                return;
            }

            var ruta = new Ruta
            {
                IdRuta = _ruta?.IdRuta ?? "",
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                DuracionMinutos = duracion,
                DistanciaKm = distancia,
                Estado = chkActiva.Checked
            };

            if (Controles.Ejecutar(() => _negocio.Guardar(ruta)))
            {
                Controles.Aviso(_ruta == null
                    ? "Ruta creada como inactiva. Agregue al menos 2 obras y luego actívela."
                    : "Ruta guardada correctamente.");
                CargarRutas(ruta.IdRuta);
            }
        }

        private void AgregarObra()
        {
            if (_ruta == null) { Controles.Aviso("Seleccione o guarde primero una ruta."); return; }
            if (!int.TryParse(txtOrden.Text.Trim(), out int orden) || !int.TryParse(txtTiempo.Text.Trim(), out int tiempo))
            {
                Controles.Aviso("El orden y los minutos de permanencia deben ser números enteros.");
                return;
            }

            string idObra = cmbObra.SelectedValue as string ?? "";
            if (Controles.Ejecutar(() => _negocio.AgregarObra(_ruta.IdRuta, idObra, orden, txtRecomendacion.Text, tiempo)))
            {
                txtRecomendacion.Clear();
                txtTiempo.Clear();
                cmbObra.SelectedIndex = -1;
                CargarRutas(_ruta.IdRuta);
            }
        }

        private void RetirarObra()
        {
            var obra = gridObras.CurrentRow?.DataBoundItem as RutaObra;
            if (_ruta == null || obra == null) { Controles.Aviso("Seleccione una obra de la ruta."); return; }
            if (!Controles.Confirmar("¿Retirar «" + obra.NombreObra + "» de la ruta?")) return;

            if (Controles.Ejecutar(() => _negocio.RetirarObra(_ruta, obra)))
                CargarRutas(_ruta.IdRuta);
        }

        private void Mover(int direccion)
        {
            var obra = gridObras.CurrentRow?.DataBoundItem as RutaObra;
            if (_ruta == null || obra == null) { Controles.Aviso("Seleccione una obra de la ruta."); return; }

            string idRutaObra = obra.IdRutaObra;
            if (Controles.Ejecutar(() => _negocio.Mover(_obras, obra, direccion)))
            {
                string idRuta = _ruta.IdRuta;
                CargarRutas(idRuta);
                foreach (DataGridViewRow fila in gridObras.Rows)
                {
                    if (fila.DataBoundItem is RutaObra o && o.IdRutaObra == idRutaObra)
                    {
                        gridObras.CurrentCell = fila.Cells[0];
                        fila.Selected = true;
                        break;
                    }
                }
            }
        }
    }
}
