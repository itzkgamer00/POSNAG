using System;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class FrmRol : Form
    {
        private readonly CN_Roles _negocio = new CN_Roles();

        /// <summary>Rol que se esta editando, o null si el formulario esta creando uno nuevo.</summary>
        private readonly Roles _rolEditando;

        /// <summary>Modo creacion: alta de un rol nuevo.</summary>
        public FrmRol()
        {
            InitializeComponent();
        }

        /// <summary>Modo edicion: renombrar/activar-desactivar un rol existente.</summary>
        public FrmRol(Roles rol)
        {
            InitializeComponent();

            _rolEditando = rol ?? throw new ArgumentNullException(nameof(rol));

            lblTitulo.Text = "Editar Rol";
            lblSubtitulo.Text = "Modifique los datos del rol.";
            txtDescripcion.Text = rol.Descripcion;
            chkActivo.Visible = true;
            chkActivo.Checked = rol.estado;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string descripcion = txtDescripcion.Text.Trim();
            bool esNuevo = _rolEditando == null;

            try
            {
                if (esNuevo)
                {
                    _negocio.RegistrarRol(descripcion);
                }
                else
                {
                    _negocio.ActualizarRol(_rolEditando.IdRol, descripcion);

                    if (chkActivo.Checked != _rolEditando.estado)
                        _negocio.CambiarEstadoRol(_rolEditando.IdRol, chkActivo.Checked);
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                MessageBox.Show(ex.Message, "Rol", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el rol: " + ex.Message, "Rol",
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
