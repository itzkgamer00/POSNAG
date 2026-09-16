using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_TasaCambio
    {
        private static readonly string[] TiposValidos = { "COMPRA", "VENTA" };

        private readonly CD_TasaCambio _datosTasaCambio = new CD_TasaCambio();

        /// <summary>Tasa vigente para una moneda y tipo de operacion, o null si no hay ninguna configurada.</summary>
        public TasaCambio ObtenerVigente(int monedaId, string tipoOperacion) =>
            _datosTasaCambio.ObtenerVigente(monedaId, tipoOperacion);

        /// <summary>Todas las tasas registradas (vigentes e historicas), para la pantalla de administracion.</summary>
        public List<TasaCambio> Listar() => _datosTasaCambio.Listar();

        /// <summary>
        /// Registra una nueva tasa vigente para una moneda y tipo de operacion. La tasa vigente anterior
        /// (si existe) queda desactivada automaticamente, preservando el historial.
        /// </summary>
        /// <exception cref="ArgumentException">Si el tipo de operacion no es valido o el valor no es mayor que cero.</exception>
        public TasaCambio RegistrarTasa(int monedaId, string tipoOperacion, decimal valor)
        {
            tipoOperacion = ValidarTipoOperacion(tipoOperacion);

            if (valor <= 0)
                throw new ArgumentException("El valor de la tasa debe ser mayor que cero.");

            int tasaId = _datosTasaCambio.Registrar(monedaId, tipoOperacion, valor);
            return new TasaCambio { TasaId = tasaId, MonedaId = monedaId, TipoOperacion = tipoOperacion, Valor = valor, Estado = true };
        }

        /// <summary>Activa o desactiva una tasa puntual.</summary>
        /// <exception cref="InvalidOperationException">
        /// Si se intenta activar una tasa cuando ya existe otra vigente para la misma moneda y tipo de operacion.
        /// </exception>
        public void CambiarEstadoTasa(int tasaId, bool estado)
        {
            try
            {
                _datosTasaCambio.CambiarEstado(tasaId, estado);
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                throw new InvalidOperationException(
                    "Ya existe otra tasa vigente para esa moneda y tipo de operacion. Desactivela primero o registre una tasa nueva.", ex);
            }
        }

        private static string ValidarTipoOperacion(string tipoOperacion)
        {
            tipoOperacion = tipoOperacion?.Trim().ToUpperInvariant();

            if (Array.IndexOf(TiposValidos, tipoOperacion) < 0)
                throw new ArgumentException("El tipo de operacion debe ser COMPRA o VENTA.");

            return tipoOperacion;
        }
    }
}
