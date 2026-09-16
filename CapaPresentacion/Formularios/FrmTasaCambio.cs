using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class FrmTasaCambio : Form
    {
        /// <summary>Codigo de la moneda local: no tiene sentido registrarle una tasa de cambio (es siempre 1 a 1 consigo misma).</summary>
        private const string CodigoMonedaLocal = "NIO";

        private readonly CN_TasaCambio _negocio = new CN_TasaCambio();
        private readonly CN_Moneda _negocioMoneda = new CN_Moneda();

        /// <summary>Modo creacion: registra una tasa nueva (moneda y tipo en blanco).</summary>
        public FrmTasaCambio()
        {
            InitializeComponent();
            CargarMonedas();
            cboTipo.SelectedIndex = 0;
        }

        /// <summary>
        /// Modo actualizacion: preselecciona moneda, tipo y valor de una tasa existente. Al guardar se
        /// registra una tasa nueva para ese par (la anterior queda desactivada automaticamente).
        /// </summary>
        public FrmTasaCambio(TasaCambio tasa)
        {
            InitializeComponent();
            CargarMonedas();

            if (tasa == null) throw new ArgumentNullException(nameof(tasa));

            lblTitulo.Text = "Actualizar Tasa de Cambio";
            lblSubtitulo.Text = "Registre el nuevo valor. La tasa anterior quedara en el historial.";
            SeleccionarMonedaActual(tasa.MonedaId);
            cboTipo.SelectedItem = tasa.TipoOperacion;
            txtValor.Text = tasa.Valor.ToString("N4", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Solo las monedas extranjeras: registrar una tasa contra la propia moneda local (NIO) no tiene
        /// sentido y fue la causa de que una tasa quedara guardada para la moneda equivocada.
        /// </summary>
        private void CargarMonedas()
        {
            List<Moneda> monedas = _negocioMoneda.ListarActivas()
                .Where(m => m.Codigo != CodigoMonedaLocal)
                .ToList();
            cboMoneda.DataSource = monedas;
            cboMoneda.DisplayMember = nameof(Moneda.Codigo);
            cboMoneda.ValueMember = nameof(Moneda.MonedaId);
            cboMoneda.SelectedIndex = monedas.Count > 0 ? 0 : -1;
        }

        private void SeleccionarMonedaActual(int monedaId)
        {
            if (!(cboMoneda.DataSource is List<Moneda> monedas)) return;

            Moneda moneda = monedas.Find(m => m.MonedaId == monedaId);
            if (moneda != null)
                cboMoneda.SelectedValue = moneda.MonedaId;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cboMoneda.SelectedValue == null)
            {
                MessageBox.Show("Seleccione una moneda.", "Tasa de Cambio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtValor.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal valor))
            {
                MessageBox.Show("Ingrese un valor numerico valido.", "Tasa de Cambio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValor.Focus();
                return;
            }

            int monedaId = Convert.ToInt32(cboMoneda.SelectedValue);
            string tipoOperacion = cboTipo.SelectedItem as string;

            try
            {
                _negocio.RegistrarTasa(monedaId, tipoOperacion, valor);
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                MessageBox.Show(ex.Message, "Tasa de Cambio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo registrar la tasa: " + ex.Message, "Tasa de Cambio",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>Restringe el campo Valor a digitos y un unico separador decimal.</summary>
        private void txtValor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;

            char separador = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if (e.KeyChar == separador && txtValor.Text.IndexOf(separador) < 0)
                return;

            e.Handled = true;
        }
    }
}
