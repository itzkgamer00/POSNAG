using System;
using System.Globalization;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class FrmSalida : Form
    {
        private readonly CN_Transaccion _negocio = new CN_Transaccion();
        private readonly AperturaCaja _apertura;

        public FrmSalida() : this(null)
        {
        }

        public FrmSalida(AperturaCaja apertura)
        {
            InitializeComponent();

            _apertura = apertura;

            CargarConceptos();
            CargarMonedas();
            CargarFormasPago();
        }

        private void CargarConceptos()
        {
            guna2ComboBox1.DataSource = _negocio.ListarConceptosEgreso();
            guna2ComboBox1.DisplayMember = nameof(Concepto.Nombre);
            guna2ComboBox1.ValueMember = nameof(Concepto.ConceptoId);
        }

        private void CargarMonedas()
        {
            guna2ComboBox2.DataSource = _negocio.ListarMonedasActivas();
            guna2ComboBox2.DisplayMember = nameof(Moneda.Nombre);
            guna2ComboBox2.ValueMember = nameof(Moneda.MonedaId);
        }

        private void CargarFormasPago()
        {
            guna2ComboBox3.DataSource = _negocio.ListarFormasPagoActivas();
            guna2ComboBox3.DisplayMember = nameof(FormaPago.Nombre);
            guna2ComboBox3.ValueMember = nameof(FormaPago.FormaPagoId);
        }

        private void btndetsalida_Click(object sender, EventArgs e)
        {
            FrmDetalle detalsal = new FrmDetalle(); // Crear una instancia del fmrcaja
            detalsal.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
            detalsal.ShowDialog(); // Mostrarlo como emergente
        }

        private void btnegredetalle_Click(object sender, EventArgs e)
        {
            FrmDetalle detal = new FrmDetalle(); // Crear una instancia del fmrcaja
            detal.StartPosition = FormStartPosition.CenterScreen; // Centrar el formulario emergente
            detal.ShowDialog(); // Mostrarlo como emergente
        }

        private void btnguardaringre_Click(object sender, EventArgs e)
        {
            if (_apertura == null)
            {
                MessageBox.Show("Debe abrir la caja antes de registrar un egreso.", "Egreso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (guna2ComboBox1.SelectedValue == null || guna2ComboBox2.SelectedValue == null)
            {
                MessageBox.Show("Seleccione la operacion y la moneda.", "Egreso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(guna2TextBox1.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal monto) || monto <= 0)
            {
                MessageBox.Show("Ingrese un monto valido (mayor que 0).", "Egreso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox1.Focus();
                return;
            }

            int conceptoId = Convert.ToInt32(guna2ComboBox1.SelectedValue);
            int monedaId = Convert.ToInt32(guna2ComboBox2.SelectedValue);
            int? formaPagoId = guna2ComboBox3.SelectedValue == null ? (int?)null : Convert.ToInt32(guna2ComboBox3.SelectedValue);

            try
            {
                _negocio.RegistrarEgreso(_apertura, SesionActual.Usuario.usuario_id, conceptoId, monedaId,
                    formaPagoId, monto, textBox3.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo registrar el egreso: " + ex.Message, "Egreso",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Egreso registrado correctamente.", "Egreso",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
