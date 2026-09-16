using System;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class FrmMoneda : Form
    {
        private readonly CN_Moneda _negocio = new CN_Moneda();

        /// <summary>Moneda que se esta editando, o null si el formulario esta creando una nueva.</summary>
        private readonly Moneda _monedaEditando;

        /// <summary>Modo creacion: alta de una moneda nueva.</summary>
        public FrmMoneda()
        {
            InitializeComponent();
        }

        /// <summary>Modo edicion: renombrar/cambiar codigo o simbolo/activar-desactivar una moneda existente.</summary>
        public FrmMoneda(Moneda moneda)
        {
            InitializeComponent();

            _monedaEditando = moneda ?? throw new ArgumentNullException(nameof(moneda));

            lblTitulo.Text = "Editar Moneda";
            lblSubtitulo.Text = "Modifique los datos de la moneda.";
            txtNombre.Text = moneda.Nombre;
            txtCodigo.Text = moneda.Codigo;
            txtSimbolo.Text = moneda.Simbolo;
            chkActiva.Visible = true;
            chkActiva.Checked = moneda.Estado;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string codigo = txtCodigo.Text.Trim();
            string simbolo = txtSimbolo.Text.Trim();
            bool esNueva = _monedaEditando == null;

            try
            {
                if (esNueva)
                {
                    _negocio.RegistrarMoneda(nombre, codigo, simbolo);
                }
                else
                {
                    _negocio.ActualizarMoneda(_monedaEditando.MonedaId, nombre, codigo, simbolo);

                    if (chkActiva.Checked != _monedaEditando.Estado)
                        _negocio.CambiarEstadoMoneda(_monedaEditando.MonedaId, chkActiva.Checked);
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                MessageBox.Show(ex.Message, "Moneda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar la moneda: " + ex.Message, "Moneda",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
