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
        private AperturaCaja _aperturaActual;
        private List<Transaccion> _movimientosActuales = new List<Transaccion>();

        public fmrInicio()
        {
            InitializeComponent();

            guna2DataGridView6.ReadOnly = true;
            guna2DataGridView6.AllowUserToAddRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.AllowUserToAddRows = false;

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
            CargarMovimientos();
            CargarHistorial();
            CargarCajas();
            CargarDatosReportes();
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
                LlenarGridInicio(movimientos);
            else
                LlenarGridMovimientos(movimientos);
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
            guna2DataGridView1.Rows.Clear();
            foreach (Transaccion t in movimientos)
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
