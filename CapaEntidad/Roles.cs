using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Roles
    {
        public int IdRol { get; set; }
        public string Descripcion { get; set; }
        public bool estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
