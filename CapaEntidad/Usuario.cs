using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Usuario
    {
        public int usuario_id { get; set; }
        public string NombreCompleto { get; set; }
        public string usuario { get; set; }
        public string password_hash { get; set; }
        public int IdRol { get; set; }
        public bool estado { get; set; }
        public DateTime fechacreacion { get; set; }

    }
}
