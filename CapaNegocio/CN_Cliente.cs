using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Cliente
    {
        private readonly CD_Cliente _datosCliente = new CD_Cliente();

        /// <summary>Todos los clientes registrados, para la pestaña Clientes.</summary>
        public List<Cliente> Listar() => _datosCliente.Listar();

        /// <summary>Busca un cliente por tipo y numero de identificacion. Null si no existe.</summary>
        public Cliente BuscarPorIdentificacion(string tipoIdentificacion, string numeroIdentificacion)
        {
            if (string.IsNullOrWhiteSpace(numeroIdentificacion))
                throw new ArgumentException("Debe indicar el numero de identificacion.");

            return _datosCliente.BuscarPorIdentificacion((tipoIdentificacion ?? string.Empty).Trim(), numeroIdentificacion.Trim());
        }

        /// <summary>Registra un nuevo cliente.</summary>
        /// <exception cref="ArgumentException">Si falta algun dato obligatorio.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe un cliente con esa identificacion.</exception>
        public Cliente RegistrarCliente(string tipoIdentificacion, string numeroIdentificacion, string nombres, string apellidos,
            string telefono, string direccion, string correo)
        {
            if (string.IsNullOrWhiteSpace(tipoIdentificacion))
                throw new ArgumentException("Debe seleccionar el tipo de identificacion.");
            if (string.IsNullOrWhiteSpace(numeroIdentificacion))
                throw new ArgumentException("Debe indicar el numero de identificacion.");
            if (string.IsNullOrWhiteSpace(nombres))
                throw new ArgumentException("Debe indicar el nombre del cliente.");

            tipoIdentificacion = tipoIdentificacion.Trim();
            numeroIdentificacion = numeroIdentificacion.Trim();

            if (_datosCliente.BuscarPorIdentificacion(tipoIdentificacion, numeroIdentificacion) != null)
                throw new InvalidOperationException("Ya existe un cliente registrado con esa identificacion.");

            var cliente = new Cliente
            {
                TipoIdentificacion = tipoIdentificacion,
                NumeroIdentificacion = numeroIdentificacion,
                Nombres = nombres.Trim(),
                Apellidos = string.IsNullOrWhiteSpace(apellidos) ? null : apellidos.Trim(),
                Telefono = string.IsNullOrWhiteSpace(telefono) ? null : telefono.Trim(),
                Direccion = string.IsNullOrWhiteSpace(direccion) ? null : direccion.Trim(),
                Correo = string.IsNullOrWhiteSpace(correo) ? null : correo.Trim()
            };
            cliente.ClienteId = _datosCliente.Registrar(cliente);
            return cliente;
        }

        /// <summary>Elimina definitivamente un cliente.</summary>
        /// <exception cref="InvalidOperationException">Si el cliente tiene movimientos u otros registros asociados.</exception>
        public void EliminarCliente(int clienteId)
        {
            try
            {
                _datosCliente.Eliminar(clienteId);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar el cliente porque tiene operaciones registradas asociadas.");
            }
        }
    }
}
