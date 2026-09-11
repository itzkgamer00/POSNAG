using System;

namespace CapaEntidad
{
    /// <summary>
    /// Una fila del historial de cierres de caja: una sesion (apertura+cierre) para una
    /// moneda especifica. Una sesion multi-moneda genera varias filas, una por moneda.
    /// </summary>
    public class HistorialCierre
    {
        public int AperturaId { get; set; }
        public int CierreId { get; set; }
        public string CajaNombre { get; set; }
        public string UsuarioAperturaNombre { get; set; }
        public string UsuarioCierreNombre { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime FechaCierre { get; set; }
        public string MonedaNombre { get; set; }
        public string MonedaCodigo { get; set; }
        public string MonedaSimbolo { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal MontoSistema { get; set; }
        public decimal MontoFinal { get; set; }
        public decimal Diferencia { get; set; }
        public string ObservacionesApertura { get; set; }
        public string ObservacionesCierre { get; set; }
    }
}
