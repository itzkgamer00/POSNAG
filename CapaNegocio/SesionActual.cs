using System;
using CapaEntidad;

namespace CapaNegocio
{
    /// <summary>
    /// Contexto del usuario autenticado durante la sesión de la aplicación.
    /// Se establece al iniciar sesión y se limpia al cerrarla.
    /// </summary>
    public static class SesionActual
    {
        public static Usuario Usuario { get; private set; }

        public static DateTime? InicioSesion { get; private set; }

        public static bool HaySesion => Usuario != null;

        public static void Iniciar(Usuario usuario)
        {
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            InicioSesion = DateTime.Now;
        }

        public static void Cerrar()
        {
            Usuario = null;
            InicioSesion = null;
        }

        /// <summary>Indica si el usuario actual pertenece al rol indicado (por Id).</summary>
        public static bool EsRol(int idRol) => Usuario != null && Usuario.IdRol == idRol;
    }
}
