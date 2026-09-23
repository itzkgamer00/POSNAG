using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using CapaNegocio;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion
{
    public partial class FrmDetalle : Form
    {
        private class FilaDenominacion
        {
            public decimal Valor;
            public TextBox Subtotal;
        }

        /// <summary>Mapa Cantidad -> (Denominacion, Subtotal) para las 13 filas del contador de efectivo.</summary>
        private readonly Dictionary<TextBox, FilaDenominacion> _filas;
        private readonly List<TextBox> _subtotales;

        /// <summary>Suma de todos los subtotales (Cantidad x Denominacion), recalculada en vivo.</summary>
        public decimal Total { get; private set; }

        public FrmDetalle()
        {
            InitializeComponent();

            _filas = new Dictionary<TextBox, FilaDenominacion>
            {
                { textBox1,  new FilaDenominacion { Valor = 1000.00m, Subtotal = textBox20 } },
                { textBox2,  new FilaDenominacion { Valor = 500.00m,  Subtotal = textBox19 } },
                { textBox3,  new FilaDenominacion { Valor = 200.00m,  Subtotal = textBox18 } },
                { textBox4,  new FilaDenominacion { Valor = 100.00m,  Subtotal = textBox17 } },
                { textBox5,  new FilaDenominacion { Valor = 50.00m,   Subtotal = textBox16 } },
                { textBox6,  new FilaDenominacion { Valor = 20.00m,   Subtotal = textBox15 } },
                { textBox7,  new FilaDenominacion { Valor = 10.00m,   Subtotal = textBox14 } },
                { textBox8,  new FilaDenominacion { Valor = 5.00m,    Subtotal = textBox13 } },
                { textBox9,  new FilaDenominacion { Valor = 1.00m,    Subtotal = textBox12 } },
                { textBox10, new FilaDenominacion { Valor = 0.50m,    Subtotal = textBox26 } },
                { textBox11, new FilaDenominacion { Valor = 0.25m,    Subtotal = textBox27 } },
                { textBox22, new FilaDenominacion { Valor = 0.05m,    Subtotal = textBox24 } },
                { textBox21, new FilaDenominacion { Valor = 0.01m,    Subtotal = textBox23 } },
            };

            _subtotales = new List<TextBox>();
            foreach (KeyValuePair<TextBox, FilaDenominacion> par in _filas)
            {
                TextBox cantidad = par.Key;
                TextBox subtotal = par.Value.Subtotal;

                cantidad.Text = "0";
                cantidad.KeyPress += Cantidad_KeyPress;
                cantidad.TextChanged += Cantidad_TextChanged;

                subtotal.ReadOnly = true;
                subtotal.TabStop = false;
                subtotal.Text = "0.00";

                _subtotales.Add(subtotal);
            }

            textBox29.ReadOnly = true;
            textBox29.TabStop = false;
            textBox29.Text = "0.00";
        }

        /// <summary>La cantidad de billetes/monedas es un entero: solo digitos, sin letras ni decimales.</summary>
        private void Cantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) return;
            e.Handled = true;
        }

        /// <summary>Recalcula el subtotal de la fila (Cantidad x Denominacion) y el total general.</summary>
        private void Cantidad_TextChanged(object sender, System.EventArgs e)
        {
            var cantidadBox = (TextBox)sender;
            if (!_filas.TryGetValue(cantidadBox, out FilaDenominacion fila)) return;

            int cantidad = int.TryParse(cantidadBox.Text, out int valor) ? valor : 0;
            decimal subtotal = cantidad * fila.Valor;
            fila.Subtotal.Text = subtotal.ToString("N2", CultureInfo.CurrentCulture);

            RecalcularTotal();
        }

        private void iconButton1_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>Boton "Imprimir Detalle": imprime el desglose por denominacion del efectivo contado/entregado.</summary>
        private void btnImprimirDetalle_Click(object sender, EventArgs e)
        {
            bool hayCantidades = false;
            foreach (TextBox cantidad in _filas.Keys)
            {
                if (int.TryParse(cantidad.Text, out int valor) && valor > 0) { hayCantidades = true; break; }
            }

            if (!hayCantidades)
            {
                MessageBox.Show("No hay cantidades cargadas para imprimir.", "Imprimir Detalle",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ImprimirDetalleConteo();
        }

        /// <summary>Imprime el desglose por denominacion (cantidad x denominacion = subtotal), dejando elegir la impresora (o cancelar).</summary>
        private void ImprimirDetalleConteo()
        {
            using (var documento = new System.Drawing.Printing.PrintDocument())
            {
                ConfiguracionImpresora.Aplicar(documento);
                documento.PrintPage += (s, e) => DibujarDetalleConteo(e);

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
                    MessageBox.Show("No se pudo imprimir el detalle: " + ex.Message, "Imprimir Detalle",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DibujarDetalleConteo(System.Drawing.Printing.PrintPageEventArgs e)
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
            Escribir("Detalle de Efectivo Entregado", fontTexto, centrado);
            Escribir(separador, fontChico);
            Escribir($"Fecha: {DateTime.Now:dd/MM/yyyy hh:mm tt}", fontChico);
            Escribir($"Cajero: {SesionActual.Usuario?.NombreCompleto}", fontChico);
            Escribir(separador, fontChico);

            foreach (KeyValuePair<TextBox, FilaDenominacion> par in _filas)
            {
                int cantidad = int.TryParse(par.Key.Text, out int c) ? c : 0;
                if (cantidad <= 0) continue;

                decimal subtotal = cantidad * par.Value.Valor;
                Escribir($"{cantidad} x {par.Value.Valor.ToString("N2", CultureInfo.CurrentCulture)}  =  {subtotal.ToString("N2", CultureInfo.CurrentCulture)}", fontTexto);
            }

            Escribir(separador, fontChico);
            Escribir($"TOTAL: {Total.ToString("N2", CultureInfo.CurrentCulture)}", fontTitulo);

            e.HasMorePages = false;
        }

        private void RecalcularTotal()
        {
            decimal total = 0m;
            foreach (TextBox subtotal in _subtotales)
            {
                if (decimal.TryParse(subtotal.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal parcial))
                    total += parcial;
            }

            Total = total;
            textBox29.Text = total.ToString("N2", CultureInfo.CurrentCulture);
        }
    }
}
