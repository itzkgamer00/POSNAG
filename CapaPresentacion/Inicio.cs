﻿using System;
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
    public partial class Inicio : Form
    {
        public Inicio()
        {
            InitializeComponent();
        }

        private void btnControlEfectivo_Click(object sender, EventArgs e)
        {
            // Cerrar cualquier formulario que ya esté en el panel.
            foreach (Control control in PanelContenedor.Controls)
            {
                control.Dispose();
            }

            // Crear instancia del formulario secundario.
            Entrada formControlEfectivo = new Entrada
            {
                TopLevel = false, // Para que se comporte como un control en el panel
                Dock = DockStyle.Fill // Para que ocupe todo el espacio del panel
            };

            PanelContenedor.Controls.Add(formControlEfectivo);
            PanelContenedor.Tag = formControlEfectivo;
            formControlEfectivo.Show();


        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            // Recorre cada control en el panel y verifica si es un formulario
            foreach (Control control in PanelContenedor.Controls)
            {
                if (control is Form)
                {
                    // Cierra el formulario
                    Form formularioSecundario = (Form)control;
                    formularioSecundario.Close();
                }
            }

            // Limpia el panel de cualquier control restante
            PanelContenedor.Controls.Clear();
        }
    }
}
