using CapaPresentacion.Formularios;
using CapaPresentacion.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    public partial class fmrInicio : Form
    {
        /// <summary>
        /// Queda en true cuando el usuario eligió "Cerrar sesión" (para que el flujo
        /// principal vuelva a mostrar el login en lugar de terminar la aplicación).
        /// </summary>
        public bool CerrarSesionSolicitado { get; private set; }

        private readonly CN_AperturaCaja _negocioCaja = new CN_AperturaCaja();
        private readonly CN_Transaccion _negocioTransaccion = new CN_Transaccion();
        private readonly CN_Usuario _negocioUsuario = new CN_Usuario();
        private readonly CN_Roles _negocioRoles = new CN_Roles();
        private readonly CN_Moneda _negocioMoneda = new CN_Moneda();
        private readonly CN_FormaPago _negocioFormaPago = new CN_FormaPago();
        private readonly CN_TasaCambio _negocioTasaCambio = new CN_TasaCambio();
        private readonly CN_Concepto _negocioConcepto = new CN_Concepto();
        private AperturaCaja _aperturaActual;
        private List<Transaccion> _movimientosActuales = new List<Transaccion>();

        /// <summary>Movimientos mostrados actualmente en guna2DataGridView1, en el mismo orden que sus filas (para ubicar la fila seleccionada al anular).</summary>
        private List<Transaccion> _movimientosGridMovimientos = new List<Transaccion>();

        public fmrInicio()
        {
            InitializeComponent();

            guna2DataGridView6.ReadOnly = true;
            guna2DataGridView6.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.AllowUserToAddRows = false;

            // Colorea la columna "Estado" (verde Activo, rojo Inactivo) en todas las grillas que la tienen.
            foreach (DataGridView grilla in new DataGridView[]
            {
                guna2DataGridView1, guna2DataGridView2, guna2DataGridView3, guna2DataGridView4,
                guna2DataGridView5, guna2DataGridView6, dgvRoles, dgvMonedas, dgvFormasPago,
                dgvTasas, dgvOperaciones
            })
            {
                grilla.CellFormatting += ColorearCeldaEstado;
            }

            guna2ComboBox1.SelectedIndex = 0;
            guna2ComboBox2.SelectedIndex = 0;
        }

        public fmrInicio(Usuario usuario) : this()
        {
            if (usuario != null)
            {
                this.Text = $"Sistema de Caja  -  {usuario.NombreCompleto}" +
                            (string.IsNullOrWhiteSpace(usuario.RolDescripcion)
                                ? string.Empty
                                : $" ({usuario.RolDescripcion})");
            }
        }

        //private void btnControlEfectivo_Click(object sender, EventArgs e)
        //{
        //    //Cerrar cualquier formulario que ya esté en el panel.
        //    foreach (Control control in PanelContenedor.Controls)
        //    {
        //        control.Dispose();
        //    }

        //    //Crear instancia del formulario secundario.
        //   FrmlEntrada formControlEfectivo = new FrmlEntrada
        //   {
        //       TopLevel = false, // Para que se comporte como un control en el panel
        //       Dock = DockStyle.Fill // Para que ocupe todo el espacio del panel
        //   };

        //    PanelContenedor.Controls.Add(formControlEfectivo);
        //    PanelContenedor.Tag = formControlEfectivo;
        //    formControlEfectivo.Show();
            //FrmlEntrada efectivo = new FrmlEntrada(); // Crear una instancia del fmrcaja
            //efectivo.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
            //efectivo.ShowDialog(); // Mostrarlo como emergente

        //}

        //private void btnHome_Click(object sender, EventArgs e)
        //{
        //    //Cerrar cualquier formulario que ya esté en el panel.
        //    foreach (Control control in PanelContenedor.Controls)
        //    {
        //        control.Dispose();
        //    }

        //    //Crear instancia del formulario secundario.
        //    Dashboard dash = new Dashboard
        //    {
        //        TopLevel = false, // Para que se comporte como un control en el panel
        //        Dock = DockStyle.Fill // Para que ocupe todo el espacio del panel
        //    };

        //    PanelContenedor.Controls.Add(dash);
        //    PanelContenedor.Tag = dash;
        //    dash.Show();
        //}

      


        private void fmrInicio_Load(object sender, EventArgs e)
        {
            // Restaura el estado real desde la base de datos (por si la app se cerro
            // con una caja abierta, o la abrio otra instancia).
            _aperturaActual = _negocioCaja.ObtenerUltimaAperturaAbierta();
            ActualizarEstadoCaja();
            CargarFiltroOperacionInicio();
            CargarMovimientos();
            CargarHistorial();
            CargarCajas();
            CargarDatosReportes();
            CargarUsuarios();
            CargarRoles();
            CargarMonedas();
            CargarFormasPago();
            CargarTasas();
            CargarOperaciones();
            CargarTasasInicio();

            // Rango por defecto del KPI de Mesa de Cambio: el ultimo mes (modificable por el usuario).
            dtpKpiDesde.Value = DateTime.Today.AddMonths(-1);
            dtpKpiHasta.Value = DateTime.Today;
            CargarKpiMesaCambio();
        }

        /// <summary>Trae de la base de datos las tasas de cambio vigentes y refresca el panel "Tasas de Cambio" del Inicio.</summary>
        private void CargarTasasInicio()
        {
            List<TasaCambio> tasas = _negocioTasaCambio.Listar().Where(t => t.Estado).ToList();

            var tabla = new DataTable();
            tabla.Columns.Add("Moneda");
            tabla.Columns.Add("Tipo");
            tabla.Columns.Add("Valor");

            foreach (TasaCambio t in tasas.OrderBy(t => t.MonedaNombre).ThenBy(t => t.TipoOperacion))
            {
                tabla.Rows.Add(
                    $"{t.MonedaNombre} ({t.MonedaCodigo})",
                    t.TipoOperacion == "COMPRA" ? "Compra" : "Venta",
                    t.Valor.ToString("N4", CultureInfo.CurrentCulture));
            }

            guna2DataGridView7.DataSource = tabla;
            guna2DataGridView7.ReadOnly = true;
            guna2DataGridView7.AllowUserToAddRows = false;
        }

        /// <summary>Codigo de la divisa extranjera que maneja Mesa de Cambio (ver Frmlcambiodivisas).</summary>
        private const string CodigoMonedaExtranjera = "USD";

        /// <summary>
        /// Suma el total de dolares comprados y vendidos en Mesa de Cambio en el rango de fechas
        /// seleccionado y actualiza las tarjetas KPI. Compra = la caja recibe USD (INGRESO);
        /// Venta = la caja entrega USD (EGRESO).
        /// </summary>
        private void CargarKpiMesaCambio()
        {
            DateTime desde = dtpKpiDesde.Value.Date;
            DateTime hasta = dtpKpiHasta.Value.Date;
            if (hasta < desde) return; // rango invalido: se espera a que el usuario termine de ajustar las fechas

            List<Transaccion> movimientos = _negocioTransaccion.ListarCambiosDivisaParaReporte(desde, hasta);

            decimal totalCompras = movimientos.Where(t => t.Tipo == "INGRESO" && t.MonedaCodigo == CodigoMonedaExtranjera).Sum(t => t.Monto);
            decimal totalVentas = movimientos.Where(t => t.Tipo == "EGRESO" && t.MonedaCodigo == CodigoMonedaExtranjera).Sum(t => t.Monto);

            lblKpiComprasValor.Text = "$" + totalCompras.ToString("N2", CultureInfo.CurrentCulture);
            lblKpiVentasValor.Text = "$" + totalVentas.ToString("N2", CultureInfo.CurrentCulture);
        }

        private void FiltroKpiCambio_ValueChanged(object sender, EventArgs e)
        {
            CargarKpiMesaCambio();
        }

        /// <summary>Trae de la base de datos todas las cajas registradoras (activas e inactivas) y refresca la pestaña Cajas.</summary>
        private void CargarCajas()
        {
            List<Caja> cajas = _negocioCaja.ListarTodasLasCajas();

            var tabla = new DataTable();
            tabla.Columns.Add("Id", typeof(int));
            tabla.Columns.Add("Nombre");
            tabla.Columns.Add("Estado");
            tabla.Columns.Add("Fecha de Creacion");

            foreach (Caja caja in cajas)
            {
                tabla.Rows.Add(
                    caja.CajaId,
                    caja.Nombre,
                    caja.Estado ? "Activa" : "Inactiva",
                    caja.FechaCreacion.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture));
            }

            guna2DataGridView2.DataSource = tabla;
            guna2DataGridView2.ReadOnly = true;
            guna2DataGridView2.AllowUserToAddRows = false;
            guna2DataGridView2.Columns["Id"].Visible = false;
        }

        /// <summary>Caja seleccionada actualmente en la grilla de la pestaña Cajas, o null si no hay seleccion.</summary>
        private Caja ObtenerCajaSeleccionada()
        {
            if (guna2DataGridView2.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una caja de la lista.", "Cajas Registradoras",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            DataRowView fila = (DataRowView)guna2DataGridView2.CurrentRow.DataBoundItem;
            return new Caja
            {
                CajaId = Convert.ToInt32(fila["Id"]),
                Nombre = Convert.ToString(fila["Nombre"]),
                Estado = Convert.ToString(fila["Estado"]) == "Activa"
            };
        }

        private void btnEditarCaja_Click(object sender, EventArgs e)
        {
            Caja caja = ObtenerCajaSeleccionada();
            if (caja == null) return;

            Frmcajaregistradora editor = new Frmcajaregistradora(caja);
            editor.StartPosition = FormStartPosition.CenterParent;
            if (editor.ShowDialog() == DialogResult.OK)
                CargarCajas();
        }

        private void btnEstadoCaja_Click(object sender, EventArgs e)
        {
            Caja caja = ObtenerCajaSeleccionada();
            if (caja == null) return;

            bool nuevoEstado = !caja.Estado;
            string accion = nuevoEstado ? "activar" : "desactivar";

            DialogResult confirmacion = MessageBox.Show(
                $"¿Desea {accion} la caja \"{caja.Nombre}\"?", "Cajas Registradoras",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _negocioCaja.CambiarEstadoCaja(caja.CajaId, nuevoEstado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cambiar el estado de la caja: " + ex.Message, "Cajas Registradoras",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarCajas();
        }

        /// <summary>Trae de la base de datos las sesiones de caja cerradas y refresca la pestaña Historial.</summary>
        private void CargarHistorial()
        {
            List<HistorialCierre> historial = _negocioCaja.ListarHistorialCierres();

            var tabla = new DataTable();
            tabla.Columns.Add("Fecha Apertura");
            tabla.Columns.Add("Fecha Cierre");
            tabla.Columns.Add("Caja");
            tabla.Columns.Add("Abierta Por");
            tabla.Columns.Add("Cerrada Por");
            tabla.Columns.Add("Moneda");
            tabla.Columns.Add("Monto Inicial");
            tabla.Columns.Add("Monto Sistema");
            tabla.Columns.Add("Monto Contado");
            tabla.Columns.Add("Diferencia");
            tabla.Columns.Add("Observaciones");

            foreach (HistorialCierre h in historial)
            {
                tabla.Rows.Add(
                    h.FechaApertura.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture),
                    h.FechaCierre.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture),
                    h.CajaNombre,
                    h.UsuarioAperturaNombre,
                    h.UsuarioCierreNombre,
                    $"{h.MonedaNombre} ({h.MonedaCodigo})",
                    h.MontoInicial.ToString("N2", CultureInfo.CurrentCulture),
                    h.MontoSistema.ToString("N2", CultureInfo.CurrentCulture),
                    h.MontoFinal.ToString("N2", CultureInfo.CurrentCulture),
                    h.Diferencia.ToString("N2", CultureInfo.CurrentCulture),
                    string.IsNullOrWhiteSpace(h.ObservacionesCierre) ? h.ObservacionesApertura : h.ObservacionesCierre);
            }

            guna2DataGridView4.DataSource = tabla;
            guna2DataGridView4.ReadOnly = true;
            guna2DataGridView4.AllowUserToAddRows = false;

            ActualizarResumenHistorial(historial);
        }

        /// <summary>Refleja en los paneles de estadisticas cuantas sesiones se cerraron, con falta/sobra, y la diferencia total por moneda.</summary>
        private void ActualizarResumenHistorial(List<HistorialCierre> historial)
        {
            int sesionesCerradas = historial.Select(h => h.CierreId).Distinct().Count();
            int conFaltante = historial.Where(h => h.Diferencia < 0).Select(h => h.CierreId).Distinct().Count();
            int conSobrante = historial.Where(h => h.Diferencia > 0).Select(h => h.CierreId).Distinct().Count();

            label15.Text = sesionesCerradas.ToString(CultureInfo.CurrentCulture);
            label16.Text = conFaltante.ToString(CultureInfo.CurrentCulture);
            label17.Text = conSobrante.ToString(CultureInfo.CurrentCulture);

            // La diferencia se acumula por moneda (no tiene sentido sumar cordobas con dolares).
            var diferenciaPorMoneda = historial
                .GroupBy(h => h.MonedaCodigo)
                .Select(g => new { Codigo = g.Key, Simbolo = g.First().MonedaSimbolo, Total = g.Sum(h => h.Diferencia) })
                .Where(x => x.Total != 0)
                .ToList();

            if (diferenciaPorMoneda.Count == 0)
            {
                label18.Text = "0.00";
                label18.ForeColor = System.Drawing.Color.FromArgb(68, 88, 112);
            }
            else
            {
                label18.Text = string.Join(" / ", diferenciaPorMoneda.Select(x =>
                    $"{(string.IsNullOrWhiteSpace(x.Simbolo) ? x.Codigo : x.Simbolo)} {x.Total:N2}"));
                label18.ForeColor = diferenciaPorMoneda.Any(x => x.Total < 0)
                    ? System.Drawing.Color.FromArgb(185, 51, 73)
                    : System.Drawing.Color.FromArgb(5, 150, 105);
            }
        }

        private void guna2TabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (guna2TabControl2.SelectedTab == tabPage7)
                CargarHistorial();
        }

        /// <summary>Trae de la base de datos los movimientos de la apertura actual y refresca ambas grillas.</summary>
        private void CargarMovimientos()
        {
            _movimientosActuales = _aperturaActual == null
                ? new List<Transaccion>()
                : _negocioTransaccion.ListarPorApertura(_aperturaActual.AperturaId);

            AplicarFiltro(guna2DataGridView6, guna2ComboBox1);
            AplicarFiltro(guna2DataGridView1, guna2ComboBox2);
            CargarResumenCajaDia();
            CargarKpiMesaCambio();
            ActualizarSaldoDisponible();
        }

        /// <summary>Trae de la base de datos el total de ingresos y egresos de hoy (todas las cajas) y refresca el panel "Resumen de caja".</summary>
        private void CargarResumenCajaDia()
        {
            DateTime hoy = DateTime.Today;
            List<Transaccion> movimientosHoy = _negocioTransaccion.ListarParaReporte(hoy, hoy);

            label24.Text = FormatoTotalPorMoneda(movimientosHoy.Where(t => t.Tipo == "INGRESO"));
            label25.Text = FormatoTotalPorMoneda(movimientosHoy.Where(t => t.Tipo == "EGRESO"));
        }

        /// <summary>Suma los montos agrupados por moneda (no tiene sentido sumar cordobas con dolares) y los formatea "Simbolo Total".</summary>
        private static string FormatoTotalPorMoneda(IEnumerable<Transaccion> movimientos)
        {
            var porMoneda = movimientos
                .GroupBy(t => new { t.MonedaCodigo, t.MonedaSimbolo })
                .Select(g => new { g.Key.MonedaCodigo, g.Key.MonedaSimbolo, Total = g.Sum(t => t.Monto) })
                .OrderBy(x => x.MonedaCodigo)
                .ToList();

            if (porMoneda.Count == 0) return "0.00";

            return string.Join(" / ", porMoneda.Select(x =>
                $"{(string.IsNullOrWhiteSpace(x.MonedaSimbolo) ? x.MonedaCodigo : x.MonedaSimbolo)} {x.Total:N2}"));
        }

        private void AplicarFiltro(DataGridView grilla, ComboBox comboFiltro)
        {
            string filtro = comboFiltro.SelectedItem as string ?? "Todos";

            IEnumerable<Transaccion> movimientos = _movimientosActuales;
            if (filtro == "Ingreso")
                movimientos = movimientos.Where(t => t.Tipo == "INGRESO");
            else if (filtro == "Egreso")
                movimientos = movimientos.Where(t => t.Tipo == "EGRESO");

            if (grilla == guna2DataGridView6)
            {
                int? conceptoId = (cboFiltroOperacionInicio.SelectedItem as OpcionFiltro)?.Id;
                if (conceptoId.HasValue)
                    movimientos = movimientos.Where(t => t.ConceptoId == conceptoId.Value);

                LlenarGridInicio(movimientos);
            }
            else
            {
                LlenarGridMovimientos(movimientos);
            }
        }

        /// <summary>Llena el combo "Tipo de Operacion" de Movimientos del Turno con los conceptos activos (Ingreso y Egreso).</summary>
        private void CargarFiltroOperacionInicio()
        {
            cboFiltroOperacionInicio.Items.Clear();
            cboFiltroOperacionInicio.Items.Add(new OpcionFiltro { Id = null, Texto = "Todas las operaciones" });
            foreach (Concepto c in _negocioTransaccion.ListarConceptosActivos())
                cboFiltroOperacionInicio.Items.Add(new OpcionFiltro { Id = c.ConceptoId, Texto = $"{c.Nombre} ({(c.Tipo == "INGRESO" ? "Ingreso" : "Egreso")})" });
            cboFiltroOperacionInicio.SelectedIndex = 0;
        }

        private void FiltroOperacionInicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltro(guna2DataGridView6, guna2ComboBox1);
        }

        /// <summary>Orden de columnas de guna2DataGridView6: Fecha, Operacion, Tipo, Moneda, Monto, Descripcion, Forma de pago, Estado.</summary>
        private void LlenarGridInicio(IEnumerable<Transaccion> movimientos)
        {
            guna2DataGridView6.Rows.Clear();
            foreach (Transaccion t in movimientos)
            {
                guna2DataGridView6.Rows.Add(
                    t.FechaHora.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture),
                    t.ConceptoNombre,
                    t.Tipo == "INGRESO" ? "Ingreso" : "Egreso",
                    $"{t.MonedaNombre} ({t.MonedaCodigo})",
                    t.Monto.ToString("N2", CultureInfo.CurrentCulture),
                    t.Descripcion,
                    t.FormaPagoNombre ?? "-",
                    t.Estado ? "Activo" : "Inactivo");
            }
        }

        /// <summary>Orden de columnas de guna2DataGridView1: Fecha, Operacion, Moneda, Tipo, Monto, Forma de pago, Descripcion, Estado.</summary>
        private void LlenarGridMovimientos(IEnumerable<Transaccion> movimientos)
        {
            _movimientosGridMovimientos = movimientos.ToList();

            guna2DataGridView1.Rows.Clear();
            foreach (Transaccion t in _movimientosGridMovimientos)
            {
                guna2DataGridView1.Rows.Add(
                    t.FechaHora.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture),
                    t.ConceptoNombre,
                    $"{t.MonedaNombre} ({t.MonedaCodigo})",
                    t.Tipo == "INGRESO" ? "Ingreso" : "Egreso",
                    t.Monto.ToString("N2", CultureInfo.CurrentCulture),
                    t.FormaPagoNombre ?? "-",
                    t.Descripcion,
                    t.Estado ? "Activo" : "Inactivo");
            }
        }

        /// <summary>Colorea cualquier columna "Estado" de cualquier grilla: verde para Activo/Activa, rojo para Inactivo/Inactiva.</summary>
        private void ColorearCeldaEstado(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grilla = (DataGridView)sender;
            if (e.ColumnIndex < 0 || grilla.Columns[e.ColumnIndex].HeaderText != "Estado") return;

            string valor = e.Value as string;
            if (string.Equals(valor, "Activo", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(valor, "Activa", StringComparison.OrdinalIgnoreCase))
                e.CellStyle.ForeColor = System.Drawing.Color.FromArgb(5, 150, 105);
            else if (string.Equals(valor, "Inactivo", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(valor, "Inactiva", StringComparison.OrdinalIgnoreCase))
                e.CellStyle.ForeColor = System.Drawing.Color.FromArgb(185, 51, 73);
        }

        /// <summary>Movimiento seleccionado actualmente en guna2DataGridView1, o null si no hay seleccion.</summary>
        private Transaccion ObtenerMovimientoSeleccionado()
        {
            int indice = guna2DataGridView1.CurrentRow?.Index ?? -1;
            if (indice < 0 || indice >= _movimientosGridMovimientos.Count)
            {
                MessageBox.Show("Seleccione un movimiento de la lista.", "Anular Movimiento",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return _movimientosGridMovimientos[indice];
        }

        /// <summary>Anula (marca como Inactivo) un movimiento registrado por error, y refresca todos los paneles afectados.</summary>
        private void btnAnularMovimiento_Click(object sender, EventArgs e)
        {
            Transaccion movimiento = ObtenerMovimientoSeleccionado();
            if (movimiento == null) return;

            if (!movimiento.Estado)
            {
                MessageBox.Show("Ese movimiento ya esta anulado.", "Anular Movimiento",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                $"¿Desea anular este movimiento?\n\n{movimiento.ConceptoNombre} - {movimiento.MonedaCodigo} {movimiento.Monto:N2}\n\n" +
                "Esta accion no se puede deshacer: el movimiento quedara marcado como Inactivo y ya no contara en los saldos ni en los reportes.",
                "Anular Movimiento", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _negocioTransaccion.AnularTransaccion(movimiento.TransaccionId);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Anular Movimiento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo anular el movimiento: " + ex.Message, "Anular Movimiento",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarMovimientos();
        }

        private void FiltroMovimientosInicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltro(guna2DataGridView6, guna2ComboBox1);
        }

        private void FiltroMovimientosTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltro(guna2DataGridView1, guna2ComboBox2);
        }

        /// <summary>Refleja en el panel "Control de caja" si hay una sesión de caja abierta.</summary>
        private void ActualizarEstadoCaja()
        {
            if (_aperturaActual != null)
            {
                label20.Text = "Caja Abierta";
                label19.Text = $"{_aperturaActual.CajaNombre} - Abierta {_aperturaActual.FechaHora:dd/MM/yyyy hh:mm tt}";
            }
            else
            {
                label20.Text = "Caja Cerrada";
                label19.Text = "No hay una sesion activa";
            }

            guna2Button3.Enabled = _aperturaActual == null;
            btnCerrarCaja.Enabled = _aperturaActual != null;
        }

        /// <summary>Refleja en el panel "Control de caja" el saldo disponible (monto en sistema) de cada moneda de la apertura actual.</summary>
        private void ActualizarSaldoDisponible()
        {
            if (_aperturaActual == null)
            {
                lblSaldoDisponible.Text = string.Empty;
                return;
            }

            Dictionary<int, decimal> saldos = _negocioCaja.CalcularMontoSistema(_aperturaActual.AperturaId);
            if (saldos.Count == 0)
            {
                lblSaldoDisponible.Text = string.Empty;
                return;
            }

            List<Moneda> monedas = _negocioMoneda.Listar();
            string detalle = string.Join("     ", saldos
                .Select(kv => new { Moneda = monedas.Find(m => m.MonedaId == kv.Key), Monto = kv.Value })
                .OrderBy(x => x.Moneda?.Codigo)
                .Select(x => $"{(string.IsNullOrWhiteSpace(x.Moneda?.Simbolo) ? x.Moneda?.Codigo : x.Moneda.Simbolo)} {x.Monto.ToString("N2", CultureInfo.CurrentCulture)}"));

            lblSaldoDisponible.Text = "Saldo disponible: " + detalle;
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            using (var apertura = new Apertura())
            {
                if (apertura.ShowDialog(this) == DialogResult.OK)
                {
                    _aperturaActual = apertura.AperturaCreada;
                    ActualizarEstadoCaja();
                    CargarMovimientos();
                }
            }
        }

        private void btnCerrarCaja_Click(object sender, EventArgs e)
        {
            using (var cierre = new Cierre(_aperturaActual))
            {
                if (cierre.ShowDialog(this) == DialogResult.OK)
                {
                    string resumen = string.Join("\n", cierre.CierreCreado.Montos.ConvertAll(m =>
                        $"{m.MonedaCodigo}: contado {m.MontoFinal:N2}, diferencia {m.Diferencia:N2}"));

                    MessageBox.Show($"Caja cerrada.\n{resumen}", "Cierre de caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _aperturaActual = null;
                    ActualizarEstadoCaja();
                    CargarMovimientos();
                    CargarHistorial();
                }
            }
        }

        private void tmTiempo_Tick(object sender, EventArgs e)
        {
           lblfecha.Text = DateTime.Now.ToLongDateString();
           lblHora.Text = DateTime.Now.ToLongTimeString();
        }

        
     

        private void btnlog_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show(
                "¿Estás seguro que deseas cerrar sesión?", "Cerrar sesión",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                CerrarSesionSolicitado = true;
                this.Close(); // el flujo principal (Program) mostrará el login nuevamente
            }
        }

        private void btningreso_Click(object sender, EventArgs e)
        {
            if (_aperturaActual == null)
            {
                MessageBox.Show("Debe abrir la caja antes de registrar un ingreso.", "Ingreso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FrmIngreso ingre = new FrmIngreso(_aperturaActual);
            ingre.StartPosition = FormStartPosition.CenterScreen;
            if (ingre.ShowDialog() == DialogResult.OK)
                CargarMovimientos();
        }

        private void btnegreso_Click(object sender, EventArgs e)
        {
            if (_aperturaActual == null)
            {
                MessageBox.Show("Debe abrir la caja antes de registrar un egreso.", "Egreso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FrmSalida sali = new FrmSalida(_aperturaActual);
            sali.StartPosition = FormStartPosition.CenterScreen;
            if (sali.ShowDialog() == DialogResult.OK)
                CargarMovimientos();
        }

        private void MesaCambio_Click(object sender, EventArgs e)
        {
            if (_aperturaActual == null)
            {
                MessageBox.Show("Debe abrir la caja antes de registrar una operacion de cambio.", "Mesa de Cambio",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Frmlcambiodivisas entrada = new Frmlcambiodivisas(_aperturaActual);
            entrada.StartPosition = FormStartPosition.CenterParent;
            if (entrada.ShowDialog() == DialogResult.OK)
                CargarMovimientos();
        }

        private void btncajaregistradora_Click(object sender, EventArgs e)
        {
            Frmcajaregistradora cajaregis = new Frmcajaregistradora();
            cajaregis.StartPosition = FormStartPosition.CenterParent;
            if (cajaregis.ShowDialog() == DialogResult.OK)
                CargarCajas();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnnuevocliente_Click(object sender, EventArgs e)
        {
            Frmclientes cliente = new Frmclientes(); // Crear una instancia de Form2
            cliente.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
            cliente.ShowDialog(); // Mostrarlo como emergente
        }

        // ===================== Configuracion: Usuarios =====================

        /// <summary>Trae de la base de datos todos los usuarios y refresca la grilla de la pestaña Usuarios.</summary>
        private void CargarUsuarios()
        {
            List<Usuario> usuarios = _negocioUsuario.Listar();

            var tabla = new DataTable();
            tabla.Columns.Add("Id", typeof(int));
            tabla.Columns.Add("Nombre");
            tabla.Columns.Add("Usuario");
            tabla.Columns.Add("Rol");
            tabla.Columns.Add("Fecha de creacion");
            tabla.Columns.Add("Estado");

            foreach (Usuario u in usuarios)
            {
                tabla.Rows.Add(
                    u.usuario_id,
                    u.NombreCompleto,
                    u.usuario,
                    u.RolDescripcion ?? "-",
                    u.fechacreacion.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture),
                    u.estado ? "Activo" : "Inactivo");
            }

            guna2DataGridView3.AutoGenerateColumns = false;
            guna2DataGridView3.DataSource = tabla;
        }

        /// <summary>Usuario seleccionado actualmente en la grilla de la pestaña Usuarios, o null si no hay seleccion.</summary>
        private Usuario ObtenerUsuarioSeleccionado()
        {
            if (guna2DataGridView3.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un usuario de la lista.", "Usuarios",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            DataRowView fila = (DataRowView)guna2DataGridView3.CurrentRow.DataBoundItem;
            return _negocioUsuario.Listar().Find(u => u.usuario_id == Convert.ToInt32(fila["Id"]));
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            FrmUsuario nuevo = new FrmUsuario();
            nuevo.StartPosition = FormStartPosition.CenterParent;
            if (nuevo.ShowDialog() == DialogResult.OK)
                CargarUsuarios();
        }

        private void btnEditarUsuario_Click(object sender, EventArgs e)
        {
            Usuario usuario = ObtenerUsuarioSeleccionado();
            if (usuario == null) return;

            FrmUsuario editor = new FrmUsuario(usuario);
            editor.StartPosition = FormStartPosition.CenterParent;
            if (editor.ShowDialog() == DialogResult.OK)
                CargarUsuarios();
        }

        private void btnEstadoUsuario_Click(object sender, EventArgs e)
        {
            Usuario usuario = ObtenerUsuarioSeleccionado();
            if (usuario == null) return;

            if (SesionActual.Usuario != null && usuario.usuario_id == SesionActual.Usuario.usuario_id)
            {
                MessageBox.Show("No puede desactivar su propia cuenta mientras tiene la sesion iniciada.", "Usuarios",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool nuevoEstado = !usuario.estado;
            string accion = nuevoEstado ? "activar" : "desactivar";

            DialogResult confirmacion = MessageBox.Show(
                $"¿Desea {accion} al usuario \"{usuario.NombreCompleto}\"?", "Usuarios",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _negocioUsuario.CambiarEstadoUsuario(usuario.usuario_id, nuevoEstado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cambiar el estado del usuario: " + ex.Message, "Usuarios",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarUsuarios();
        }

        // ===================== Configuracion: Roles =====================

        /// <summary>Trae de la base de datos todos los roles y refresca la grilla de la pestaña Roles.</summary>
        private void CargarRoles()
        {
            List<Roles> roles = _negocioRoles.Listar();

            var tabla = new DataTable();
            tabla.Columns.Add("Id", typeof(int));
            tabla.Columns.Add("Descripcion");
            tabla.Columns.Add("Fecha de creacion");
            tabla.Columns.Add("Estado");

            foreach (Roles r in roles)
            {
                tabla.Rows.Add(
                    r.IdRol,
                    r.Descripcion,
                    r.FechaCreacion.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture),
                    r.estado ? "Activo" : "Inactivo");
            }

            dgvRoles.AutoGenerateColumns = false;
            dgvRoles.DataSource = tabla;
        }

        /// <summary>Rol seleccionado actualmente en la grilla de la pestaña Roles, o null si no hay seleccion.</summary>
        private Roles ObtenerRolSeleccionado()
        {
            if (dgvRoles.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un rol de la lista.", "Roles",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            DataRowView fila = (DataRowView)dgvRoles.CurrentRow.DataBoundItem;
            return _negocioRoles.Listar().Find(r => r.IdRol == Convert.ToInt32(fila["Id"]));
        }

        private void btnNuevoRol_Click(object sender, EventArgs e)
        {
            FrmRol nuevo = new FrmRol();
            nuevo.StartPosition = FormStartPosition.CenterParent;
            if (nuevo.ShowDialog() == DialogResult.OK)
                CargarRoles();
        }

        private void btnEditarRol_Click(object sender, EventArgs e)
        {
            Roles rol = ObtenerRolSeleccionado();
            if (rol == null) return;

            FrmRol editor = new FrmRol(rol);
            editor.StartPosition = FormStartPosition.CenterParent;
            if (editor.ShowDialog() == DialogResult.OK)
                CargarRoles();
        }

        private void btnEstadoRol_Click(object sender, EventArgs e)
        {
            Roles rol = ObtenerRolSeleccionado();
            if (rol == null) return;

            bool nuevoEstado = !rol.estado;
            string accion = nuevoEstado ? "activar" : "desactivar";

            DialogResult confirmacion = MessageBox.Show(
                $"¿Desea {accion} el rol \"{rol.Descripcion}\"?", "Roles",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _negocioRoles.CambiarEstadoRol(rol.IdRol, nuevoEstado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cambiar el estado del rol: " + ex.Message, "Roles",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarRoles();
        }

        // ===================== Configuracion: Monedas =====================

        /// <summary>Trae de la base de datos todas las monedas y refresca la grilla de la pestaña Monedas.</summary>
        private void CargarMonedas()
        {
            List<Moneda> monedas = _negocioMoneda.Listar();

            var tabla = new DataTable();
            tabla.Columns.Add("Id", typeof(int));
            tabla.Columns.Add("Nombre");
            tabla.Columns.Add("Codigo");
            tabla.Columns.Add("Simbolo");
            tabla.Columns.Add("Fecha de creacion");
            tabla.Columns.Add("Estado");

            foreach (Moneda m in monedas)
            {
                tabla.Rows.Add(
                    m.MonedaId,
                    m.Nombre,
                    m.Codigo,
                    m.Simbolo ?? "-",
                    m.FechaCreacion.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture),
                    m.Estado ? "Activo" : "Inactivo");
            }

            dgvMonedas.AutoGenerateColumns = false;
            dgvMonedas.DataSource = tabla;
        }

        /// <summary>Moneda seleccionada actualmente en la grilla de la pestaña Monedas, o null si no hay seleccion.</summary>
        private Moneda ObtenerMonedaSeleccionada()
        {
            if (dgvMonedas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una moneda de la lista.", "Monedas",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            DataRowView fila = (DataRowView)dgvMonedas.CurrentRow.DataBoundItem;
            return _negocioMoneda.Listar().Find(m => m.MonedaId == Convert.ToInt32(fila["Id"]));
        }

        private void btnNuevaMoneda_Click(object sender, EventArgs e)
        {
            FrmMoneda nueva = new FrmMoneda();
            nueva.StartPosition = FormStartPosition.CenterParent;
            if (nueva.ShowDialog() == DialogResult.OK)
                CargarMonedas();
        }

        private void btnEditarMoneda_Click(object sender, EventArgs e)
        {
            Moneda moneda = ObtenerMonedaSeleccionada();
            if (moneda == null) return;

            FrmMoneda editor = new FrmMoneda(moneda);
            editor.StartPosition = FormStartPosition.CenterParent;
            if (editor.ShowDialog() == DialogResult.OK)
                CargarMonedas();
        }

        private void btnEstadoMoneda_Click(object sender, EventArgs e)
        {
            Moneda moneda = ObtenerMonedaSeleccionada();
            if (moneda == null) return;

            bool nuevoEstado = !moneda.Estado;
            string accion = nuevoEstado ? "activar" : "desactivar";

            DialogResult confirmacion = MessageBox.Show(
                $"¿Desea {accion} la moneda \"{moneda.Nombre}\"?", "Monedas",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _negocioMoneda.CambiarEstadoMoneda(moneda.MonedaId, nuevoEstado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cambiar el estado de la moneda: " + ex.Message, "Monedas",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarMonedas();
        }

        // ===================== Configuracion: Formas de Pago =====================

        /// <summary>Trae de la base de datos todas las formas de pago y refresca la grilla de la pestaña Formas de Pago.</summary>
        private void CargarFormasPago()
        {
            List<FormaPago> formasPago = _negocioFormaPago.Listar();

            var tabla = new DataTable();
            tabla.Columns.Add("Id", typeof(int));
            tabla.Columns.Add("Nombre");
            tabla.Columns.Add("Estado");

            foreach (FormaPago f in formasPago)
            {
                tabla.Rows.Add(
                    f.FormaPagoId,
                    f.Nombre,
                    f.Estado ? "Activo" : "Inactivo");
            }

            dgvFormasPago.AutoGenerateColumns = false;
            dgvFormasPago.DataSource = tabla;
        }

        /// <summary>Forma de pago seleccionada actualmente en la grilla, o null si no hay seleccion.</summary>
        private FormaPago ObtenerFormaPagoSeleccionada()
        {
            if (dgvFormasPago.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una forma de pago de la lista.", "Formas de Pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            DataRowView fila = (DataRowView)dgvFormasPago.CurrentRow.DataBoundItem;
            return _negocioFormaPago.Listar().Find(f => f.FormaPagoId == Convert.ToInt32(fila["Id"]));
        }

        private void btnNuevaFormaPago_Click(object sender, EventArgs e)
        {
            FrmFormaPago nueva = new FrmFormaPago();
            nueva.StartPosition = FormStartPosition.CenterParent;
            if (nueva.ShowDialog() == DialogResult.OK)
                CargarFormasPago();
        }

        private void btnEditarFormaPago_Click(object sender, EventArgs e)
        {
            FormaPago formaPago = ObtenerFormaPagoSeleccionada();
            if (formaPago == null) return;

            FrmFormaPago editor = new FrmFormaPago(formaPago);
            editor.StartPosition = FormStartPosition.CenterParent;
            if (editor.ShowDialog() == DialogResult.OK)
                CargarFormasPago();
        }

        private void btnEstadoFormaPago_Click(object sender, EventArgs e)
        {
            FormaPago formaPago = ObtenerFormaPagoSeleccionada();
            if (formaPago == null) return;

            bool nuevoEstado = !formaPago.Estado;
            string accion = nuevoEstado ? "activar" : "desactivar";

            DialogResult confirmacion = MessageBox.Show(
                $"¿Desea {accion} la forma de pago \"{formaPago.Nombre}\"?", "Formas de Pago",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _negocioFormaPago.CambiarEstadoFormaPago(formaPago.FormaPagoId, nuevoEstado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cambiar el estado de la forma de pago: " + ex.Message, "Formas de Pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarFormasPago();
        }

        // ===================== Configuracion: Tipos de Cambio =====================

        /// <summary>Trae de la base de datos todas las tasas (vigentes e historicas) y refresca la grilla.</summary>
        private void CargarTasas()
        {
            List<TasaCambio> tasas = _negocioTasaCambio.Listar();

            var tabla = new DataTable();
            tabla.Columns.Add("Id", typeof(int));
            tabla.Columns.Add("Moneda");
            tabla.Columns.Add("Tipo");
            tabla.Columns.Add("Valor");
            tabla.Columns.Add("Vigente desde");
            tabla.Columns.Add("Estado");

            foreach (TasaCambio t in tasas)
            {
                tabla.Rows.Add(
                    t.TasaId,
                    $"{t.MonedaNombre} ({t.MonedaCodigo})",
                    t.TipoOperacion == "COMPRA" ? "Compra" : "Venta",
                    t.Valor.ToString("N4", CultureInfo.CurrentCulture),
                    t.FechaHora.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture),
                    t.Estado ? "Activo" : "Inactivo");
            }

            dgvTasas.AutoGenerateColumns = false;
            dgvTasas.DataSource = tabla;
        }

        /// <summary>Tasa seleccionada actualmente en la grilla, o null si no hay seleccion.</summary>
        private TasaCambio ObtenerTasaSeleccionada()
        {
            if (dgvTasas.CurrentRow == null)
            {
                MessageBox.Show("Seleccione una tasa de la lista.", "Tipos de Cambio",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            DataRowView fila = (DataRowView)dgvTasas.CurrentRow.DataBoundItem;
            return _negocioTasaCambio.Listar().Find(t => t.TasaId == Convert.ToInt32(fila["Id"]));
        }

        private void btnNuevaTasa_Click(object sender, EventArgs e)
        {
            FrmTasaCambio nueva = new FrmTasaCambio();
            nueva.StartPosition = FormStartPosition.CenterParent;
            if (nueva.ShowDialog() == DialogResult.OK)
            {
                CargarTasas();
                CargarTasasInicio();
            }
        }

        private void btnEditarTasa_Click(object sender, EventArgs e)
        {
            TasaCambio tasa = ObtenerTasaSeleccionada();
            if (tasa == null) return;

            FrmTasaCambio editor = new FrmTasaCambio(tasa);
            editor.StartPosition = FormStartPosition.CenterParent;
            if (editor.ShowDialog() == DialogResult.OK)
            {
                CargarTasas();
                CargarTasasInicio();
            }
        }

        private void btnEstadoTasa_Click(object sender, EventArgs e)
        {
            TasaCambio tasa = ObtenerTasaSeleccionada();
            if (tasa == null) return;

            bool nuevoEstado = !tasa.Estado;
            string accion = nuevoEstado ? "activar" : "desactivar";

            DialogResult confirmacion = MessageBox.Show(
                $"¿Desea {accion} la tasa de {(tasa.TipoOperacion == "COMPRA" ? "Compra" : "Venta")} de \"{tasa.MonedaNombre}\"?",
                "Tipos de Cambio", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _negocioTasaCambio.CambiarEstadoTasa(tasa.TasaId, nuevoEstado);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Tipos de Cambio", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cambiar el estado de la tasa: " + ex.Message, "Tipos de Cambio",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarTasas();
            CargarTasasInicio();
        }

        // ===================== Configuracion: Tipos de Operacion (Ingreso/Egreso) =====================

        /// <summary>Trae de la base de datos todos los tipos de operacion y refresca la grilla de la pestaña Operaciones.</summary>
        private void CargarOperaciones()
        {
            List<Concepto> conceptos = _negocioConcepto.Listar();

            var tabla = new DataTable();
            tabla.Columns.Add("Id", typeof(int));
            tabla.Columns.Add("Codigo");
            tabla.Columns.Add("Nombre");
            tabla.Columns.Add("Tipo");
            tabla.Columns.Add("Estado");

            foreach (Concepto c in conceptos)
            {
                tabla.Rows.Add(
                    c.ConceptoId,
                    c.Operacion,
                    c.Nombre,
                    c.Tipo == "EGRESO" ? "Egreso" : "Ingreso",
                    c.Estado ? "Activo" : "Inactivo");
            }

            dgvOperaciones.AutoGenerateColumns = false;
            dgvOperaciones.DataSource = tabla;
        }

        /// <summary>Tipo de operacion seleccionado actualmente en la grilla, o null si no hay seleccion.</summary>
        private Concepto ObtenerOperacionSeleccionada()
        {
            if (dgvOperaciones.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un tipo de operacion de la lista.", "Operaciones",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            DataRowView fila = (DataRowView)dgvOperaciones.CurrentRow.DataBoundItem;
            return _negocioConcepto.Listar().Find(c => c.ConceptoId == Convert.ToInt32(fila["Id"]));
        }

        private void btnNuevaOperacion_Click(object sender, EventArgs e)
        {
            FrmConcepto nueva = new FrmConcepto();
            nueva.StartPosition = FormStartPosition.CenterParent;
            if (nueva.ShowDialog() == DialogResult.OK)
                CargarOperaciones();
        }

        private void btnEditarOperacion_Click(object sender, EventArgs e)
        {
            Concepto concepto = ObtenerOperacionSeleccionada();
            if (concepto == null) return;

            FrmConcepto editor = new FrmConcepto(concepto);
            editor.StartPosition = FormStartPosition.CenterParent;
            if (editor.ShowDialog() == DialogResult.OK)
                CargarOperaciones();
        }

        private void btnEstadoOperacion_Click(object sender, EventArgs e)
        {
            Concepto concepto = ObtenerOperacionSeleccionada();
            if (concepto == null) return;

            bool nuevoEstado = !concepto.Estado;
            string accion = nuevoEstado ? "activar" : "desactivar";

            DialogResult confirmacion = MessageBox.Show(
                $"¿Desea {accion} el tipo de operacion \"{concepto.Nombre}\"?", "Operaciones",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _negocioConcepto.CambiarEstadoConcepto(concepto.ConceptoId, nuevoEstado);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Operaciones", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cambiar el estado del tipo de operacion: " + ex.Message, "Operaciones",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarOperaciones();
        }

        private void btnEliminarOperacion_Click(object sender, EventArgs e)
        {
            Concepto concepto = ObtenerOperacionSeleccionada();
            if (concepto == null) return;

            DialogResult confirmacion = MessageBox.Show(
                $"¿Desea eliminar definitivamente el tipo de operacion \"{concepto.Nombre}\"? Esta accion no se puede deshacer.",
                "Operaciones", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmacion != DialogResult.Yes) return;

            try
            {
                _negocioConcepto.EliminarConcepto(concepto.ConceptoId);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Operaciones", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo eliminar el tipo de operacion: " + ex.Message, "Operaciones",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CargarOperaciones();
        }

        // ===================== Pestaña Reportes =====================
        // Los controles (botones, combos, DataGridViews) estan declarados en fmrInicio.Designer.cs
        // para poder editarlos visualmente desde el diseñador de Visual Studio.

        /// <summary>Item de un combo de filtro con un Id opcional (null = "Todos/Todas").</summary>
        private class OpcionFiltro
        {
            public int? Id;
            public string Texto;
            public override string ToString() => Texto;
        }

        private void CargarComboCajas(ComboBox combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new OpcionFiltro { Id = null, Texto = "Todas las cajas" });
            foreach (Caja c in _negocioCaja.ListarTodasLasCajas())
                combo.Items.Add(new OpcionFiltro { Id = c.CajaId, Texto = c.Nombre });
            combo.SelectedIndex = 0;
        }

        private void CargarComboUsuarios(ComboBox combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new OpcionFiltro { Id = null, Texto = "Todos los cajeros" });
            foreach (Usuario u in _negocioUsuario.Listar())
                combo.Items.Add(new OpcionFiltro { Id = u.usuario_id, Texto = u.NombreCompleto });
            combo.SelectedIndex = 0;
        }

        private void CargarComboConceptos(ComboBox combo)
        {
            combo.Items.Clear();
            combo.Items.Add(new OpcionFiltro { Id = null, Texto = "Todos los conceptos" });
            foreach (Concepto c in _negocioTransaccion.ListarConceptosActivos())
                combo.Items.Add(new OpcionFiltro { Id = c.ConceptoId, Texto = $"{c.Nombre} ({(c.Tipo == "INGRESO" ? "Ingreso" : "Egreso")})" });
            combo.SelectedIndex = 0;
        }

        /// <summary>Llena los combos de filtro y ejecuta la primera busqueda de ambos reportes. Se llama desde fmrInicio_Load (requiere BD).</summary>
        private void CargarDatosReportes()
        {
            CargarComboCajas(cboMcCaja);
            CargarComboUsuarios(cboMcUsuario);
            CargarReporteMesaCambio();

            CargarComboCajas(cboMpCaja);
            CargarComboUsuarios(cboMpUsuario);
            CargarComboConceptos(cboMpConcepto);
            CargarReporteMovimientosPorConcepto();
        }

        // ---------- Reporte: Mesa de Cambio ----------

        private void btnMcBuscar_Click(object sender, EventArgs e) => CargarReporteMesaCambio();

        private void btnMcExportar_Click(object sender, EventArgs e) => ExportadorCsv.Exportar(this, dgvMcDetalle, "MesaCambio");

        private void CargarReporteMesaCambio()
        {
            DateTime desde = dtpMcDesde.Value.Date;
            DateTime hasta = dtpMcHasta.Value.Date;
            if (hasta < desde)
            {
                MessageBox.Show("La fecha 'Hasta' no puede ser anterior a 'Desde'.", "Mesa de Cambio",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? cajaId = (cboMcCaja.SelectedItem as OpcionFiltro)?.Id;
            int? usuarioId = (cboMcUsuario.SelectedItem as OpcionFiltro)?.Id;

            List<Transaccion> movimientos = _negocioTransaccion.ListarCambiosDivisaParaReporte(desde, hasta, cajaId, usuarioId);

            var detalle = new DataTable();
            detalle.Columns.Add("Fecha");
            detalle.Columns.Add("Caja");
            detalle.Columns.Add("Cajero");
            detalle.Columns.Add("Operacion");
            detalle.Columns.Add("Moneda");
            detalle.Columns.Add("Monto");
            detalle.Columns.Add("Forma de pago");
            detalle.Columns.Add("Descripcion");

            foreach (Transaccion t in movimientos)
            {
                detalle.Rows.Add(
                    t.FechaHora.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture),
                    t.CajaNombre,
                    t.UsuarioNombre,
                    t.Tipo == "INGRESO" ? "Recibido" : "Entregado",
                    $"{t.MonedaNombre} ({t.MonedaCodigo})",
                    t.Monto.ToString("N2", CultureInfo.CurrentCulture),
                    t.FormaPagoNombre ?? "-",
                    t.Descripcion);
            }

            dgvMcDetalle.DataSource = detalle;

            var resumen = new DataTable();
            resumen.Columns.Add("Moneda");
            resumen.Columns.Add("Total Recibido");
            resumen.Columns.Add("Total Entregado");

            var porMoneda = movimientos
                .GroupBy(t => new { t.MonedaCodigo, t.MonedaNombre })
                .Select(g => new
                {
                    g.Key.MonedaCodigo,
                    g.Key.MonedaNombre,
                    Recibido = g.Where(t => t.Tipo == "INGRESO").Sum(t => t.Monto),
                    Entregado = g.Where(t => t.Tipo == "EGRESO").Sum(t => t.Monto)
                })
                .OrderBy(x => x.MonedaCodigo);

            foreach (var m in porMoneda)
            {
                resumen.Rows.Add(
                    $"{m.MonedaNombre} ({m.MonedaCodigo})",
                    m.Recibido.ToString("N2", CultureInfo.CurrentCulture),
                    m.Entregado.ToString("N2", CultureInfo.CurrentCulture));
            }

            dgvMcResumen.DataSource = resumen;
        }

        // ---------- Reporte: Movimientos por Concepto ----------

        private void btnMpBuscar_Click(object sender, EventArgs e) => CargarReporteMovimientosPorConcepto();

        private void btnMpExportar_Click(object sender, EventArgs e) => ExportadorCsv.Exportar(this, dgvMpDetalle, "MovimientosPorConcepto");

        private void CargarReporteMovimientosPorConcepto()
        {
            DateTime desde = dtpMpDesde.Value.Date;
            DateTime hasta = dtpMpHasta.Value.Date;
            if (hasta < desde)
            {
                MessageBox.Show("La fecha 'Hasta' no puede ser anterior a 'Desde'.", "Movimientos por Concepto",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int? cajaId = (cboMpCaja.SelectedItem as OpcionFiltro)?.Id;
            int? usuarioId = (cboMpUsuario.SelectedItem as OpcionFiltro)?.Id;
            int? conceptoId = (cboMpConcepto.SelectedItem as OpcionFiltro)?.Id;
            string tipoFiltro = cboMpTipo.SelectedItem as string ?? "Todos";

            IEnumerable<Transaccion> movimientos = _negocioTransaccion.ListarParaReporte(desde, hasta, cajaId, usuarioId, conceptoId);
            if (tipoFiltro == "Ingreso")
                movimientos = movimientos.Where(t => t.Tipo == "INGRESO");
            else if (tipoFiltro == "Egreso")
                movimientos = movimientos.Where(t => t.Tipo == "EGRESO");
            movimientos = movimientos.ToList();

            var detalle = new DataTable();
            detalle.Columns.Add("Fecha");
            detalle.Columns.Add("Caja");
            detalle.Columns.Add("Cajero");
            detalle.Columns.Add("Concepto");
            detalle.Columns.Add("Tipo");
            detalle.Columns.Add("Moneda");
            detalle.Columns.Add("Monto");
            detalle.Columns.Add("Forma de pago");
            detalle.Columns.Add("Descripcion");

            foreach (Transaccion t in movimientos)
            {
                detalle.Rows.Add(
                    t.FechaHora.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.CurrentCulture),
                    t.CajaNombre,
                    t.UsuarioNombre,
                    t.ConceptoNombre,
                    t.Tipo == "INGRESO" ? "Ingreso" : "Egreso",
                    $"{t.MonedaNombre} ({t.MonedaCodigo})",
                    t.Monto.ToString("N2", CultureInfo.CurrentCulture),
                    t.FormaPagoNombre ?? "-",
                    t.Descripcion);
            }

            dgvMpDetalle.DataSource = detalle;

            var resumen = new DataTable();
            resumen.Columns.Add("Concepto");
            resumen.Columns.Add("Tipo");
            resumen.Columns.Add("Moneda");
            resumen.Columns.Add("Cantidad");
            resumen.Columns.Add("Total");

            var porConcepto = movimientos
                .GroupBy(t => new { t.ConceptoNombre, t.Tipo, t.MonedaCodigo })
                .Select(g => new
                {
                    g.Key.ConceptoNombre,
                    g.Key.Tipo,
                    g.Key.MonedaCodigo,
                    Cantidad = g.Count(),
                    Total = g.Sum(t => t.Monto)
                })
                .OrderBy(x => x.ConceptoNombre).ThenBy(x => x.MonedaCodigo);

            foreach (var c in porConcepto)
            {
                resumen.Rows.Add(
                    c.ConceptoNombre,
                    c.Tipo == "INGRESO" ? "Ingreso" : "Egreso",
                    c.MonedaCodigo,
                    c.Cantidad,
                    c.Total.ToString("N2", CultureInfo.CurrentCulture));
            }

            dgvMpResumen.DataSource = resumen;
        }
    }
}
