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
    public partial class Entrada : Form
    {
        public Entrada()
        {
            InitializeComponent();
        }

        private void Entrada_Load(object sender, EventArgs e)
        {

        }

        private void MesaCambio_Click(object sender, EventArgs e)
        {
            {
                Cambiodivisas entrada = new Cambiodivisas(); // Crear una instancia de Form2
                entrada.StartPosition = FormStartPosition.CenterParent; // Centrar el formulario emergente
                entrada.ShowDialog(); // Mostrarlo como emergente
            }
        }
    }
}
