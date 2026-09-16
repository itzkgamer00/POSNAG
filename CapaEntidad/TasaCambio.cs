using System;

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
        public DateTime FechaHora { get; set; }
        public bool Estado { get; set; }

        /// <summary>Nombre de la moneda, poblada mediante JOIN. Puede ser null.</summary>
        public string MonedaNombre { get; set; }

        /// <summary>Codigo de la moneda, poblada mediante JOIN. Puede ser null.</summary>
        public string MonedaCodigo { get; set; }
    }
}
