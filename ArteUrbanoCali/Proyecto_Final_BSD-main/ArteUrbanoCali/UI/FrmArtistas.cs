using ArteUrbanoCali.Entidades;
using ArteUrbanoCali.Negocio;

namespace ArteUrbanoCali.UI
{
    public class FrmArtistas : Form
    {
        private readonly ArtistaNegocio _negocio = new ArtistaNegocio();
        private readonly DataGridView grid = Controles.Tabla();
        private readonly TextBox txtBuscar = Controles.Caja(70, 10, 300);
        private readonly TextBox txtNombreArtistico = Controles.Caja(15, 30, 250);
        private readonly TextBox txtNombre1 = Controles.Caja(285, 30, 170);
        private readonly TextBox txtNombre2 = Controles.Caja(470, 30, 170);
        private readonly TextBox txtApellido1 = Controles.Caja(655, 30, 150);
        private readonly TextBox txtApellido2 = Controles.Caja(820, 30, 150);
        private readonly ComboBox cmbPais = new ComboBox { Location = new Point(15, 90), Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox txtPerfil = Controles.Caja(285, 90, 685);
        private readonly TextBox txtBiografia = new TextBox { Location = new Point(15, 150), Size = new Size(640, 70), Multiline = true, ScrollBars = ScrollBars.Vertical };
        private readonly Button btnNuevo;
        private readonly Button btnGuardar;
        private readonly Button btnEstado;

        private List<Artista> _todos = new List<Artista>();
        private Artista? _seleccionado;

        public FrmArtistas()
        {
            Text = "Artistas";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(1000, 640);
            MinimumSize = new Size(1020, 600);

            Controles.Columna(grid, "Nombre artístico", nameof(Artista.NombreArtistico), 30);
            Controles.Columna(grid, "Nombre real", nameof(Artista.NombreReal), 35);
            Controles.Columna(grid, "País", nameof(Artista.NombrePais), 20);
            Controles.Columna(grid, "Estado", nameof(Artista.EstadoTexto), 15);
            grid.SelectionChanged += (s, e) => MostrarSeleccionado();

            var pnlBusqueda = new Panel { Dock = DockStyle.Top, Height = 42 };
            pnlBusqueda.Controls.Add(Controles.Etiqueta("Buscar:", 15, 13));
            pnlBusqueda.Controls.Add(txtBuscar);
            txtBuscar.TextChanged += (s, e) => Filtrar();

            btnNuevo = Controles.Boton("Nuevo", 670, 150, 90, Nuevo);
            btnGuardar = Controles.Boton("Guardar", 770, 150, 90, Guardar);
            btnEstado = Controles.Boton("Desactivar", 870, 150, 100, CambiarEstado);

            var pnlCampos = new Panel { Dock = DockStyle.Bottom, Height = 240 };
            pnlCampos.Controls.AddRange(new Control[]
            {
                Controles.Etiqueta("Nombre artístico *", 15, 10), txtNombreArtistico,
                Controles.Etiqueta("Primer nombre *", 285, 10), txtNombre1,
                Controles.Etiqueta("Segundo nombre", 470, 10), txtNombre2,
                Controles.Etiqueta("Primer apellido *", 655, 10), txtApellido1,
                Controles.Etiqueta("Segundo apellido", 820, 10), txtApellido2,
                Controles.Etiqueta("País de origen *", 15, 70), cmbPais,
                Controles.Etiqueta("Perfil o red social", 285, 70), txtPerfil,
                Controles.Etiqueta("Biografía", 15, 130), txtBiografia,
                btnNuevo, btnGuardar, btnEstado
            });

            if (!Sesion.EsAdministrador)
            {
                pnlCampos.Enabled = false;
                pnlBusqueda.Controls.Add(Controles.AvisoModoConsulta(420, 13));
            }

            // Se agrega primero el control que llena el espacio y luego los que se anclan arriba y abajo.
            Controls.Add(grid);
            Controls.Add(pnlCampos);
            Controls.Add(pnlBusqueda);

            Load += (s, e) => { CargarPaises(); Cargar(); };
        }

        private void CargarPaises()
        {
            Controles.Ejecutar(() =>
            {
                cmbPais.DisplayMember = nameof(Pais.Nombre);
                cmbPais.ValueMember = nameof(Pais.IdPais);
                cmbPais.DataSource = _negocio.ListarPaises();
                cmbPais.SelectedIndex = -1;
            });
        }

        private void Cargar()
        {
            Controles.Ejecutar(() =>
            {
                _todos = _negocio.Listar();
                Filtrar();
            });
        }

        private void Filtrar()
        {
            string texto = txtBuscar.Text.Trim();
            List<Artista> lista = texto.Length == 0
                ? _todos
                : _todos.Where(a => a.NombreArtistico.Contains(texto, StringComparison.OrdinalIgnoreCase)
                                 || a.NombreReal.Contains(texto, StringComparison.OrdinalIgnoreCase)).ToList();
            grid.DataSource = lista;
        }

        private void MostrarSeleccionado()
        {
            _seleccionado = grid.CurrentRow?.DataBoundItem as Artista;
            if (_seleccionado == null) return;

            txtNombreArtistico.Text = _seleccionado.NombreArtistico;
            txtNombre1.Text = _seleccionado.Nombre1Real;
            txtNombre2.Text = _seleccionado.Nombre2Real ?? "";
            txtApellido1.Text = _seleccionado.Apellido1Real;
            txtApellido2.Text = _seleccionado.Apellido2Real ?? "";
            txtPerfil.Text = _seleccionado.PerfilRedSocial ?? "";
            txtBiografia.Text = _seleccionado.Biografia ?? "";
            cmbPais.SelectedValue = _seleccionado.IdPais;
            btnEstado.Text = _seleccionado.Estado ? "Desactivar" : "Activar";
        }

        private void Nuevo()
        {
            grid.CurrentCell = null;
            _seleccionado = null;
            foreach (TextBox caja in new[] { txtNombreArtistico, txtNombre1, txtNombre2, txtApellido1, txtApellido2, txtPerfil, txtBiografia })
                caja.Clear();
            cmbPais.SelectedIndex = -1;
            btnEstado.Text = "Desactivar";
            txtNombreArtistico.Focus();
        }

        private void Guardar()
        {
            var artista = new Artista
            {
                IdArtista = _seleccionado?.IdArtista ?? "",
                Estado = _seleccionado?.Estado ?? true,
                NombreArtistico = txtNombreArtistico.Text,
                Nombre1Real = txtNombre1.Text,
                Nombre2Real = txtNombre2.Text,
                Apellido1Real = txtApellido1.Text,
                Apellido2Real = txtApellido2.Text,
                PerfilRedSocial = txtPerfil.Text,
                Biografia = txtBiografia.Text,
                IdPais = cmbPais.SelectedValue as string ?? ""
            };

            if (Controles.Ejecutar(() => _negocio.Guardar(artista)))
            {
                Controles.Aviso("Artista guardado correctamente.");
                Cargar();
                SeleccionarPorId(artista.IdArtista);
            }
        }

        private void CambiarEstado()
        {
            if (_seleccionado == null)
            {
                Controles.Aviso("Seleccione un artista de la lista.");
                return;
            }

            bool nuevoEstado = !_seleccionado.Estado;
            string accion = nuevoEstado ? "activar" : "desactivar";
            if (!Controles.Confirmar($"¿Desea {accion} a \"{_seleccionado.NombreArtistico}\"?")) return;

            string id = _seleccionado.IdArtista;
            if (Controles.Ejecutar(() => _negocio.CambiarEstado(id, nuevoEstado)))
            {
                Cargar();
                SeleccionarPorId(id);
            }
        }

        private void SeleccionarPorId(string id)
        {
            foreach (DataGridViewRow fila in grid.Rows)
            {
                if (fila.DataBoundItem is Artista a && a.IdArtista == id)
                {
                    grid.CurrentCell = fila.Cells[0];
                    fila.Selected = true;
                    return;
                }
            }
        }
    }
}
