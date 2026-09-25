using System;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    /// <summary>
    /// Dialogo modal que solicita credenciales de un Administrador para autorizar una operacion sensible.
    /// Devuelve DialogResult.OK solo si las credenciales son validas y el usuario tiene rol Administrador.
    /// </summary>
    public partial class FrmAutorizacionAdmin : Form
    {
        private readonly CN_Usuario _negocioUsuario = new CN_Usuario();

        /// <summary>Administrador que autorizo la operacion. Solo tiene valor si el resultado fue OK.</summary>
        public Usuario AdministradorAutorizante { get; private set; }

        /// <summary>Constructor sin parametros requerido por el diseñador de Visual Studio.</summary>
        public FrmAutorizacionAdmin()
        {
            InitializeComponent();
        }

        public FrmAutorizacionAdmin(string motivo) : this()
        {
            lblMotivo.Text = motivo;
        }

        private void btnAutorizar_Click(object sender, EventArgs e)
        {
            ResultadoAutenticacion resultado;

            btnAutorizar.Enabled = false;
            UseWaitCursor = true;
            try
            {
                resultado = _negocioUsuario.AutorizarAdministrador(txtUsuario.Text, txtClave.Text);
            }
            finally
            {
                UseWaitCursor = false;
                btnAutorizar.Enabled = true;
            }

            if (!resultado.Exitoso)
            {
                MessageBox.Show(resultado.Mensaje, "Autorización denegada",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClave.Clear();
                txtClave.Focus();
                return;
            }

            AdministradorAutorizante = resultado.Usuario;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
