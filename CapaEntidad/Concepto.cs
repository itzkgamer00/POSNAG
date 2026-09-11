namespace CapaEntidad
{
    /// <summary>Motivo de un ingreso o egreso de efectivo (p.ej. "Dotacion", "Deposito BANPRO").</summary>
    public class Concepto
    {
        public int ConceptoId { get; set; }
        public string Operacion { get; set; }
        public string Nombre { get; set; }

        /// <summary>"INGRESO" o "EGRESO".</summary>
        public string Tipo { get; set; }

        public bool Estado { get; set; }
    }
}
