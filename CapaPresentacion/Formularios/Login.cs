using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    public partial class Login : Form
    {
        

        public Login()
        {
            InitializeComponent();
    
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btningresar_Click(object sender, EventArgs e)
        {

            List<Usuario> TEST = new CN_Usuario().Listar();

            Usuario ousuario = (Usuario)new CN_Usuario().Listar().Where(u => u.usuario == txtusuario.Text && u.password_hash == txtclave.Text).FirstOrDefault();

            if (ousuario != null)
            {
                MessageBox.Show($"✅ Bienvenido {ousuario.NombreCompleto}");

                fmrInicio frm = new fmrInicio();
                frm.Show();
                this.Hide();
                frm.FormClosing += frm_closing; 

            }
            else
            {
                MessageBox.Show("❌ Usuario o contraseña incorrectos");
            }

            
        }

        private void frm_closing(object sender, FormClosingEventArgs e)
        {
            txtclave.Text = "";
            txtusuario.Text = "";
            this.Show();
        }

        
    }
}
