using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class Apertura : Form
    {
        private const int ColMonedaId = 0;
        private const int ColMoneda = 1;
        private const int ColMontoInicial = 2;

        private readonly CN_AperturaCaja _negocio = new CN_AperturaCaja();

        /// <summary>Apertura creada, disponible despues de aceptar el formulario.</summary>
        public AperturaCaja AperturaCreada { get; private set; }

        public Apertura()
        {
            InitializeComponent();

            txtUsuario.Text = SesionActual.Usuario?.NombreCompleto ?? string.Empty;
            txtFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");

            ConfigurarGrilla();
            CargarCajas();
            CargarMonedas();
        }

        private void ConfigurarGrilla()
        {
            dgvMontos.Columns.Add("colMonedaId", "Id");
            dgvMontos.Columns.Add("colMoneda", "Moneda");
            dgvMontos.Columns.Add("colMontoInicial", "Monto Inicial");

            dgvMontos.Columns[ColMonedaId].Visible = false;
            dgvMontos.Columns[ColMoneda].ReadOnly = true;
            dgvMontos.Columns[ColMoneda].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMontos.Columns[ColMontoInicial].Width = 180;
        }

        private void CargarCajas()
        {
            List<Caja> cajas = _negocio.ListarCajasActivas();
            cboCaja.DataSource = cajas;
            cboCaja.DisplayMember = nameof(Caja.Nombre);
            cboCaja.ValueMember = nameof(Caja.CajaId);
        }

        private void CargarMonedas()
        {
            List<Moneda> monedas = _negocio.ListarMonedasActivas();
            foreach (Moneda moneda in monedas)
            {
                string etiqueta = string.IsNullOrWhiteSpace(moneda.Simbolo)
                    ? $"{moneda.Nombre} ({moneda.Codigo})"
                    : $"{moneda.Nombre} ({moneda.Codigo}) {moneda.Simbolo}";

                dgvMontos.Rows.Add(moneda.MonedaId, etiqueta, "0.00");
            }
        }

        private void btnAbrirCaja_Click(object sender, EventArgs e)
        {
            if (cboCaja.SelectedValue == null)
            {
                MessageBox.Show("Seleccione una caja.", "Apertura de caja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var montos = new List<AperturaCajaMoneda>();
            foreach (DataGridViewRow fila in dgvMontos.Rows)
            {
                string texto = Convert.ToString(fila.Cells[ColMontoInicial].Value);
                if (!decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal monto) || monto < 0)
                {
                    MessageBox.Show($"Monto invalido para {fila.Cells[ColMoneda].Value}.", "Apertura de caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                montos.Add(new AperturaCajaMoneda
                {
                    MonedaId = Convert.ToInt32(fila.Cells[ColMonedaId].Value),
                    MontoInicial = monto
                });
            }

            int cajaId = Convert.ToInt32(cboCaja.SelectedValue);

            try
            {
                AperturaCreada = _negocio.Abrir(cajaId, SesionActual.Usuario.usuario_id, txtObservaciones.Text.Trim(), montos);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Apertura de caja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Apertura de caja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir la caja: " + ex.Message, "Apertura de caja",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelarApertura_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
