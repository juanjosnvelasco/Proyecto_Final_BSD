using ArteUrbanoCali.Entidades;
using ArteUrbanoCali.Negocio;

namespace ArteUrbanoCali.UI
{
    public class FrmCategorias : Form
    {
        private readonly CategoriaNegocio _negocio = new CategoriaNegocio();
        private readonly DataGridView grid = Controles.Tabla();
        private readonly TextBox txtBuscar = Controles.Caja(70, 10, 300);
        private readonly TextBox txtNombre = Controles.Caja(15, 30, 260);
        private readonly TextBox txtDescripcion = new TextBox { Location = new Point(295, 30), Size = new Size(450, 60), Multiline = true, ScrollBars = ScrollBars.Vertical };
        private readonly Button btnNuevo;
        private readonly Button btnGuardar;
        private readonly Button btnEstado;

        private List<Categoria> _todas = new List<Categoria>();
        private Categoria? _seleccionada;

        public FrmCategorias()
        {
            Text = "Categorías";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(780, 520);
            MinimumSize = new Size(800, 480);

            Controles.Columna(grid, "Nombre", nameof(Categoria.Nombre), 30);
            Controles.Columna(grid, "Descripción", nameof(Categoria.Descripcion), 55);
            Controles.Columna(grid, "Estado", nameof(Categoria.EstadoTexto), 15);
            grid.SelectionChanged += (s, e) => MostrarSeleccionada();

            var pnlBusqueda = new Panel { Dock = DockStyle.Top, Height = 42 };
            pnlBusqueda.Controls.Add(Controles.Etiqueta("Buscar:", 15, 13));
            pnlBusqueda.Controls.Add(txtBuscar);
            txtBuscar.TextChanged += (s, e) => Filtrar();

            btnNuevo = Controles.Boton("Nuevo", 15, 110, 90, Nuevo);
            btnGuardar = Controles.Boton("Guardar", 115, 110, 90, Guardar);
            btnEstado = Controles.Boton("Desactivar", 215, 110, 100, CambiarEstado);

            var pnlCampos = new Panel { Dock = DockStyle.Bottom, Height = 165 };
            pnlCampos.Controls.AddRange(new Control[]
            {
                Controles.Etiqueta("Nombre *", 15, 10), txtNombre,
                Controles.Etiqueta("Descripción", 295, 10), txtDescripcion,
                btnNuevo, btnGuardar, btnEstado
            });

            if (!Sesion.EsAdministrador)
            {
                pnlCampos.Enabled = false;
                pnlBusqueda.Controls.Add(Controles.AvisoModoConsulta(420, 13));
            }

            Controls.Add(grid);
            Controls.Add(pnlCampos);
            Controls.Add(pnlBusqueda);

            Load += (s, e) => Cargar();
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
            List<Categoria> lista = texto.Length == 0
                ? _todas
                : _todas.Where(c => c.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase)).ToList();
            grid.DataSource = lista;
        }

        private void MostrarSeleccionada()
        {
            _seleccionada = grid.CurrentRow?.DataBoundItem as Categoria;
            if (_seleccionada == null) return;

            txtNombre.Text = _seleccionada.Nombre;
            txtDescripcion.Text = _seleccionada.Descripcion ?? "";
            btnEstado.Text = _seleccionada.Estado ? "Desactivar" : "Activar";
        }

        private void Nuevo()
        {
            grid.CurrentCell = null;
            _seleccionada = null;
            txtNombre.Clear();
            txtDescripcion.Clear();
            btnEstado.Text = "Desactivar";
            txtNombre.Focus();
        }

        private void Guardar()
        {
            var categoria = new Categoria
            {
                IdCategoria = _seleccionada?.IdCategoria ?? "",
                Estado = _seleccionada?.Estado ?? true,
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text
            };

            if (Controles.Ejecutar(() => _negocio.Guardar(categoria)))
            {
                Controles.Aviso("Categoría guardada correctamente.");
                Cargar();
                SeleccionarPorId(categoria.IdCategoria);
            }
        }

        private void CambiarEstado()
        {
            if (_seleccionada == null)
            {
                Controles.Aviso("Seleccione una categoría de la lista.");
                return;
            }

            bool nuevoEstado = !_seleccionada.Estado;
            string accion = nuevoEstado ? "activar" : "desactivar";
            if (!Controles.Confirmar($"¿Desea {accion} la categoría \"{_seleccionada.Nombre}\"?")) return;

            string id = _seleccionada.IdCategoria;
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
                if (fila.DataBoundItem is Categoria c && c.IdCategoria == id)
                {
                    grid.CurrentCell = fila.Cells[0];
                    fila.Selected = true;
                    return;
                }
            }
        }
    }
}
