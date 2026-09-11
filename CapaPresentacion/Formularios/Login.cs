using System;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    public partial class Login : Form
    {
        private readonly CN_Usuario _negocioUsuario = new CN_Usuario();

        public Login()
        {
            InitializeComponent();
            this.AcceptButton = btningresa;
        }

        private void btningresa_Click(object sender, EventArgs e)
        {
            ResultadoAutenticacion resultado;

            btningresa.Enabled = false;
            this.UseWaitCursor = true;
            try
            {
                resultado = _negocioUsuario.Autenticar(txtusuario.Text, txtclave.Text);
            }
            finally
            {
                this.UseWaitCursor = false;
                btningresa.Enabled = true;
            }

            if (!resultado.Exitoso)
            {
                MessageBox.Show(resultado.Mensaje, "No se pudo iniciar sesión",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtclave.Clear();
                txtclave.Focus();
                return;
            }

            SesionActual.Iniciar(resultado.Usuario);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
