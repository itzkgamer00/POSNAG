using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion.Formularios
{
    public partial class FrmSalida : Form
    {
        public FrmSalida()
        {
            InitializeComponent();
        }

        private void btndetsalida_Click(object sender, EventArgs e)
        {
            FrmDetalle detalsal = new FrmDetalle(); // Crear una instancia del fmrcaja
            detalsal.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
            detalsal.ShowDialog(); // Mostrarlo como emergente
        }

        private void btnegredetalle_Click(object sender, EventArgs e)
        {
            FrmDetalle detal = new FrmDetalle(); // Crear una instancia del fmrcaja
            detal.StartPosition = FormStartPosition.CenterScreen; // Centrar el formulario emergente
            detal.ShowDialog(); // Mostrarlo como emergente
        }
    }
}
