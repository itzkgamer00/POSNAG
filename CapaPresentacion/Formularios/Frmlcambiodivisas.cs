using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Formularios;
using CapaPresentacion.Utilidades;

namespace CapaPresentacion
{
    public partial class Frmlcambiodivisas : Form
    {
        /// <summary>Codigo de la divisa extranjera que maneja el sistema (la otra siempre es NIO, la moneda local).</summary>
        private const string CodigoMonedaExtranjera = "USD";
        private const string CodigoMonedaLocal = "NIO";

        private readonly CN_Transaccion _negocio = new CN_Transaccion();
        private readonly CN_TasaCambio _negocioTasa = new CN_TasaCambio();
        private readonly CN_Cliente _negocioCliente = new CN_Cliente();
        private readonly AperturaCaja _apertura;

        /// <summary>Evita que el recalculo cruzado entre Monto Recibido y Monto Entregar se dispare a si mismo.</summary>
        private bool _actualizandoMontos;

        private class FilaDenominacion
        {
            public decimal Valor;
            public TextBox Subtotal;
        }

        /// <summary>Mapa Cantidad -> (Denominacion, Subtotal) de las 13 filas del panel "Detalle".</summary>
        private readonly Dictionary<TextBox, FilaDenominacion> _filasDetalle = new Dictionary<TextBox, FilaDenominacion>();
        private readonly List<TextBox> _subtotalesDetalle = new List<TextBox>();

        public Frmlcambiodivisas() : this(null)
        {
        }

        public Frmlcambiodivisas(AperturaCaja apertura)
        {
            InitializeComponent();

            _apertura = apertura;

            CargarMonedas();
            CargarFormasPago();

            guna2TextBox1.ReadOnly = true;
            guna2ComboBox1.SelectedIndex = 0; // dispara guna2ComboBox1_SelectedIndexChanged

            InicializarPanelCliente();
            InicializarDetalle();
        }

        /// <summary>Conecta el panel "Detalle": Subtotal = Cantidad x Denominacion por cada fila, Total = suma de subtotales.</summary>
        private void InicializarDetalle()
        {
            _filasDetalle.Add(textBox1,  new FilaDenominacion { Valor = 1000.00m, Subtotal = textBox20 });
            _filasDetalle.Add(textBox2,  new FilaDenominacion { Valor = 500.00m,  Subtotal = textBox19 });
            _filasDetalle.Add(textBox3,  new FilaDenominacion { Valor = 200.00m,  Subtotal = textBox18 });
            _filasDetalle.Add(textBox4,  new FilaDenominacion { Valor = 100.00m,  Subtotal = textBox17 });
            _filasDetalle.Add(textBox5,  new FilaDenominacion { Valor = 50.00m,   Subtotal = textBox16 });
            _filasDetalle.Add(textBox6,  new FilaDenominacion { Valor = 20.00m,   Subtotal = textBox15 });
            _filasDetalle.Add(textBox7,  new FilaDenominacion { Valor = 10.00m,   Subtotal = textBox14 });
            _filasDetalle.Add(textBox8,  new FilaDenominacion { Valor = 5.00m,    Subtotal = textBox13 });
            _filasDetalle.Add(textBox9,  new FilaDenominacion { Valor = 1.00m,    Subtotal = textBox12 });
            _filasDetalle.Add(textBox10, new FilaDenominacion { Valor = 0.50m,    Subtotal = textBox26 });
            _filasDetalle.Add(textBox11, new FilaDenominacion { Valor = 0.25m,    Subtotal = textBox27 });
            _filasDetalle.Add(textBox22, new FilaDenominacion { Valor = 0.05m,    Subtotal = textBox24 });
            _filasDetalle.Add(textBox21, new FilaDenominacion { Valor = 0.01m,    Subtotal = textBox23 });

            foreach (KeyValuePair<TextBox, FilaDenominacion> par in _filasDetalle)
            {
                TextBox cantidad = par.Key;
                TextBox subtotal = par.Value.Subtotal;

                cantidad.Text = "0";
                cantidad.KeyPress += DetalleCantidad_KeyPress;
                cantidad.TextChanged += DetalleCantidad_TextChanged;

                subtotal.ReadOnly = true;
                subtotal.TabStop = false;
                subtotal.Text = "0.00";

                _subtotalesDetalle.Add(subtotal);
            }

            textBox29.ReadOnly = true;
            textBox29.TabStop = false;
            textBox29.Text = "0.00";
        }

