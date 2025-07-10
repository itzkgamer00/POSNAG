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
            Usuario ousuario = (Usuario)new CN_Usuario().Listar().Where(u => u.usuario == txtusuario.Text && u.password_hash == txtclave.Text).FirstOrDefault();

        }

        private void frm_closing(object sender, FormClosingEventArgs e)
        {
            txtclave.Text = "";
            txtusuario.Text = "";
            this.Show();
        }

        private void btnProbarConexion_Click(object sender, EventArgs e)
        {
            string connectionString =   "Server=localhost;Database=SistemaCaja;User Id=sa;Password=kenrrichpg0621;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    MessageBox.Show("✅ Conexión exitosa a la base de datos.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Error al conectar: " + ex.Message);
                }
            }
        }
    }
}
