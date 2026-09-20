using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using Guna.UI2.WinForms;

namespace CapaPresentacion.Formularios
{
    public partial class Apertura : Form
    {
        private const int AltoFila = 42;
        private const int AnchoEtiqueta = 380;
        private const int AnchoMonto = 180;
        private const int AnchoDetalle = 110;

        private readonly CN_AperturaCaja _negocio = new CN_AperturaCaja();

        /// <summary>Mapa MonedaId -> TextBox donde el usuario digita el monto inicial de esa moneda.</summary>
        private readonly Dictionary<int, TextBox> _montoPorMoneda = new Dictionary<int, TextBox>();

        /// <summary>Apertura creada, disponible despues de aceptar el formulario.</summary>
        public AperturaCaja AperturaCreada { get; private set; }

        public Apertura()
        {
            InitializeComponent();

            txtUsuario.Text = SesionActual.Usuario?.NombreCompleto ?? string.Empty;
            txtFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");

            CargarCajas();
            CargarMonedas();
        }

        private void CargarCajas()
        {
            List<Caja> cajas = _negocio.ListarCajasActivas();
            cboCaja.DataSource = cajas;
            cboCaja.DisplayMember = nameof(Caja.Nombre);
            cboCaja.ValueMember = nameof(Caja.CajaId);
        }

        /// <summary>Genera dinamicamente una fila Etiqueta + TextBox por cada moneda activa dentro de pnlMontos.</summary>
        private void CargarMonedas()
        {
            List<Moneda> monedas = _negocio.ListarMonedasActivas();
            int y = 8;

            foreach (Moneda moneda in monedas)
            {
                string etiqueta = string.IsNullOrWhiteSpace(moneda.Simbolo)
                    ? $"{moneda.Nombre} ({moneda.Codigo})"
                    : $"{moneda.Nombre} ({moneda.Codigo}) {moneda.Simbolo}";

                var lbl = new Label
                {
                    Text = etiqueta,
                    Location = new System.Drawing.Point(8, y + 4),
                    Size = new System.Drawing.Size(AnchoEtiqueta, 24),
                    Font = new System.Drawing.Font("Segoe UI", 11F)
                };

                var txt = new TextBox
                {
                    Text = "0.00",
                    Location = new System.Drawing.Point(AnchoEtiqueta + 16, y),
                    Size = new System.Drawing.Size(AnchoMonto, 28),
                    TextAlign = HorizontalAlignment.Right,
                    Font = new System.Drawing.Font("Segoe UI", 11F)
                };
                txt.KeyPress += MontoTextBox_KeyPress;

                var btnDetalle = new Guna2Button
                {
                    Text = "Detallar",
                    Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
                    ForeColor = System.Drawing.Color.White,
                    FillColor = System.Drawing.Color.FromArgb(37, 99, 235),
                    BorderRadius = 6,
                    Location = new System.Drawing.Point(AnchoEtiqueta + 16 + AnchoMonto + 16, y),
                    Size = new System.Drawing.Size(AnchoDetalle, 28)
                };
                btnDetalle.Click += (s, e) => AbrirDetalleConteo(txt);

                pnlMontos.Controls.Add(lbl);
                pnlMontos.Controls.Add(txt);
                pnlMontos.Controls.Add(btnDetalle);
                _montoPorMoneda[moneda.MonedaId] = txt;

                y += AltoFila;
            }
        }

        /// <summary>Abre el contador de efectivo por denominacion y vuelca el total contado en el monto inicial de esa moneda.</summary>
        private void AbrirDetalleConteo(TextBox destino)
        {
            using (FrmDetalle detalle = new FrmDetalle())
            {
                detalle.StartPosition = FormStartPosition.CenterScreen;
                if (detalle.ShowDialog() == DialogResult.OK)
                    destino.Text = detalle.Total.ToString("N2", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>Restringe el monto a digitos y un unico separador decimal.</summary>
        private void MontoTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (char.IsDigit(e.KeyChar)) return;

            var textBox = (TextBox)sender;
            char separador = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if (e.KeyChar == separador && textBox.Text.IndexOf(separador) < 0)
                return;

            e.Handled = true;
        }

        private void btnAbrirCaja_Click(object sender, EventArgs e)
        {
            if (cboCaja.SelectedValue == null)
            {
                MessageBox.Show("Seleccione una caja.", "Apertura de caja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var montos = new List<AperturaCajaMoneda>();
            foreach (KeyValuePair<int, TextBox> par in _montoPorMoneda)
            {
                if (!decimal.TryParse(par.Value.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal monto) || monto < 0)
                {
                    MessageBox.Show("Ingrese un monto inicial valido (mayor o igual a 0) para cada moneda.", "Apertura de caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    par.Value.Focus();
                    return;
                }

                montos.Add(new AperturaCajaMoneda
                {
                    MonedaId = par.Key,
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
