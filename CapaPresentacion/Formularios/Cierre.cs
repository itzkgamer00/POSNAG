using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class Cierre : Form
    {
        private const int ColMonedaId = 0;
        private const int ColMoneda = 1;
        private const int ColMontoSistema = 2;
        private const int ColMontoContado = 3;
        private const int ColDiferencia = 4;

        private readonly CN_AperturaCaja _negocio = new CN_AperturaCaja();
        private readonly AperturaCaja _apertura;

        /// <summary>Cierre creado, disponible despues de aceptar el formulario.</summary>
        public CierreCaja CierreCreado { get; private set; }

        public Cierre(AperturaCaja apertura)
        {
            InitializeComponent();

            _apertura = apertura ?? throw new ArgumentNullException(nameof(apertura));

            txtCaja.Text = apertura.CajaNombre;
            txtUsuario.Text = SesionActual.Usuario?.NombreCompleto ?? string.Empty;
            txtFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");

            ConfigurarGrilla();
            CargarMontos();
        }

        private void ConfigurarGrilla()
        {
            dgvMontos.Columns.Add("colMonedaId", "Id");
            dgvMontos.Columns.Add("colMoneda", "Moneda");
            dgvMontos.Columns.Add("colMontoSistema", "Monto Sistema");
            dgvMontos.Columns.Add("colMontoContado", "Monto Contado");
            dgvMontos.Columns.Add("colDiferencia", "Diferencia");

            dgvMontos.Columns[ColMonedaId].Visible = false;
            dgvMontos.Columns[ColMoneda].ReadOnly = true;
            dgvMontos.Columns[ColMoneda].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMontos.Columns[ColMontoSistema].ReadOnly = true;
            dgvMontos.Columns[ColMontoSistema].Width = 150;
            dgvMontos.Columns[ColMontoContado].Width = 150;
            dgvMontos.Columns[ColDiferencia].ReadOnly = true;
            dgvMontos.Columns[ColDiferencia].Width = 150;

            dgvMontos.CellValueChanged += DgvMontos_CellValueChanged;
            dgvMontos.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvMontos.IsCurrentCellDirty)
                    dgvMontos.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
        }

        private void CargarMontos()
        {
            Dictionary<int, decimal> montoSistema = _negocio.CalcularMontoSistema(_apertura.AperturaId);

            foreach (AperturaCajaMoneda monto in _apertura.Montos)
            {
                string etiqueta = string.IsNullOrWhiteSpace(monto.MonedaSimbolo)
                    ? $"{monto.MonedaNombre} ({monto.MonedaCodigo})"
                    : $"{monto.MonedaNombre} ({monto.MonedaCodigo}) {monto.MonedaSimbolo}";

                decimal sistema = montoSistema.TryGetValue(monto.MonedaId, out decimal valor) ? valor : monto.MontoInicial;

                dgvMontos.Rows.Add(monto.MonedaId, etiqueta, sistema.ToString("N2", CultureInfo.CurrentCulture), "0.00", "0.00");
            }
        }

        private void DgvMontos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != ColMontoContado) return;
            ActualizarDiferencia(dgvMontos.Rows[e.RowIndex]);
        }

        private void ActualizarDiferencia(DataGridViewRow fila)
        {
            decimal.TryParse(Convert.ToString(fila.Cells[ColMontoSistema].Value), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal sistema);
            decimal.TryParse(Convert.ToString(fila.Cells[ColMontoContado].Value), NumberStyles.Number, CultureInfo.CurrentCulture, out decimal contado);

            decimal diferencia = contado - sistema;
            fila.Cells[ColDiferencia].Value = diferencia.ToString("N2", CultureInfo.CurrentCulture);
            fila.Cells[ColDiferencia].Style.ForeColor = diferencia < 0
                ? System.Drawing.Color.FromArgb(185, 51, 73)
                : System.Drawing.Color.FromArgb(5, 150, 105);
        }

        private void btnCerrarCaja_Click(object sender, EventArgs e)
        {
            var montosFinales = new Dictionary<int, decimal>();
            foreach (DataGridViewRow fila in dgvMontos.Rows)
            {
                string texto = Convert.ToString(fila.Cells[ColMontoContado].Value);
                if (!decimal.TryParse(texto, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal contado) || contado < 0)
                {
                    MessageBox.Show($"Monto contado invalido para {fila.Cells[ColMoneda].Value}.", "Cierre de caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                montosFinales[Convert.ToInt32(fila.Cells[ColMonedaId].Value)] = contado;
            }

            try
            {
                CierreCreado = _negocio.Cerrar(_apertura.AperturaId, SesionActual.Usuario.usuario_id,
                    txtObservaciones.Text.Trim(), montosFinales);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Cierre de caja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cerrar la caja: " + ex.Message, "Cierre de caja",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelarCierre_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
