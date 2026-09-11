using System;
using System.Collections.Generic;
using System.Diagnostics;
using CapaDatos;
using CapaEntidad;
using CapaNegocio.Seguridad;

namespace CapaNegocio
{
    public class CN_Usuario
    {
        private readonly CD_Usuario objcd_usuario = new CD_Usuario();

        public List<Usuario> Listar()
        {
            return objcd_usuario.listar();
        }

        /// <summary>
        /// Valida las credenciales de un usuario. No revela si el fallo fue por
        /// usuario inexistente o contraseña incorrecta (mismo mensaje para ambos).
        /// </summary>
        public ResultadoAutenticacion Autenticar(string usuario, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrEmpty(contrasena))
            {
                return ResultadoAutenticacion.Fallo(
                    MotivoAutenticacion.DatosIncompletos,
                    "Ingrese usuario y contraseña.");
            }

            Usuario encontrado;
            try
            {
                encontrado = objcd_usuario.ObtenerPorUsuario(usuario);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al autenticar: " + ex);
                return ResultadoAutenticacion.Fallo(
                    MotivoAutenticacion.ErrorInterno,
                    "No se pudo conectar con la base de datos. Intente nuevamente o avise al administrador.");
            }

            bool credencialesValidas =
                encontrado != null &&
                PasswordHasher.Verificar(contrasena, encontrado.password_hash);

            if (!credencialesValidas)
            {
                return ResultadoAutenticacion.Fallo(
                    MotivoAutenticacion.CredencialesInvalidas,
                    "Usuario o contraseña incorrectos.");
            }

            if (!encontrado.estado)
            {
                return ResultadoAutenticacion.Fallo(
                    MotivoAutenticacion.UsuarioInactivo,
                    "La cuenta está inactiva. Contacte al administrador.");
            }

            encontrado.password_hash = null; // no propagar el hash a la capa de presentación
            return ResultadoAutenticacion.Ok(encontrado);
        }
    }
}
