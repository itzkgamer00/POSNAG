using System;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class FrmFormaPago : Form
    {
        private readonly CN_FormaPago _negocio = new CN_FormaPago();

        /// <summary>Forma de pago que se esta editando, o null si el formulario esta creando una nueva.</summary>
        private readonly FormaPago _formaPagoEditando;

        /// <summary>Modo creacion: alta de una forma de pago nueva.</summary>
        public FrmFormaPago()
        {
            InitializeComponent();
        }

        /// <summary>Modo edicion: renombrar/activar-desactivar una forma de pago existente.</summary>
        public FrmFormaPago(FormaPago formaPago)
        {
            InitializeComponent();

            _formaPagoEditando = formaPago ?? throw new ArgumentNullException(nameof(formaPago));

            lblTitulo.Text = "Editar Forma de Pago";
            lblSubtitulo.Text = "Modifique los datos de la forma de pago.";
            txtNombre.Text = formaPago.Nombre;
            chkActiva.Visible = true;
            chkActiva.Checked = formaPago.Estado;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            bool esNueva = _formaPagoEditando == null;

            try
            {
                if (esNueva)
                {
                    _negocio.RegistrarFormaPago(nombre);
                }
                else
                {
                    _negocio.ActualizarFormaPago(_formaPagoEditando.FormaPagoId, nombre);

                    if (chkActiva.Checked != _formaPagoEditando.Estado)
                        _negocio.CambiarEstadoFormaPago(_formaPagoEditando.FormaPagoId, chkActiva.Checked);
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                MessageBox.Show(ex.Message, "Forma de Pago", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar la forma de pago: " + ex.Message, "Forma de Pago",
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
