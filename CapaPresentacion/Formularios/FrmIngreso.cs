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
    public partial class FrmIngreso : Form
    {
        public FrmIngreso()
        {
            InitializeComponent();
        }

        

        private void btndetalleingre_Click(object sender, EventArgs e)
        {
            FrmDetalle detal = new FrmDetalle(); // Crear una instancia del fmrcaja
            detal.StartPosition = FormStartPosition.CenterScreen; // Centrar el formulario emergente
            detal.ShowDialog(); // Mostrarlo como emergente
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}
