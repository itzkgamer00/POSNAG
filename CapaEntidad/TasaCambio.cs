namespace CapaEntidad
{
    /// <summary>Tasa vigente para comprar o vender una divisa extranjera (valor en NIO por unidad).</summary>
    public class TasaCambio
    {
        public int TasaId { get; set; }
        public int MonedaId { get; set; }

        /// <summary>"COMPRA" o "VENTA".</summary>
        public string TipoOperacion { get; set; }

        public decimal Valor { get; set; }
        public bool Estado { get; set; }
    }
}
