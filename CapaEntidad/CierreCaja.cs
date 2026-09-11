using System;
using System.Collections.Generic;

namespace CapaEntidad
{
    public class CierreCaja
    {
        public int CierreId { get; set; }
        public int AperturaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Observaciones { get; set; }

        /// <summary>Conteo final cargado para cada moneda de la apertura.</summary>
        public List<CierreCajaMoneda> Montos { get; set; } = new List<CierreCajaMoneda>();
    }
}
