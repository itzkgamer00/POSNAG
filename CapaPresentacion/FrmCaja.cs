using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class FrmCaja : Form
    {
        public FrmCaja()
        {
            InitializeComponent();
        }

        private void btnaptcaja_Click(object sender, EventArgs e)
        {
            // Cerrar cualquier formulario que ya esté en el panel.
            foreach (Control control in PanelContenedorCaja.Controls)
            {
                control.Dispose();
            }

            // Crear instancia del formulario secundario.
            FrmApertura formControlEfectivo = new FrmApertura
            {
                TopLevel = false, // Para que se comporte como un control en el panel
                Dock = DockStyle.Fill // Para que ocupe todo el espacio del panel
            };

            PanelContenedorCaja.Controls.Add(formControlEfectivo);
            PanelContenedorCaja.Tag = formControlEfectivo;
            formControlEfectivo.Show();

        }
    }
}
