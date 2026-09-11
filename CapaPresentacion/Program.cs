using System;
using System.Windows.Forms;
using CapaNegocio;

namespace CapaPresentacion
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal. Alterna entre el login y la ventana principal:
        /// al cerrar sesión se vuelve a pedir el login; al cerrar la ventana principal
        /// (sin cerrar sesión) termina la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                using (var login = new Login())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                        break; // el usuario cerró el login: salir
                }

                bool volverALogin;
                using (var inicio = new fmrInicio(SesionActual.Usuario))
                {
                    inicio.ShowDialog();
                    volverALogin = inicio.CerrarSesionSolicitado;
                }

                SesionActual.Cerrar();

                if (!volverALogin)
                    break; // se cerró la ventana principal: salir
            }
        }
    }
}
