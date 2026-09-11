using System;
using System.Collections.Generic;

namespace CapaEntidad
{
    public class AperturaCaja
    {
        public int AperturaId { get; set; }
        public int CajaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaHora { get; set; }

        /// <summary>"ABIERTA" o "CERRADA".</summary>
        public string Estado { get; set; }

        public string Observaciones { get; set; }

        /// <summary>Poblado mediante JOIN, para mostrar en la UI sin otra consulta.</summary>
        public string CajaNombre { get; set; }

        /// <summary>Monto inicial cargado para cada moneda activa.</summary>
        public List<AperturaCajaMoneda> Montos { get; set; } = new List<AperturaCajaMoneda>();
    }
}
