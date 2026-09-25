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
        private readonly CD_Roles _datosRoles = new CD_Roles();

        public List<Usuario> Listar()
        {
            return objcd_usuario.listar();
        }

        /// <summary>Roles activos, para el combo de seleccion al crear/editar un usuario.</summary>
        public List<Roles> ListarRolesActivos() => _datosRoles.ListarActivos();

        /// <summary>Registra un nuevo usuario.</summary>
        /// <exception cref="ArgumentException">Si falta el nombre, el usuario, o la contraseña no cumple el minimo.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe un usuario con ese nombre de usuario.</exception>
        public Usuario RegistrarUsuario(string nombreCompleto, string nombreUsuario, string contrasena, int? idRol)
        {
            ValidarDatosBasicos(nombreCompleto, nombreUsuario);
            ValidarContrasena(contrasena);

            nombreCompleto = nombreCompleto.Trim();
            nombreUsuario = nombreUsuario.Trim();

            if (objcd_usuario.ExisteUsuario(nombreUsuario))
                throw new InvalidOperationException("Ya existe un usuario con ese nombre de usuario.");

            var usuario = new Usuario
            {
                NombreCompleto = nombreCompleto,
                usuario = nombreUsuario,
                password_hash = PasswordHasher.Hash(contrasena),
                IdRol = idRol ?? 0,
                estado = true
            };

            usuario.usuario_id = objcd_usuario.Registrar(usuario);
            return usuario;
        }

        /// <summary>Actualiza nombre, usuario y rol de un usuario existente. La contraseña se cambia con CambiarContrasena.</summary>
        /// <exception cref="ArgumentException">Si falta el nombre o el usuario.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe otro usuario con ese nombre de usuario.</exception>
        public void ActualizarUsuario(int usuarioId, string nombreCompleto, string nombreUsuario, int? idRol)
        {
            ValidarDatosBasicos(nombreCompleto, nombreUsuario);

            nombreCompleto = nombreCompleto.Trim();
            nombreUsuario = nombreUsuario.Trim();

            if (objcd_usuario.ExisteUsuario(nombreUsuario, usuarioId))
                throw new InvalidOperationException("Ya existe un usuario con ese nombre de usuario.");

            objcd_usuario.Actualizar(usuarioId, nombreCompleto, nombreUsuario, idRol);
        }

        /// <summary>Cambia la contraseña de un usuario existente.</summary>
        /// <exception cref="ArgumentException">Si la contraseña no cumple el minimo de longitud.</exception>
        public void CambiarContrasena(int usuarioId, string nuevaContrasena)
        {
            ValidarContrasena(nuevaContrasena);
            objcd_usuario.ActualizarPassword(usuarioId, PasswordHasher.Hash(nuevaContrasena));
        }

        /// <summary>Activa o desactiva un usuario.</summary>
        public void CambiarEstadoUsuario(int usuarioId, bool estado) => objcd_usuario.CambiarEstado(usuarioId, estado);

        private static void ValidarDatosBasicos(string nombreCompleto, string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                throw new ArgumentException("Debe indicar el nombre completo.");
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                throw new ArgumentException("Debe indicar el nombre de usuario.");
        }

        private static void ValidarContrasena(string contrasena)
        {
            if (string.IsNullOrWhiteSpace(contrasena) || contrasena.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");
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

        /// <summary>
        /// Valida credenciales para autorizar una operación sensible (p. ej. anular un movimiento).
        /// Solo es exitoso si las credenciales son válidas y el usuario tiene rol Administrador.
        /// </summary>
        public ResultadoAutenticacion AutorizarAdministrador(string usuario, string contrasena)
        {
            ResultadoAutenticacion resultado = Autenticar(usuario, contrasena);
            if (!resultado.Exitoso)
                return resultado;

            bool esAdministrador = string.Equals(
                resultado.Usuario.RolDescripcion, "Administrador", StringComparison.OrdinalIgnoreCase);

            if (!esAdministrador)
            {
                return ResultadoAutenticacion.Fallo(
                    MotivoAutenticacion.CredencialesInvalidas,
                    "El usuario indicado no tiene permisos de Administrador.");
            }

            return resultado;
        }
    }
}
