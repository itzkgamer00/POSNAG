using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    /// <summary>
    /// Administracion de los tipos de operacion (Concepto) que aparecen en los combos de
    /// Ingreso y Egreso. La lectura para esos combos vive en CN_Transaccion; esta clase es
    /// para la pantalla de configuracion (alta, edicion, activar/desactivar).
    /// </summary>
    public class CN_Concepto
    {
        private static readonly string[] TiposValidos = { "INGRESO", "EGRESO" };

        /// <summary>
        /// Codigos de operacion que usa internamente Mesa de Cambio (Frmlcambiodivisas / CN_Transaccion.RegistrarCambioDivisa).
        /// No se pueden renombrar (codigo/tipo) ni desactivar desde la pantalla de Operaciones porque Mesa de Cambio
        /// dejaria de funcionar al no encontrarlos.
        /// </summary>
        private static readonly string[] OperacionesDeCambioDivisa = { "CAMBIO_DIVISA_RECIBIDO", "CAMBIO_DIVISA_ENTREGADO" };

        private readonly CD_Concepto _datosConcepto = new CD_Concepto();

        /// <summary>Todos los conceptos (activos e inactivos, ingreso y egreso), para la pantalla de administracion.</summary>
        public List<Concepto> Listar() => _datosConcepto.Listar();

        /// <summary>True si el codigo de operacion es uno de los que usa internamente Mesa de Cambio.</summary>
        public bool EsOperacionDeCambioDivisa(string operacion) =>
            Array.IndexOf(OperacionesDeCambioDivisa, operacion?.Trim().ToUpperInvariant()) >= 0;

        /// <summary>Registra un nuevo tipo de operacion.</summary>
        /// <exception cref="ArgumentException">Si falta el codigo o el nombre, o el tipo no es valido.</exception>
        /// <exception cref="InvalidOperationException">Si ya existe un concepto con ese codigo de operacion.</exception>
        public Concepto RegistrarConcepto(string operacion, string nombre, string tipo)
        {
            operacion = ValidarOperacion(operacion);
            nombre = ValidarNombre(nombre);
            tipo = ValidarTipo(tipo);

            if (_datosConcepto.ExisteOperacion(operacion))
                throw new InvalidOperationException("Ya existe un tipo de operacion con ese codigo.");

            var concepto = new Concepto { Operacion = operacion, Nombre = nombre, Tipo = tipo, Estado = true };
            concepto.ConceptoId = _datosConcepto.Registrar(operacion, nombre, tipo);
            return concepto;
        }

        /// <summary>Actualiza codigo, nombre y tipo de un concepto existente.</summary>
        /// <exception cref="ArgumentException">Si falta el codigo o el nombre, o el tipo no es valido.</exception>
        /// <exception cref="InvalidOperationException">
        /// Si ya existe otro concepto con ese codigo de operacion, o si se intenta cambiar el codigo o el tipo
        /// de un concepto que usa internamente Mesa de Cambio.
        /// </exception>
        public void ActualizarConcepto(int conceptoId, string operacion, string nombre, string tipo)
        {
            operacion = ValidarOperacion(operacion);
            nombre = ValidarNombre(nombre);
            tipo = ValidarTipo(tipo);

            Concepto actual = _datosConcepto.ObtenerPorId(conceptoId);
            if (actual != null && EsOperacionDeCambioDivisa(actual.Operacion) &&
                (operacion != actual.Operacion || tipo != actual.Tipo))
            {
                throw new InvalidOperationException(
                    $"\"{actual.Nombre}\" lo usa Mesa de Cambio internamente: no se puede cambiar su codigo ni su tipo. Solo puede renombrarlo.");
            }

            if (_datosConcepto.ExisteOperacion(operacion, conceptoId))
                throw new InvalidOperationException("Ya existe otro tipo de operacion con ese codigo.");

            _datosConcepto.Actualizar(conceptoId, operacion, nombre, tipo);
        }

        /// <summary>Activa o desactiva un tipo de operacion.</summary>
        /// <exception cref="InvalidOperationException">
        /// Si se intenta desactivar un concepto que usa internamente Mesa de Cambio.
        /// </exception>
        public void CambiarEstadoConcepto(int conceptoId, bool estado)
        {
            if (!estado)
            {
                Concepto actual = _datosConcepto.ObtenerPorId(conceptoId);
                if (actual != null && EsOperacionDeCambioDivisa(actual.Operacion))
                    throw new InvalidOperationException(
                        $"\"{actual.Nombre}\" lo usa Mesa de Cambio internamente y no se puede desactivar.");
            }

            _datosConcepto.CambiarEstado(conceptoId, estado);
        }

        /// <summary>Elimina definitivamente un tipo de operacion.</summary>
        /// <exception cref="InvalidOperationException">
        /// Si el concepto no existe, si lo usa internamente Mesa de Cambio, o si ya tiene movimientos
        /// (ingresos/egresos) registrados con el.
        /// </exception>
        public void EliminarConcepto(int conceptoId)
        {
            Concepto actual = _datosConcepto.ObtenerPorId(conceptoId);
            if (actual == null)
                throw new InvalidOperationException("El tipo de operacion ya no existe.");

            if (EsOperacionDeCambioDivisa(actual.Operacion))
                throw new InvalidOperationException($"\"{actual.Nombre}\" lo usa Mesa de Cambio internamente y no se puede eliminar.");

            try
            {
                _datosConcepto.Eliminar(conceptoId);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new InvalidOperationException(
                    $"No se puede eliminar \"{actual.Nombre}\": ya tiene movimientos (ingresos/egresos) registrados. Desactivelo en su lugar.", ex);
            }
        }

        private static string ValidarOperacion(string operacion)
        {
            if (string.IsNullOrWhiteSpace(operacion))
                throw new ArgumentException("Debe indicar el codigo de la operacion (ej. DOTACION, DEPOSITO_BAC).");
            return operacion.Trim().ToUpperInvariant();
        }

        private static string ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("Debe indicar el nombre del tipo de operacion.");
            return nombre.Trim();
        }

        private static string ValidarTipo(string tipo)
        {
            tipo = tipo?.Trim().ToUpperInvariant();

            if (Array.IndexOf(TiposValidos, tipo) < 0)
                throw new ArgumentException("El tipo debe ser Ingreso o Egreso.");

            return tipo;
        }
    }
}
