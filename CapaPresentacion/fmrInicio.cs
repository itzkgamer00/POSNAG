using CapaPresentacion.Formularios;
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
            {
                Frmlcambiodivisas entrada = new Frmlcambiodivisas(); // Crear una instancia de Form2
                entrada.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
                entrada.ShowDialog(); // Mostrarlo como emergente
            }
        }

        private void btncajaregistradora_Click(object sender, EventArgs e)
        {
            Frmcajaregistradora cajaregis = new Frmcajaregistradora(); // Crear una instancia de Form2
            cajaregis.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
            cajaregis.ShowDialog(); // Mostrarlo como emergente
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
    }
}
