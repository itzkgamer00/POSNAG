using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using Guna.UI2.WinForms;

namespace CapaPresentacion.Formularios
{
    public partial class Cierre : Form
    {
        private const int AltoFila = 42;
        private const int AnchoMoneda = 320;
        private const int AnchoSistema = 150;
        private const int AnchoContado = 150;
        private const int AnchoDiferencia = 150;
        private const int AnchoDetalle = 100;

        private readonly CN_AperturaCaja _negocio = new CN_AperturaCaja();
        private readonly AperturaCaja _apertura;

        private class FilaCierre
        {
            public int MonedaId;
            public decimal MontoSistema;
            public TextBox Contado;
            public Label Diferencia;
        }

        private readonly List<FilaCierre> _filas = new List<FilaCierre>();

        /// <summary>Cierre creado, disponible despues de aceptar el formulario.</summary>
        public CierreCaja CierreCreado { get; private set; }

        public Cierre(AperturaCaja apertura)
        {
            InitializeComponent();

            _apertura = apertura ?? throw new ArgumentNullException(nameof(apertura));

            txtCaja.Text = apertura.CajaNombre;
            txtUsuario.Text = SesionActual.Usuario?.NombreCompleto ?? string.Empty;
            txtFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");

            CargarMontos();
        }

        /// <summary>Genera dinamicamente encabezado + una fila (Moneda, Monto Sistema, Monto Contado, Diferencia) por cada moneda de la apertura.</summary>
        private void CargarMontos()
        {
            Dictionary<int, decimal> montoSistema = _negocio.CalcularMontoSistema(_apertura.AperturaId);

            var fuenteEncabezado = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            pnlMontos.Controls.Add(new Label { Text = "Moneda", Font = fuenteEncabezado, Location = new System.Drawing.Point(8, 6), Size = new System.Drawing.Size(AnchoMoneda, 22) });
            pnlMontos.Controls.Add(new Label { Text = "Monto Sistema", Font = fuenteEncabezado, Location = new System.Drawing.Point(8 + AnchoMoneda, 6), Size = new System.Drawing.Size(AnchoSistema, 22), TextAlign = System.Drawing.ContentAlignment.MiddleRight });
            pnlMontos.Controls.Add(new Label { Text = "Monto Contado", Font = fuenteEncabezado, Location = new System.Drawing.Point(8 + AnchoMoneda + AnchoSistema + 16, 6), Size = new System.Drawing.Size(AnchoContado, 22), TextAlign = System.Drawing.ContentAlignment.MiddleRight });
            pnlMontos.Controls.Add(new Label { Text = "Diferencia", Font = fuenteEncabezado, Location = new System.Drawing.Point(8 + AnchoMoneda + AnchoSistema + AnchoContado + 24, 6), Size = new System.Drawing.Size(AnchoDiferencia, 22), TextAlign = System.Drawing.ContentAlignment.MiddleRight });
            pnlMontos.Controls.Add(new Label { Text = "Detalle", Font = fuenteEncabezado, Location = new System.Drawing.Point(8 + AnchoMoneda + AnchoSistema + AnchoContado + AnchoDiferencia + 40, 6), Size = new System.Drawing.Size(AnchoDetalle, 22), TextAlign = System.Drawing.ContentAlignment.MiddleCenter });

            int y = 32;
            foreach (AperturaCajaMoneda monto in _apertura.Montos)
            {
                string etiqueta = string.IsNullOrWhiteSpace(monto.MonedaSimbolo)
                    ? $"{monto.MonedaNombre} ({monto.MonedaCodigo})"
                    : $"{monto.MonedaNombre} ({monto.MonedaCodigo}) {monto.MonedaSimbolo}";

                decimal sistema = montoSistema.TryGetValue(monto.MonedaId, out decimal valor) ? valor : monto.MontoInicial;

                var lblMoneda = new Label { Text = etiqueta, Location = new System.Drawing.Point(8, y + 4), Size = new System.Drawing.Size(AnchoMoneda, 24) };

                var lblSistema = new Label
                {
                    Text = sistema.ToString("N2", CultureInfo.CurrentCulture),
                    Location = new System.Drawing.Point(8 + AnchoMoneda, y + 4),
                    Size = new System.Drawing.Size(AnchoSistema, 24),
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight
                };

                var txtContado = new TextBox
                {
                    Text = "0.00",
                    Location = new System.Drawing.Point(8 + AnchoMoneda + AnchoSistema + 16, y),
                    Size = new System.Drawing.Size(AnchoContado, 28),
                    TextAlign = HorizontalAlignment.Right
                };

                var lblDiferencia = new Label
                {
                    Text = "0.00",
                    Location = new System.Drawing.Point(8 + AnchoMoneda + AnchoSistema + AnchoContado + 24, y + 4),
                    Size = new System.Drawing.Size(AnchoDiferencia, 24),
                    TextAlign = System.Drawing.ContentAlignment.MiddleRight,
                    ForeColor = System.Drawing.Color.FromArgb(185, 51, 73)
                };

                var btnDetalle = new Guna2Button
                {
                    Text = "Detallar",
                    Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold),
                    ForeColor = System.Drawing.Color.White,
                    FillColor = System.Drawing.Color.FromArgb(37, 99, 235),
                    BorderRadius = 6,
                    Location = new System.Drawing.Point(8 + AnchoMoneda + AnchoSistema + AnchoContado + AnchoDiferencia + 40, y),
                    Size = new System.Drawing.Size(AnchoDetalle, 28)
                };

                var fila = new FilaCierre { MonedaId = monto.MonedaId, MontoSistema = sistema, Contado = txtContado, Diferencia = lblDiferencia };
                txtContado.KeyPress += MontoTextBox_KeyPress;
                txtContado.TextChanged += (s, e) => ActualizarDiferencia(fila);
                btnDetalle.Click += (s, e) => AbrirDetalleConteo(fila);

                pnlMontos.Controls.Add(lblMoneda);
                pnlMontos.Controls.Add(lblSistema);
                pnlMontos.Controls.Add(txtContado);
                pnlMontos.Controls.Add(lblDiferencia);
                pnlMontos.Controls.Add(btnDetalle);
                _filas.Add(fila);

                y += AltoFila;
            }
        }

