using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class FrmUsuario : Form
    {
        private readonly CN_Usuario _negocio = new CN_Usuario();

        /// <summary>Usuario que se esta editando, o null si el formulario esta creando uno nuevo.</summary>
        private readonly Usuario _usuarioEditando;

        /// <summary>Modo creacion: alta de un usuario nuevo.</summary>
        public FrmUsuario()
        {
            InitializeComponent();
            CargarRoles();
        }

        /// <summary>Modo edicion: renombrar/cambiar rol/activar-desactivar/cambiar contraseña de un usuario existente.</summary>
        public FrmUsuario(Usuario usuario)
        {
            InitializeComponent();
            CargarRoles();

            _usuarioEditando = usuario ?? throw new ArgumentNullException(nameof(usuario));

            lblTitulo.Text = "Editar Usuario";
            lblSubtitulo.Text = "Modifique los datos del usuario. Deje la contraseña en blanco para no cambiarla.";
            txtNombreCompleto.Text = usuario.NombreCompleto;
            txtUsuario.Text = usuario.usuario;
            SeleccionarRolActual(usuario.IdRol);
            chkActivo.Visible = true;
            chkActivo.Checked = usuario.estado;
        }

        private void CargarRoles()
        {
            List<Roles> roles = _negocio.ListarRolesActivos();
            cboRol.DataSource = roles;
            cboRol.DisplayMember = nameof(Roles.Descripcion);
            cboRol.ValueMember = nameof(Roles.IdRol);
            cboRol.SelectedIndex = roles.Count > 0 ? 0 : -1;
        }

        private void SeleccionarRolActual(int idRol)
        {
            if (!(cboRol.DataSource is List<Roles> roles)) return;

            Roles rol = roles.Find(r => r.IdRol == idRol);
            if (rol != null)
                cboRol.SelectedValue = rol.IdRol;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombreCompleto = txtNombreCompleto.Text.Trim();
            string nombreUsuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;
            string confirmacion = txtConfirmarContrasena.Text;
            int? idRol = cboRol.SelectedValue == null ? (int?)null : Convert.ToInt32(cboRol.SelectedValue);

            bool esNuevo = _usuarioEditando == null;

            if (!esNuevo && string.IsNullOrEmpty(contrasena) && string.IsNullOrEmpty(confirmacion))
            {
                // Edicion sin cambio de contraseña: se omite la validacion de confirmacion.
            }
            else if (contrasena != confirmacion)
            {
                MessageBox.Show("La contraseña y su confirmación no coinciden.", "Usuario",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtConfirmarContrasena.Focus();
                return;
            }

            try
            {
                if (esNuevo)
                {
                    _negocio.RegistrarUsuario(nombreCompleto, nombreUsuario, contrasena, idRol);
                }
                else
                {
                    _negocio.ActualizarUsuario(_usuarioEditando.usuario_id, nombreCompleto, nombreUsuario, idRol);

                    if (!string.IsNullOrEmpty(contrasena))
                        _negocio.CambiarContrasena(_usuarioEditando.usuario_id, contrasena);

                    if (chkActivo.Checked != _usuarioEditando.estado)
                        _negocio.CambiarEstadoUsuario(_usuarioEditando.usuario_id, chkActivo.Checked);
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                MessageBox.Show(ex.Message, "Usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el usuario: " + ex.Message, "Usuario",
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
