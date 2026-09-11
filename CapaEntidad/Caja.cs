using System;

namespace CapaEntidad
{
    public class Caja
    {
        public int CajaId { get; set; }
        public string Nombre { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
