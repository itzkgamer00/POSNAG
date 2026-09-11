namespace CapaEntidad
{
    /// <summary>Conteo final de un cierre de caja para una moneda especifica.</summary>
    public class CierreCajaMoneda
    {
        public int CierreMonedaId { get; set; }
        public int CierreId { get; set; }
        public int MonedaId { get; set; }

        /// <summary>Monto esperado segun el sistema (apertura + ingresos - egresos).</summary>
        public decimal MontoSistema { get; set; }

        /// <summary>Monto contado fisicamente al cerrar.</summary>
        public decimal MontoFinal { get; set; }

        /// <summary>Diferencia = monto_final - monto_sistema (columna calculada en la base de datos).</summary>
        public decimal Diferencia => MontoFinal - MontoSistema;

        /// <summary>Poblado mediante JOIN con Moneda, para mostrar en la UI sin otra consulta.</summary>
        public string MonedaNombre { get; set; }
        public string MonedaCodigo { get; set; }
        public string MonedaSimbolo { get; set; }
    }
}