        /// <summary>Abre el contador de efectivo por denominacion y vuelca el total contado en el Monto Contado de esa fila.</summary>
        private void AbrirDetalleConteo(FilaCierre fila)
        {
            using (FrmDetalle detalle = new FrmDetalle())
            {
                detalle.StartPosition = FormStartPosition.CenterScreen;
                if (detalle.ShowDialog() == DialogResult.OK)
                    fila.Contado.Text = detalle.Total.ToString("N2", CultureInfo.CurrentCulture);
            }
        }

        /// <summary>Restringe el monto contado a digitos y un unico separador decimal.</summary>
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

        private void ActualizarDiferencia(FilaCierre fila)
        {
            decimal.TryParse(fila.Contado.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal contado);

            decimal diferencia = contado - fila.MontoSistema;
            fila.Diferencia.Text = diferencia.ToString("N2", CultureInfo.CurrentCulture);
            fila.Diferencia.ForeColor = diferencia < 0
                ? System.Drawing.Color.FromArgb(185, 51, 73)
                : System.Drawing.Color.FromArgb(5, 150, 105);
        }

        private void btnCerrarCaja_Click(object sender, EventArgs e)
        {
            var montosFinales = new Dictionary<int, decimal>();
            foreach (FilaCierre fila in _filas)
            {
                if (!decimal.TryParse(fila.Contado.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal contado) || contado < 0)
                {
                    MessageBox.Show("Ingrese un monto contado valido (mayor o igual a 0) para cada moneda.", "Cierre de caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    fila.Contado.Focus();
                    return;
                }

                montosFinales[fila.MonedaId] = contado;
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

            DialogResult deseaImprimir = MessageBox.Show(
                "Caja cerrada correctamente." + Environment.NewLine + Environment.NewLine +
                "¿Desea imprimir el tiquet de cierre?", "Cierre de caja",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (deseaImprimir == DialogResult.Yes)
                ImprimirCierre();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelarCierre_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>Exporta el conteo final (sistema, contado, diferencia) de cada moneda y las observaciones a un CSV que Excel abre como hoja de calculo.</summary>
        private void btnExportarExcel_Click(object sender, EventArgs e)
        {
            var encabezados = new[] { "Moneda", "Monto Sistema", "Monto Contado", "Diferencia" };
            var filas = new List<string[]>();

            foreach (FilaCierre fila in _filas)
            {
                decimal.TryParse(fila.Contado.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal contado);
                decimal diferencia = contado - fila.MontoSistema;

                AperturaCajaMoneda monto = _apertura.Montos.Find(m => m.MonedaId == fila.MonedaId);
                string etiqueta = monto != null ? $"{monto.MonedaNombre} ({monto.MonedaCodigo})" : fila.MonedaId.ToString();

                filas.Add(new[]
                {
                    etiqueta,
                    fila.MontoSistema.ToString("N2", CultureInfo.CurrentCulture),
                    contado.ToString("N2", CultureInfo.CurrentCulture),
                    diferencia.ToString("N2", CultureInfo.CurrentCulture)
                });
            }

            var notas = new List<string>
            {
                $"Caja: {txtCaja.Text}",
                $"Usuario: {txtUsuario.Text}",
                $"Fecha: {txtFechaHora.Text}",
                "Observaciones:",
                string.IsNullOrWhiteSpace(txtObservaciones.Text) ? "(Sin observaciones)" : txtObservaciones.Text
            };

            ExportadorCsv.Exportar(this, encabezados, filas, "CierreCaja", notas);
        }

        /// <summary>Imprime un tiquet con el conteo final (sistema, contado, diferencia) de cada moneda y las observaciones del cierre, dejando elegir la impresora (o cancelar).</summary>
        private void ImprimirCierre()
        {
            using (var documento = new System.Drawing.Printing.PrintDocument())
            {
                ConfiguracionImpresora.Aplicar(documento);
                documento.PrintPage += (s, e) => DibujarTiqueteCierre(e);

                if (ConfiguracionImpresora.MostrarDialogoImpresion)
                {
                    using (var dialogoImpresion = new PrintDialog { Document = documento, AllowSomePages = false, AllowSelection = false, AllowPrintToFile = false })
                    {
                        if (dialogoImpresion.ShowDialog(this) != DialogResult.OK) return;
                    }
                }

                try
                {
                    documento.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo imprimir el cierre: " + ex.Message, "Cierre de caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DibujarTiqueteCierre(System.Drawing.Printing.PrintPageEventArgs e)
        {
            System.Drawing.Graphics g = e.Graphics;
            float anchoTiquete = e.MarginBounds.Width;
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;

            var fontTitulo = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            var fontTexto = new System.Drawing.Font("Consolas", 9.5F);
            var fontChico = new System.Drawing.Font("Consolas", 8F);
            var centrado = new System.Drawing.StringFormat { Alignment = System.Drawing.StringAlignment.Center };
            float anchoGuion = g.MeasureString("-", fontChico, int.MaxValue, System.Drawing.StringFormat.GenericTypographic).Width;
            int cantidadGuiones = anchoGuion > 0 ? Math.Max(1, (int)(anchoTiquete / anchoGuion)) : 34;
            string separador = new string('-', cantidadGuiones);

            void Escribir(string texto, System.Drawing.Font fuente, System.Drawing.StringFormat formato = null)
            {
                System.Drawing.SizeF tamanio = g.MeasureString(texto, fuente, (int)anchoTiquete,
                    formato ?? System.Drawing.StringFormat.GenericDefault);
                float alto = tamanio.Height + 6;
                g.DrawString(texto, fuente, System.Drawing.Brushes.Black,
                    new System.Drawing.RectangleF(x, y, anchoTiquete, alto), formato);
                y += alto;
            }

            Escribir("SISTEMA DE CAJA", fontTitulo, centrado);
            Escribir("Cierre de Caja", fontTexto, centrado);
            Escribir(separador, fontChico);
            Escribir($"Caja: {txtCaja.Text}", fontChico);
            Escribir($"Usuario: {txtUsuario.Text}", fontChico);
            Escribir($"Fecha: {txtFechaHora.Text}", fontChico);
            Escribir(separador, fontChico);

            foreach (FilaCierre fila in _filas)
            {
                decimal.TryParse(fila.Contado.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal contado);
                decimal diferencia = contado - fila.MontoSistema;

                AperturaCajaMoneda monto = _apertura.Montos.Find(m => m.MonedaId == fila.MonedaId);
                string etiqueta = monto != null ? $"{monto.MonedaNombre} ({monto.MonedaCodigo})" : fila.MonedaId.ToString();

                Escribir(etiqueta, fontTexto);
                Escribir($"  Sistema:    {fila.MontoSistema.ToString("N2", CultureInfo.CurrentCulture)}", fontTexto);
                Escribir($"  Contado:    {contado.ToString("N2", CultureInfo.CurrentCulture)}", fontTexto);
                Escribir($"  Diferencia: {diferencia.ToString("N2", CultureInfo.CurrentCulture)}", fontTexto);
            }

            Escribir(separador, fontChico);
            Escribir("Observaciones:", fontTexto);
            Escribir(string.IsNullOrWhiteSpace(txtObservaciones.Text) ? "(Sin observaciones)" : txtObservaciones.Text, fontTexto);

            e.HasMorePages = false;
        }
    }
}
