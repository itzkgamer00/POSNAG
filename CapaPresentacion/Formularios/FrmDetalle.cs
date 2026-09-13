using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

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
