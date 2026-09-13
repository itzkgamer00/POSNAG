using System;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion.Formularios
{
    public partial class Frmcajaregistradora : Form
    {
        private readonly CN_AperturaCaja _negocio = new CN_AperturaCaja();

        /// <summary>Caja que se esta editando, o null si el formulario esta creando una nueva.</summary>
        private readonly Caja _cajaEditando;

        /// <summary>Modo creacion: alta de una caja nueva.</summary>
        public Frmcajaregistradora()
        {
            InitializeComponent();
        }

        /// <summary>Modo edicion: renombrar/activar-desactivar una caja existente.</summary>
        public Frmcajaregistradora(Caja caja)
        {
            InitializeComponent();

            _cajaEditando = caja ?? throw new ArgumentNullException(nameof(caja));

            label3.Text = "Editar Caja Registradora";
            label1.Text = "Modifique los datos de la caja.";
            guna2TextBox1.Text = caja.Nombre;
            chkActiva.Visible = true;
            chkActiva.Checked = caja.Estado;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            string nombre = guna2TextBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("Ingrese el nombre de la caja.", "Caja Registradora",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox1.Focus();
                return;
            }

            try
            {
                if (_cajaEditando == null)
                {
                    _negocio.RegistrarCaja(nombre);
                }
                else
                {
                    _negocio.ActualizarCaja(_cajaEditando.CajaId, nombre);
                    if (chkActiva.Checked != _cajaEditando.Estado)
                        _negocio.CambiarEstadoCaja(_cajaEditando.CajaId, chkActiva.Checked);
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
            {
                MessageBox.Show(ex.Message, "Caja Registradora", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar la caja: " + ex.Message, "Caja Registradora",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btncerrarcjregist_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
