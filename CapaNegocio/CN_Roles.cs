using System;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Roles
    {
        private readonly CD_Roles _datosRoles = new CD_Roles();

        /// <summary>Todos los roles (activos e inactivos), para la pantalla de administracion.</summary>
        public List<Roles> Listar() => _datosRoles.Listar();

        /// <summary>Roles activos, para el combo de seleccion al crear/editar un usuario.</summary>
        public List<Roles> ListarActivos() => _datosRoles.ListarActivos();

        /// <summary>Registra un nuevo rol.</summary>
        /// <exception cref="ArgumentException">Si falta la descripcion.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe un rol con esa descripcion.</exception>
        public Roles RegistrarRol(string descripcion)
        {
            ValidarDescripcion(descripcion);
            descripcion = descripcion.Trim();

            if (_datosRoles.ExisteDescripcion(descripcion))
                throw new InvalidOperationException("Ya existe un rol con esa descripcion.");

            var rol = new Roles { Descripcion = descripcion, estado = true };
            rol.IdRol = _datosRoles.Registrar(descripcion);
            return rol;
        }

        /// <summary>Actualiza la descripcion de un rol existente.</summary>
        /// <exception cref="ArgumentException">Si falta la descripcion.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe otro rol con esa descripcion.</exception>
        public void ActualizarRol(int idRol, string descripcion)
        {
            ValidarDescripcion(descripcion);
            descripcion = descripcion.Trim();

            if (_datosRoles.ExisteDescripcion(descripcion, idRol))
                throw new InvalidOperationException("Ya existe otro rol con esa descripcion.");

            _datosRoles.Actualizar(idRol, descripcion);
        }

        /// <summary>Activa o desactiva un rol.</summary>
        public void CambiarEstadoRol(int idRol, bool estado) => _datosRoles.CambiarEstado(idRol, estado);

        private static void ValidarDescripcion(string descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                throw new ArgumentException("Debe indicar la descripcion del rol.");
        }
    }
}
