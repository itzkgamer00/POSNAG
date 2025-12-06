using CapaPresentacion.Formularios;
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
    public partial class FrmlEntrada : Form
    {
        public FrmlEntrada()
        {
            InitializeComponent();
        }

        private void Entrada_Load(object sender, EventArgs e)
        {

        }

        private void MesaCambio_Click(object sender, EventArgs e)
        {
            {
                Frmlcambiodivisas entrada = new Frmlcambiodivisas(); // Crear una instancia de Form2
                entrada.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
                entrada.ShowDialog(); // Mostrarlo como emergente
            }
        }

        private void btndetallein_Click(object sender, EventArgs e)
        {
            {
                FrmDetalle detalle1 = new FrmDetalle(); // Crear una instancia de Form2
                detalle1.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
                detalle1.ShowDialog(); // Mostrarlo como emergente
            }
        }

        private void btningr_Click(object sender, EventArgs e)
        {
            {
                FrmIngreso ingre = new FrmIngreso(); // Crear una instancia del fmrcaja
                ingre.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
                ingre.ShowDialog(); // Mostrarlo como emergente
            }
        }

        private void btnegre_Click(object sender, EventArgs e)
        {
            FrmSalida sali = new FrmSalida(); // Crear una instancia del fmrcaja
            sali.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
            sali.ShowDialog(); // Mostrarlo como emergente
        }
    }
}
