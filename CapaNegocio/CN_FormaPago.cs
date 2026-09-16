using System;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_FormaPago
    {
        private readonly CD_FormaPago _datosFormaPago = new CD_FormaPago();

        /// <summary>Todas las formas de pago (activas e inactivas), para la pantalla de administracion.</summary>
        public List<FormaPago> Listar() => _datosFormaPago.Listar();

        /// <summary>Formas de pago activas, para combos de seleccion (mesa de cambio, ingresos/egresos, etc.).</summary>
        public List<FormaPago> ListarActivas() => _datosFormaPago.ListarActivas();

        /// <summary>Registra una nueva forma de pago.</summary>
        /// <exception cref="ArgumentException">Si falta el nombre.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe una forma de pago con ese nombre.</exception>
        public FormaPago RegistrarFormaPago(string nombre)
        {
            nombre = ValidarNombre(nombre);

            if (_datosFormaPago.ExisteNombre(nombre))
                throw new InvalidOperationException("Ya existe una forma de pago con ese nombre.");

            var formaPago = new FormaPago { Nombre = nombre, Estado = true };
            formaPago.FormaPagoId = _datosFormaPago.Registrar(nombre);
            return formaPago;
        }

        /// <summary>Renombra una forma de pago existente.</summary>
        /// <exception cref="ArgumentException">Si falta el nombre.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe otra forma de pago con ese nombre.</exception>
        public void ActualizarFormaPago(int formaPagoId, string nombre)
        {
            nombre = ValidarNombre(nombre);

            if (_datosFormaPago.ExisteNombre(nombre, formaPagoId))
                throw new InvalidOperationException("Ya existe otra forma de pago con ese nombre.");

            _datosFormaPago.Actualizar(formaPagoId, nombre);
        }

        /// <summary>Activa o desactiva una forma de pago.</summary>
        public void CambiarEstadoFormaPago(int formaPagoId, bool estado) => _datosFormaPago.CambiarEstado(formaPagoId, estado);

        private static string ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("Debe indicar el nombre de la forma de pago.");
            return nombre.Trim();
        }
    }
}