        /// <summary>La cantidad de billetes/monedas es un entero: solo digitos, sin letras ni decimales.</summary>
        private void DetalleCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar)) return;
            e.Handled = true;
        }

        /// <summary>Recalcula el subtotal de la fila (Cantidad x Denominacion) y el total general del panel Detalle.</summary>
        private void DetalleCantidad_TextChanged(object sender, EventArgs e)
        {
            var cantidadBox = (TextBox)sender;
            if (!_filasDetalle.TryGetValue(cantidadBox, out FilaDenominacion fila)) return;

            int cantidad = int.TryParse(cantidadBox.Text, out int valor) ? valor : 0;
            decimal subtotal = cantidad * fila.Valor;
            fila.Subtotal.Text = subtotal.ToString("N2", CultureInfo.CurrentCulture);

            RecalcularTotalDetalle();
        }

        private void RecalcularTotalDetalle()
        {
            decimal total = 0m;
            foreach (TextBox subtotal in _subtotalesDetalle)
            {
                if (decimal.TryParse(subtotal.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal parcial))
                    total += parcial;
            }

            textBox29.Text = total.ToString("N2", CultureInfo.CurrentCulture);
        }

        /// <summary>Boton "Limpiar": pone todas las cantidades en 0 (subtotales y total se recalculan solos a 0.00).</summary>
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            foreach (TextBox cantidad in _filasDetalle.Keys)
                cantidad.Text = "0";
        }

        /// <summary>Deja el panel "Informacion del Cliente" en su estado inicial: sin ficha cargada y sin poder agregar hasta buscar.</summary>
        private void InicializarPanelCliente()
        {
            guna2TextBox5.ReadOnly = true;
            guna2TextBox6.ReadOnly = true;
            guna2TextBox7.ReadOnly = true;

            MostrarCliente(null, null);
        }

        /// <summary>Busca el cliente por tipo/numero de identificacion y muestra el resultado en el panel "Informacion del Cliente".</summary>
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string tipoIdentificacion = guna2ComboBox3.SelectedItem as string;
            string numeroIdentificacion = guna2TextBox4.Text.Trim();

            if (string.IsNullOrWhiteSpace(tipoIdentificacion) || string.IsNullOrWhiteSpace(numeroIdentificacion))
            {
                MessageBox.Show("Seleccione el tipo e ingrese el numero de identificacion para buscar.", "Identificacion del Cliente",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cliente cliente = _negocioCliente.BuscarPorIdentificacion(tipoIdentificacion, numeroIdentificacion);
            MostrarCliente(cliente, numeroIdentificacion);
        }

        /// <summary>Abre el formulario "Nuevo Cliente" (con el Tipo/N° de Identificacion ya escritos) para dar de alta
        /// al cliente que no aparecio en la ultima busqueda; si se guarda, su ficha se muestra en el panel.</summary>
        private void MesaCambio_Click(object sender, EventArgs e)
        {
            string tipoIdentificacion = guna2ComboBox3.SelectedItem as string;
            string numeroIdentificacion = guna2TextBox4.Text.Trim();

            using (var formNuevoCliente = new Frmclientes(tipoIdentificacion, numeroIdentificacion))
            {
                if (formNuevoCliente.ShowDialog(this) == DialogResult.OK)
                    MostrarCliente(formNuevoCliente.ClienteRegistrado, numeroIdentificacion);
            }
        }

        /// <summary>Refleja el resultado de la busqueda en el panel, siempre de solo lectura: la ficha del cliente si existe,
        /// o vacio si no (en ese caso se completa desde el formulario "Nuevo Cliente" que abre el boton Agregar).</summary>
        private void MostrarCliente(Cliente cliente, string numeroIdentificacionBuscado)
        {
            bool encontrado = cliente != null;

            guna2TextBox5.Text = encontrado ? cliente.NombreCompleto : string.Empty;
            guna2TextBox6.Text = encontrado ? cliente.Telefono : string.Empty;
            guna2TextBox7.Text = encontrado ? cliente.Direccion : string.Empty;

            MesaCambio.Enabled = !encontrado && !string.IsNullOrWhiteSpace(numeroIdentificacionBuscado);

            if (string.IsNullOrWhiteSpace(numeroIdentificacionBuscado))
            {
                label29.Text = "Busque un cliente por su identificacion para ver o agregar sus datos.";
                label29.ForeColor = System.Drawing.Color.Black;
            }
            else if (encontrado)
            {
                label29.Text = $"Cliente encontrado: {cliente.NombreCompleto}";
                label29.ForeColor = System.Drawing.Color.DarkGreen;
            }
            else
            {
                label29.Text = $"No existe un cliente con identificacion {numeroIdentificacionBuscado}. Complete los datos y presione Agregar.";
                label29.ForeColor = System.Drawing.Color.DarkRed;
            }
        }

        private void CargarMonedas()
        {
            guna2ComboBox4.DataSource = _negocio.ListarMonedasActivas();
            guna2ComboBox4.DisplayMember = nameof(Moneda.Codigo);
            guna2ComboBox4.ValueMember = nameof(Moneda.MonedaId);

            guna2ComboBox2.DataSource = _negocio.ListarMonedasActivas();
            guna2ComboBox2.DisplayMember = nameof(Moneda.Codigo);
            guna2ComboBox2.ValueMember = nameof(Moneda.MonedaId);
        }

        private void CargarFormasPago()
        {
            guna2ComboBox5.DataSource = _negocio.ListarFormasPagoActivas();
            guna2ComboBox5.DisplayMember = nameof(FormaPago.Nombre);
            guna2ComboBox5.ValueMember = nameof(FormaPago.FormaPagoId);
        }

        /// <summary>Al cambiar Operacion (COMPRA/VENTA) fija las monedas: COMPRA recibe USD y entrega NIO; VENTA al reves.</summary>
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string operacion = guna2ComboBox1.SelectedItem as string ?? "COMPRA";
            bool esCompra = operacion == "COMPRA";

            SeleccionarMonedaPorCodigo(guna2ComboBox4, esCompra ? CodigoMonedaExtranjera : CodigoMonedaLocal);
            SeleccionarMonedaPorCodigo(guna2ComboBox2, esCompra ? CodigoMonedaLocal : CodigoMonedaExtranjera);

            ActualizarTasaAutomatica();

            // Los montos ya escritos correspondian a la moneda de la operacion anterior: no tiene sentido conservarlos.
            guna2TextBox2.Text = string.Empty;
            guna2TextBox3.Text = string.Empty;
        }

        private static void SeleccionarMonedaPorCodigo(ComboBox combo, string codigo)
        {
            if (!(combo.DataSource is List<Moneda> monedas)) return;

            Moneda moneda = monedas.Find(m => m.Codigo == codigo);
            if (moneda != null)
                combo.SelectedValue = moneda.MonedaId;
        }

        /// <summary>Habilita/deshabilita la edicion manual de la Tasa de Cambio segun "Tasa Preferencial".</summary>
        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            guna2TextBox1.ReadOnly = !guna2CheckBox1.Checked;

            if (!guna2CheckBox1.Checked)
                ActualizarTasaAutomatica();
        }

        /// <summary>Trae de la base de datos la tasa vigente para la moneda extranjera y el tipo de operacion actual.</summary>
        private void ActualizarTasaAutomatica()
        {
            if (guna2CheckBox1.Checked) return; // modo preferencial: no pisar el valor que el usuario está digitando

            string operacion = guna2ComboBox1.SelectedItem as string;
            Moneda monedaRecibida = guna2ComboBox4.SelectedItem as Moneda;
            Moneda monedaEntregada = guna2ComboBox2.SelectedItem as Moneda;
            Moneda extranjera = monedaRecibida != null && monedaRecibida.Codigo == CodigoMonedaExtranjera
                ? monedaRecibida
                : monedaEntregada;

            if (string.IsNullOrEmpty(operacion) || extranjera == null)
            {
                guna2TextBox1.Text = string.Empty;
                return;
            }

            TasaCambio tasa = _negocioTasa.ObtenerVigente(extranjera.MonedaId, operacion);
            guna2TextBox1.Text = tasa != null ? tasa.Valor.ToString("N4", CultureInfo.CurrentCulture) : string.Empty;
        }

        /// <summary>Monto Entregar = Monto Recibido convertido con la Tasa de Cambio (COMPRA multiplica, VENTA divide).</summary>
        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (_actualizandoMontos) return;

            if (!decimal.TryParse(guna2TextBox2.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal recibido)) return;
            if (!decimal.TryParse(guna2TextBox1.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal tasa) || tasa <= 0) return;

            bool esVenta = (guna2ComboBox1.SelectedItem as string) == "VENTA";
            decimal entregado = esVenta ? recibido / tasa : recibido * tasa;

            _actualizandoMontos = true;
            guna2TextBox3.Text = entregado.ToString("N2", CultureInfo.CurrentCulture);
            _actualizandoMontos = false;
        }

        /// <summary>Monto Recibido = Monto Entregar convertido con la Tasa de Cambio (inverso de guna2TextBox2_TextChanged).</summary>
        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {
            if (_actualizandoMontos) return;

            if (!decimal.TryParse(guna2TextBox3.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal entregado)) return;
            if (!decimal.TryParse(guna2TextBox1.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal tasa) || tasa <= 0) return;

            bool esVenta = (guna2ComboBox1.SelectedItem as string) == "VENTA";
            decimal recibido = esVenta ? entregado * tasa : entregado / tasa;

            _actualizandoMontos = true;
            guna2TextBox2.Text = recibido.ToString("N2", CultureInfo.CurrentCulture);
            _actualizandoMontos = false;
        }

        /// <summary>Si la tasa cambia (automatica o preferencial) y ya hay un Monto Recibido, recalcula lo que hay que entregar.</summary>
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            guna2TextBox2_TextChanged(sender, e);
        }

        /// <summary>Restringe un Guna2TextBox a digitos y un unico separador decimal (sin letras).</summary>
        private void TextBoxNumerico_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            if (char.IsDigit(e.KeyChar)) return;

            var textBox = (Guna.UI2.WinForms.Guna2TextBox)sender;
            char separador = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            if (e.KeyChar == separador && textBox.Text.IndexOf(separador) < 0)
                return;

            e.Handled = true;
        }

        /// <summary>Imprime una cotizacion con los montos ya calculados en pantalla, sin registrar ninguna operacion.</summary>
        private void btnCotizar_Click(object sender, EventArgs e)
        {
            if (guna2ComboBox4.SelectedValue == null || guna2ComboBox2.SelectedValue == null)
            {
                MessageBox.Show("Seleccione la moneda recibida y la moneda entregada.", "Cotizacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(guna2TextBox2.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal montoRecibido) || montoRecibido <= 0)
            {
                MessageBox.Show("Ingrese un monto recibido valido (mayor que 0) para cotizar.", "Cotizacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox2.Focus();
                return;
            }

            if (!decimal.TryParse(guna2TextBox3.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal montoEntregado) || montoEntregado <= 0)
            {
                MessageBox.Show("Ingrese un monto a entregar valido (mayor que 0) para cotizar.", "Cotizacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox3.Focus();
                return;
            }

            string operacion = guna2ComboBox1.SelectedItem as string ?? string.Empty;
            Moneda monedaRecibidaObj = guna2ComboBox4.SelectedItem as Moneda;
            Moneda monedaEntregadaObj = guna2ComboBox2.SelectedItem as Moneda;
            FormaPago formaPagoObj = guna2ComboBox5.SelectedItem as FormaPago;
            string cliente = guna2TextBox5.Text.Trim();
            string identificacion = guna2TextBox4.Text.Trim();
            decimal? tasaValor = decimal.TryParse(guna2TextBox1.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal tasa)
                ? tasa
                : (decimal?)null;

            ImprimirTiquete(operacion, monedaRecibidaObj, montoRecibido, monedaEntregadaObj, montoEntregado,
                tasaValor, guna2CheckBox1.Checked, formaPagoObj?.Nombre, cliente, identificacion, esCotizacion: true);
        }

        private void btnguardaringre_Click(object sender, EventArgs e)
        {
            if (_apertura == null)
            {
                MessageBox.Show("Debe abrir la caja antes de registrar una operacion de cambio.", "Mesa de Cambio",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (guna2ComboBox4.SelectedValue == null || guna2ComboBox2.SelectedValue == null)
            {
                MessageBox.Show("Seleccione la moneda recibida y la moneda entregada.", "Mesa de Cambio",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(guna2TextBox2.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal montoRecibido) || montoRecibido <= 0)
            {
                MessageBox.Show("Ingrese un monto recibido valido (mayor que 0).", "Mesa de Cambio",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox2.Focus();
                return;
            }

            if (!decimal.TryParse(guna2TextBox3.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal montoEntregado) || montoEntregado <= 0)
            {
                MessageBox.Show("Ingrese un monto a entregar valido (mayor que 0).", "Mesa de Cambio",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox3.Focus();
                return;
            }

            int monedaRecibidaId = Convert.ToInt32(guna2ComboBox4.SelectedValue);
            int monedaEntregadaId = Convert.ToInt32(guna2ComboBox2.SelectedValue);
            int? formaPagoId = guna2ComboBox5.SelectedValue == null ? (int?)null : Convert.ToInt32(guna2ComboBox5.SelectedValue);

            string operacion = guna2ComboBox1.SelectedItem as string ?? string.Empty;
            string cliente = guna2TextBox5.Text.Trim();
            string identificacion = guna2TextBox4.Text.Trim();
            string tipoCambio = guna2TextBox1.Text.Trim();

            string descripcion = $"Mesa de Cambio {operacion}".Trim();
            if (!string.IsNullOrWhiteSpace(tipoCambio))
                descripcion += $" - T.C. {tipoCambio}";
            if (guna2CheckBox1.Checked)
                descripcion += " (Tasa Preferencial)";
            if (!string.IsNullOrWhiteSpace(cliente))
                descripcion += $" - Cliente: {cliente}";
            if (!string.IsNullOrWhiteSpace(identificacion))
                descripcion += $" ({identificacion})";

            try
            {
                _negocio.RegistrarCambioDivisa(_apertura, SesionActual.Usuario.usuario_id,
                    monedaRecibidaId, montoRecibido, monedaEntregadaId, montoEntregado,
                    formaPagoId, descripcion);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo registrar la operacion de cambio: " + ex.Message, "Mesa de Cambio",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult deseaImprimir = MessageBox.Show(
                "Operacion de cambio registrada correctamente." + Environment.NewLine + Environment.NewLine +
                "¿Desea imprimir el tiquet?", "Mesa de Cambio",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (deseaImprimir == DialogResult.Yes)
            {
                Moneda monedaRecibidaObj = guna2ComboBox4.SelectedItem as Moneda;
                Moneda monedaEntregadaObj = guna2ComboBox2.SelectedItem as Moneda;
                FormaPago formaPagoObj = guna2ComboBox5.SelectedItem as FormaPago;
                decimal? tasaValor = decimal.TryParse(tipoCambio, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal tasa)
                    ? tasa
                    : (decimal?)null;

                ImprimirTiquete(operacion, monedaRecibidaObj, montoRecibido, monedaEntregadaObj, montoEntregado,
                    tasaValor, guna2CheckBox1.Checked, formaPagoObj?.Nombre, cliente, identificacion);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>Imprime un tiquet de la operacion de cambio (registrada, o solo una cotizacion si esCotizacion es true), dejando elegir la impresora (o cancelar).</summary>
        private void ImprimirTiquete(string operacion, Moneda monedaRecibida, decimal montoRecibido,
            Moneda monedaEntregada, decimal montoEntregado, decimal? tasa, bool tasaPreferencial,
            string formaPago, string cliente, string identificacion, bool esCotizacion = false)
        {
            using (var documento = new System.Drawing.Printing.PrintDocument())
            {
                // Aplica la impresora, el ancho de papel y los margenes configurados en
                // Configuracion > Impresora (o los valores por defecto si no se configuro nada).
                ConfiguracionImpresora.Aplicar(documento);
                documento.PrintPage += (sender, e) => DibujarTiquete(e, operacion, monedaRecibida, montoRecibido,
                    monedaEntregada, montoEntregado, tasa, tasaPreferencial, formaPago, cliente, identificacion, esCotizacion);

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
                    MessageBox.Show("No se pudo imprimir el tiquet: " + ex.Message, "Mesa de Cambio",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DibujarTiquete(System.Drawing.Printing.PrintPageEventArgs e, string operacion,
            Moneda monedaRecibida, decimal montoRecibido, Moneda monedaEntregada, decimal montoEntregado,
            decimal? tasa, bool tasaPreferencial, string formaPago, string cliente, string identificacion,
            bool esCotizacion)
        {
            System.Drawing.Graphics g = e.Graphics;
            // Ancho real del area imprimible (p.ej. ~189 centesimas de pulgada en papel termico
            // de 58mm/48mm imprimible), en vez de un valor fijo pensado para papel mas ancho.
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
            Escribir(esCotizacion ? "COTIZACION" : "Mesa de Cambio", fontTexto, centrado);
            if (esCotizacion)
                Escribir("(No es una operacion registrada)", fontChico, centrado);
            Escribir(separador, fontChico);
            Escribir($"Fecha: {DateTime.Now:dd/MM/yyyy hh:mm tt}", fontChico);
            if (!esCotizacion)
                Escribir($"Caja: {_apertura?.CajaNombre}", fontChico);
            Escribir($"Cajero: {SesionActual.Usuario?.NombreCompleto}", fontChico);
            Escribir($"Operacion: {operacion}", fontChico);
            Escribir(separador, fontChico);
            Escribir($"Recibido:  {FormatoMonto(monedaRecibida, montoRecibido)}", fontTexto);
            Escribir($"Entregado: {FormatoMonto(monedaEntregada, montoEntregado)}", fontTexto);
            if (tasa.HasValue)
                Escribir($"Tasa: {tasa.Value.ToString("N4", CultureInfo.CurrentCulture)}{(tasaPreferencial ? " (Preferencial)" : string.Empty)}", fontChico);
            if (!string.IsNullOrWhiteSpace(formaPago))
                Escribir($"Forma de pago: {formaPago}", fontChico);
            if (!string.IsNullOrWhiteSpace(cliente))
                Escribir($"Cliente: {cliente}", fontChico);
            if (!string.IsNullOrWhiteSpace(identificacion))
                Escribir($"Identificacion: {identificacion}", fontChico);
            Escribir(separador, fontChico);
            Escribir("Gracias por su preferencia", fontChico, centrado);

            e.HasMorePages = false;
        }

        private static string FormatoMonto(Moneda moneda, decimal monto)
        {
            string simbolo = string.IsNullOrWhiteSpace(moneda?.Simbolo) ? string.Empty : moneda.Simbolo;
            return $"{simbolo}{monto.ToString("N2", CultureInfo.CurrentCulture)} {moneda?.Codigo}";
        }
    }
}
