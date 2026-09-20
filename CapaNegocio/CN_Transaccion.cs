using System;
using System.Collections.Generic;
using System.Linq;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Transaccion
    {
        private const string ConceptoCambioRecibido = "CAMBIO_DIVISA_RECIBIDO";
        private const string ConceptoCambioEntregado = "CAMBIO_DIVISA_ENTREGADO";

        private readonly CD_Transaccion _datosTransaccion = new CD_Transaccion();
        private readonly CD_Concepto _datosConcepto = new CD_Concepto();
        private readonly CD_FormaPago _datosFormaPago = new CD_FormaPago();
        private readonly CD_Moneda _datosMoneda = new CD_Moneda();

        public List<Concepto> ListarConceptosIngreso() => _datosConcepto.ListarPorTipo("INGRESO");

        public List<Concepto> ListarConceptosEgreso() => _datosConcepto.ListarPorTipo("EGRESO");

        /// <summary>Todos los conceptos activos (ingreso y egreso), para el filtro del reporte de movimientos.</summary>
        public List<Concepto> ListarConceptosActivos() =>
            _datosConcepto.ListarPorTipo("INGRESO")
                .Concat(_datosConcepto.ListarPorTipo("EGRESO"))
                .OrderBy(c => c.Nombre)
                .ToList();

        public List<FormaPago> ListarFormasPagoActivas() => _datosFormaPago.ListarActivas();

        public List<Moneda> ListarMonedasActivas() => _datosMoneda.ListarActivas();

        /// <summary>Movimientos de una apertura, del mas reciente al mas antiguo.</summary>
        public List<Transaccion> ListarPorApertura(int aperturaId) => _datosTransaccion.ListarPorApertura(aperturaId);

        /// <summary>Solo los movimientos de Mesa de Cambio de una apertura, del mas reciente al mas antiguo.</summary>
        public List<Transaccion> ListarCambiosDivisaPorApertura(int aperturaId) => _datosTransaccion.ListarCambiosDivisaPorApertura(aperturaId);

        /// <summary>
        /// Movimientos filtrados por rango de fecha (inclusive en ambos extremos) y, opcionalmente,
        /// caja/usuario/concepto. Para el reporte de Movimientos por Concepto.
        /// </summary>
        public List<Transaccion> ListarParaReporte(DateTime desde, DateTime hasta, int? cajaId = null, int? usuarioId = null, int? conceptoId = null, int? monedaId = null)
        {
            if (hasta.Date < desde.Date)
                throw new ArgumentException("La fecha 'hasta' no puede ser anterior a la fecha 'desde'.");

            return _datosTransaccion.ListarParaReporte(desde.Date, hasta.Date.AddDays(1), cajaId, usuarioId, conceptoId, monedaId);
        }

        /// <summary>
        /// Solo movimientos de Mesa de Cambio, filtrados por rango de fecha (inclusive en ambos extremos)
        /// y, opcionalmente, caja/usuario. Para el reporte de Mesa de Cambio.
        /// </summary>
        public List<Transaccion> ListarCambiosDivisaParaReporte(DateTime desde, DateTime hasta, int? cajaId = null, int? usuarioId = null)
        {
            if (hasta.Date < desde.Date)
                throw new ArgumentException("La fecha 'hasta' no puede ser anterior a la fecha 'desde'.");

            return _datosTransaccion.ListarCambiosDivisaParaReporte(desde.Date, hasta.Date.AddDays(1), cajaId, usuarioId);
        }

        public Transaccion RegistrarIngreso(AperturaCaja apertura, int usuarioId, int conceptoId, int monedaId,
            int? formaPagoId, decimal monto, string descripcion)
            => Registrar(apertura, usuarioId, conceptoId, monedaId, formaPagoId, monto, descripcion, "INGRESO");

        public Transaccion RegistrarEgreso(AperturaCaja apertura, int usuarioId, int conceptoId, int monedaId,
            int? formaPagoId, decimal monto, string descripcion)
            => Registrar(apertura, usuarioId, conceptoId, monedaId, formaPagoId, monto, descripcion, "EGRESO");

        private Transaccion Registrar(AperturaCaja apertura, int usuarioId, int conceptoId, int monedaId,
            int? formaPagoId, decimal monto, string descripcion, string tipo)
        {
            if (apertura == null)
                throw new InvalidOperationException("Debe abrir la caja antes de registrar movimientos.");

            if (monto <= 0)
                throw new ArgumentException("El monto debe ser mayor que cero.");

            var transaccion = new Transaccion
            {
                AperturaId = apertura.AperturaId,
                CajaId = apertura.CajaId,
                UsuarioId = usuarioId,
                ConceptoId = conceptoId,
                MonedaId = monedaId,
                FormaPagoId = formaPagoId,
                Tipo = tipo,
                Monto = monto,
                Descripcion = descripcion
            };

            transaccion.TransaccionId = _datosTransaccion.Registrar(transaccion);
            return transaccion;
        }

        /// <summary>
        /// Registra una operacion de Mesa de Cambio como un par de movimientos en Transacciones:
        /// un INGRESO por la moneda que el cliente entrega y un EGRESO por la moneda que la caja
        /// entrega al cliente.
        /// </summary>
        public void RegistrarCambioDivisa(AperturaCaja apertura, int usuarioId,
            int monedaRecibidaId, decimal montoRecibido, int monedaEntregadaId, decimal montoEntregado,
            int? formaPagoId, string descripcion)
        {
            if (apertura == null)
                throw new InvalidOperationException("Debe abrir la caja antes de registrar una operacion de cambio.");

            if (montoRecibido <= 0 || montoEntregado <= 0)
                throw new ArgumentException("Los montos recibido y entregado deben ser mayores que cero.");

            if (monedaRecibidaId == monedaEntregadaId)
                throw new ArgumentException("La moneda recibida y la moneda entregada deben ser distintas.");

            Concepto conceptoRecibido = _datosConcepto.ObtenerPorOperacion(ConceptoCambioRecibido);
            Concepto conceptoEntregado = _datosConcepto.ObtenerPorOperacion(ConceptoCambioEntregado);

            if (conceptoRecibido == null || conceptoEntregado == null)
                throw new InvalidOperationException(
                    "Faltan los conceptos de Mesa de Cambio en la base de datos. Ejecute sql/06_seed_cambio_divisas.sql.");

            Registrar(apertura, usuarioId, conceptoRecibido.ConceptoId, monedaRecibidaId,
                formaPagoId, montoRecibido, descripcion, "INGRESO");

            Registrar(apertura, usuarioId, conceptoEntregado.ConceptoId, monedaEntregadaId,
                formaPagoId, montoEntregado, descripcion, "EGRESO");
        }

        /// <summary>Anula un movimiento (ingreso, egreso o Mesa de Cambio) registrado por error. Queda en el historial marcado como Inactivo.</summary>
        /// <exception cref="InvalidOperationException">Si la transaccion no existe o ya estaba anulada.</exception>
        public void AnularTransaccion(int transaccionId)
        {
            if (!_datosTransaccion.Anular(transaccionId))
                throw new InvalidOperationException("La transaccion no existe o ya estaba anulada.");
        }
    }
}
