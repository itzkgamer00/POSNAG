using System;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class Frmclientes : Form
    {
        private readonly CN_Cliente _negocioCliente = new CN_Cliente();

        /// <summary>Cliente que quedo registrado al guardar. Null si el formulario se cerro sin guardar.</summary>
        public Cliente ClienteRegistrado { get; private set; }

        public Frmclientes() : this(null, null)
        {
        }

        /// <summary>Permite abrir el formulario con el Tipo y N° de Identificacion ya escritos (p.ej. desde Mesa de Cambio).</summary>
        public Frmclientes(string tipoIdentificacionSugerido, string numeroIdentificacionSugerido)
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(tipoIdentificacionSugerido))
                guna2ComboBox1.SelectedItem = tipoIdentificacionSugerido;

            if (!string.IsNullOrWhiteSpace(numeroIdentificacionSugerido))
                guna2TextBox2.Text = numeroIdentificacionSugerido;

            btnguardarcliente.Click += btnguardarcliente_Click;
            btncancelcliente.Click += btncancelcliente_Click;
        }

        private void btnguardarcliente_Click(object sender, EventArgs e)
        {
            string tipoIdentificacion = guna2ComboBox1.SelectedItem as string;
            string numeroIdentificacion = guna2TextBox2.Text.Trim();
            string nombres = guna2TextBox1.Text.Trim();
            string apellidos = guna2TextBox5.Text.Trim();
            string telefono = guna2TextBox3.Text.Trim();
            string direccion = guna2TextBox4.Text.Trim();
            string correo = guna2TextBox6.Text.Trim();

            if (string.IsNullOrWhiteSpace(tipoIdentificacion))
            {
                MessageBox.Show("Seleccione el tipo de identificacion.", "Nuevo Cliente",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ClienteRegistrado = _negocioCliente.RegistrarCliente(tipoIdentificacion, numeroIdentificacion,
                    nombres, apellidos, telefono, direccion, correo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nuevo Cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("Cliente registrado correctamente.", "Nuevo Cliente",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btncancelcliente_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
