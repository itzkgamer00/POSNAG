using System;
using System.Collections.Generic;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_Transaccion
    {
        private readonly CD_Transaccion _datosTransaccion = new CD_Transaccion();
        private readonly CD_Concepto _datosConcepto = new CD_Concepto();
        private readonly CD_FormaPago _datosFormaPago = new CD_FormaPago();
        private readonly CD_Moneda _datosMoneda = new CD_Moneda();

        public List<Concepto> ListarConceptosIngreso() => _datosConcepto.ListarPorTipo("INGRESO");

        public List<Concepto> ListarConceptosEgreso() => _datosConcepto.ListarPorTipo("EGRESO");

        public List<FormaPago> ListarFormasPagoActivas() => _datosFormaPago.ListarActivas();

        public List<Moneda> ListarMonedasActivas() => _datosMoneda.ListarActivas();

        /// <summary>Movimientos de una apertura, del mas reciente al mas antiguo.</summary>
        public List<Transaccion> ListarPorApertura(int aperturaId) => _datosTransaccion.ListarPorApertura(aperturaId);

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
    }
}
