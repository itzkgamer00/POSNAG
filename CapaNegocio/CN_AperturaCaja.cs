using System;
using System.Collections.Generic;
using System.Linq;
using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class CN_AperturaCaja
    {
        private readonly CD_AperturaCaja _datosApertura = new CD_AperturaCaja();
        private readonly CD_Caja _datosCaja = new CD_Caja();
        private readonly CD_Moneda _datosMoneda = new CD_Moneda();

        public List<Caja> ListarCajasActivas() => _datosCaja.ListarActivas();

        public List<Moneda> ListarMonedasActivas() => _datosMoneda.ListarActivas();

        public AperturaCaja ObtenerAperturaAbierta(int cajaId) => _datosApertura.ObtenerAperturaAbierta(cajaId);

        /// <summary>La apertura abierta mas reciente, sin importar la caja (para restaurar el estado de la UI al iniciar).</summary>
        public AperturaCaja ObtenerUltimaAperturaAbierta() => _datosApertura.ObtenerUltimaAperturaAbierta();

        /// <summary>
        /// Abre una caja con un monto inicial por cada moneda indicada.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si la caja ya tiene una apertura abierta.</exception>
        /// <exception cref="ArgumentException">Si faltan montos o alguno es negativo.</exception>
        public AperturaCaja Abrir(int cajaId, int usuarioId, string observaciones, List<AperturaCajaMoneda> montos)
        {
            if (montos == null || montos.Count == 0)
                throw new ArgumentException("Debe indicar el monto inicial de al menos una moneda.");

            if (montos.Any(m => m.MontoInicial < 0))
                throw new ArgumentException("Los montos iniciales no pueden ser negativos.");

            if (_datosApertura.ObtenerAperturaAbierta(cajaId) != null)
                throw new InvalidOperationException("Esta caja ya tiene una apertura abierta.");

            var apertura = new AperturaCaja
            {
                CajaId = cajaId,
                UsuarioId = usuarioId,
                Estado = "ABIERTA",
                Observaciones = observaciones,
                Montos = montos
            };

            _datosApertura.Abrir(apertura);

            // Se relee desde la base de datos en lugar de devolver el objeto recien
            // armado: asi quedan completos FechaHora, CajaNombre y los nombres de
            // moneda de cada detalle (poblados por JOIN), que el llamador no tiene.
            return _datosApertura.ObtenerAperturaAbierta(cajaId);
        }

        /// <summary>Monto esperado en sistema por moneda para la apertura indicada.</summary>
        public Dictionary<int, decimal> CalcularMontoSistema(int aperturaId) => _datosApertura.CalcularMontoSistema(aperturaId);

        /// <summary>Historial de sesiones de caja cerradas, una fila por (cierre, moneda).</summary>
        public List<HistorialCierre> ListarHistorialCierres() => _datosApertura.ListarHistorialCierres();

        /// <summary>
        /// Cierra una apertura con el monto contado (fisico) de cada moneda.
        /// El monto esperado en sistema se calcula internamente, no se recibe del llamador.
        /// </summary>
        /// <param name="montosFinales">Clave = moneda_id, valor = monto contado.</param>
        public CierreCaja Cerrar(int aperturaId, int usuarioId, string observaciones, Dictionary<int, decimal> montosFinales)
        {
            if (montosFinales == null || montosFinales.Count == 0)
                throw new ArgumentException("Debe indicar el monto contado de al menos una moneda.");

            if (montosFinales.Values.Any(v => v < 0))
                throw new ArgumentException("Los montos contados no pueden ser negativos.");

            Dictionary<int, decimal> montoSistema = _datosApertura.CalcularMontoSistema(aperturaId);

            var detalle = montosFinales.Select(kv => new CierreCajaMoneda
            {
                MonedaId = kv.Key,
                MontoSistema = montoSistema.TryGetValue(kv.Key, out decimal sistema) ? sistema : 0m,
                MontoFinal = kv.Value
            }).ToList();

            var cierre = new CierreCaja
            {
                AperturaId = aperturaId,
                UsuarioId = usuarioId,
                Observaciones = observaciones,
                Montos = detalle
            };

            cierre.CierreId = _datosApertura.Cerrar(cierre);
            return cierre;
        }
    }
}
