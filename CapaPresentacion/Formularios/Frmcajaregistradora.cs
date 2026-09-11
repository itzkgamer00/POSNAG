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
    public partial class Frmcajaregistradora : Form
    {
        public Frmcajaregistradora()
        {
            InitializeComponent();
        }

        private void btncerrarcjregist_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
