using System;

namespace CapaEntidad
{
    public class Permiso
    {
        public int idPermiso { get; set; }
        public int IdRol { get; set; }
        public string codigo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
