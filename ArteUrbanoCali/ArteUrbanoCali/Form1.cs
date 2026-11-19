using ArteUrbanoCali.Datos;

namespace ArteUrbanoCali
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using var cn = Conexion.Crear();
                cn.Open();
                MessageBox.Show("Conexión exitosa. Versión de MySQL: " + cn.ServerVersion);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo conectar:\n" + ex.Message);
            }
        }
    }
}