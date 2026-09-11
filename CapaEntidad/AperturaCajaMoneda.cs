namespace CapaEntidad
{
    /// <summary>Monto inicial de una apertura de caja para una moneda especifica.</summary>
    public class AperturaCajaMoneda
    {
        public int AperturaMonedaId { get; set; }
        public int AperturaId { get; set; }
        public int MonedaId { get; set; }
        public decimal MontoInicial { get; set; }

        /// <summary>Poblado mediante JOIN con Moneda, para mostrar en la UI sin otra consulta.</summary>
        public string MonedaNombre { get; set; }
        public string MonedaCodigo { get; set; }
        public string MonedaSimbolo { get; set; }
    }
}
