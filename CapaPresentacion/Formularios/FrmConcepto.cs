using System;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class FrmConcepto : Form
    {
        private readonly CN_Concepto _negocio = new CN_Concepto();

        /// <summary>Concepto que se esta editando, o null si el formulario esta creando uno nuevo.</summary>
        private readonly Concepto _conceptoEditando;

        /// <summary>Modo creacion: alta de un tipo de operacion nuevo.</summary>
        public FrmConcepto()
        {
            InitializeComponent();
            cboTipo.SelectedIndex = 0;
        }

        /// <summary>Modo edicion: renombrar/cambiar codigo o tipo/activar-desactivar un tipo de operacion existente.</summary>
        public FrmConcepto(Concepto concepto)
        {
            InitializeComponent();

            _conceptoEditando = concepto ?? throw new ArgumentNullException(nameof(concepto));

            lblTitulo.Text = "Editar Tipo de Operacion";
            lblSubtitulo.Text = "Modifique los datos del tipo de operacion.";
            txtOperacion.Text = concepto.Operacion;
            txtNombre.Text = concepto.Nombre;
            cboTipo.SelectedItem = concepto.Tipo == "EGRESO" ? "EGRESO" : "INGRESO";
            chkActivo.Visible = true;
            chkActivo.Checked = concepto.Estado;

            if (_negocio.EsOperacionDeCambioDivisa(concepto.Operacion))
            {
                lblSubtitulo.Text = "Este tipo lo usa Mesa de Cambio: solo se puede renombrar.";
                txtOperacion.Enabled = false;
                cboTipo.Enabled = false;
                chkActivo.Enabled = false;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string operacion = txtOperacion.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string tipo = cboTipo.SelectedItem as string;
            bool esNuevo = _conceptoEditando == null;

            try
            {
                if (esNuevo)
                {
                    _negocio.RegistrarConcepto(operacion, nombre, tipo);
                }
                else
                {
                    _negocio.ActualizarConcepto(_conceptoEditando.ConceptoId, operacion, nombre, tipo);

                    if (chkActivo.Checked != _conceptoEditando.Estado)
                        _negocio.CambiarEstadoConcepto(_conceptoEditando.ConceptoId, chkActivo.Checked);
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                MessageBox.Show(ex.Message, "Tipo de Operacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el tipo de operacion: " + ex.Message, "Tipo de Operacion",
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
