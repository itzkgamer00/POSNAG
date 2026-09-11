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
        public string MonedaNombre { get; set; }
        public string MonedaCodigo { get; set; }
        public string FormaPagoNombre { get; set; }
    }
}
