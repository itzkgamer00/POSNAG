using System;
using System.Collections.Generic;
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
            List<FormaPago> formasPago = _negocio.ListarFormasPagoActivas();
            guna2ComboBox3.DataSource = formasPago;
            guna2ComboBox3.DisplayMember = nameof(FormaPago.Nombre);
            guna2ComboBox3.ValueMember = nameof(FormaPago.FormaPagoId);

            FormaPago efectivo = formasPago.Find(f => f.Nombre == "Efectivo");
            if (efectivo != null)
                guna2ComboBox3.SelectedValue = efectivo.FormaPagoId;
        }

        private void btndetsalida_Click(object sender, EventArgs e)
        {
            FrmDetalle detalsal = new FrmDetalle(); // Crear una instancia del fmrcaja
            detalsal.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
            detalsal.ShowDialog(); // Mostrarlo como emergente
        }

        private void btnegredetalle_Click(object sender, EventArgs e)
        {
            using (FrmDetalle detal = new FrmDetalle())
            {
                detal.StartPosition = FormStartPosition.CenterScreen;
                if (detal.ShowDialog() == DialogResult.OK)
                    guna2TextBox1.Text = detal.Total.ToString("N2", CultureInfo.CurrentCulture);
            }
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

            Concepto conceptoSeleccionado = guna2ComboBox1.SelectedItem as Concepto;
            Moneda monedaSeleccionada = guna2ComboBox2.SelectedItem as Moneda;

            DialogResult confirmacion = MessageBox.Show(
                $"¿Confirma registrar el egreso?" + Environment.NewLine + Environment.NewLine +
                $"Concepto: {conceptoSeleccionado?.Nombre}" + Environment.NewLine +
                $"Monto: {monto.ToString("N2", CultureInfo.CurrentCulture)} {monedaSeleccionada?.Codigo}",
                "Confirmar egreso", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes) return;

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
