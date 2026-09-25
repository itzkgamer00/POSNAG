using System;

namespace CapaEntidad
{
    /// <summary>Ingreso o egreso de efectivo registrado durante una apertura de caja.</summary>
    public class Transaccion
    {
        public int TransaccionId { get; set; }
        public int AperturaId { get; set; }
        public int CajaId { get; set; }
        public int UsuarioId { get; set; }
        public int ConceptoId { get; set; }
        public int MonedaId { get; set; }

        /// <summary>Opcional: puede no aplicar segun el concepto.</summary>
        public int? FormaPagoId { get; set; }

        /// <summary>"INGRESO" o "EGRESO".</summary>
        public string Tipo { get; set; }

        public decimal Monto { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaHora { get; set; }
        public bool Estado { get; set; }

        /// <summary>Poblados mediante JOIN, para mostrar en la UI sin otra consulta.</summary>
        public string ConceptoNombre { get; set; }

        /// <summary>Codigo unico del concepto (p.ej. "CAMBIO_DIVISA_RECIBIDO"), util para filtrar Mesa de Cambio en reportes.</summary>
        public string ConceptoOperacion { get; set; }
        public string MonedaNombre { get; set; }
        public string MonedaCodigo { get; set; }
        public string MonedaSimbolo { get; set; }
        public string FormaPagoNombre { get; set; }
        public string CajaNombre { get; set; }
        public string UsuarioNombre { get; set; }

        /// <summary>Administrador que autorizo la anulacion y cuando. Null si la transaccion no esta anulada.</summary>
        public string AnuladoPorNombre { get; set; }
        public DateTime? FechaAnulacion { get; set; }
    }
}
