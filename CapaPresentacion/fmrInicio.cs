using CapaPresentacion.Formularios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class fmrInicio : Form
    {
        public fmrInicio()
        {
            InitializeComponent();
        }

        private void btnControlEfectivo_Click(object sender, EventArgs e)
        {
            //Cerrar cualquier formulario que ya esté en el panel.
            foreach (Control control in PanelContenedor.Controls)
            {
                control.Dispose();
            }

            //Crear instancia del formulario secundario.
           FrmlEntrada formControlEfectivo = new FrmlEntrada
           {
               TopLevel = false, // Para que se comporte como un control en el panel
               Dock = DockStyle.Fill // Para que ocupe todo el espacio del panel
           };

            PanelContenedor.Controls.Add(formControlEfectivo);
            PanelContenedor.Tag = formControlEfectivo;
            formControlEfectivo.Show();
            //FrmlEntrada efectivo = new FrmlEntrada(); // Crear una instancia del fmrcaja
            //efectivo.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
            //efectivo.ShowDialog(); // Mostrarlo como emergente

        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            //Cerrar cualquier formulario que ya esté en el panel.
            foreach (Control control in PanelContenedor.Controls)
            {
                control.Dispose();
            }

            //Crear instancia del formulario secundario.
            Dashboard dash = new Dashboard
            {
                TopLevel = false, // Para que se comporte como un control en el panel
                Dock = DockStyle.Fill // Para que ocupe todo el espacio del panel
            };

            PanelContenedor.Controls.Add(dash);
            PanelContenedor.Tag = dash;
            dash.Show();
        }

      

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void fmrInicio_Load(object sender, EventArgs e)
        {

        }

        private void tmTiempo_Tick(object sender, EventArgs e)
        {
           lblfecha.Text = DateTime.Now.ToLongDateString();
           lblHora.Text = DateTime.Now.ToLongTimeString();
        }

        
        private void PanelContenedor_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnlog_Click(object sender, EventArgs e)
        {
            DialogResult resultado = System.Windows.Forms.MessageBox.Show("¿Estás seguro que deseas cerrar sesión?", "Cerrar sesión", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question
);

            if (resultado == System.Windows.Forms.DialogResult.Yes)
            {
                this.Hide(); // Oculta el formulario actual
                Login login = new Login();
                login.Show();
            }
        }
    }
}
