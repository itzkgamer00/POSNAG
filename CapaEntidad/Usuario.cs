using System;

namespace CapaEntidad
{
    public class Usuario
    {
        public int usuario_id { get; set; }
        public string NombreCompleto { get; set; }
        public string usuario { get; set; }

        /// <summary>
        /// Hash de la contraseña en formato "PBKDF2.SHA256.{iteraciones}.{saltBase64}.{hashBase64}".
        /// Nunca contiene la contraseña en texto plano.
        /// </summary>
        public string password_hash { get; set; }

        /// <summary>Identificador del rol (FK a la tabla Roles).</summary>
        public int IdRol { get; set; }

        /// <summary>Descripción del rol, poblada mediante JOIN. Puede ser null.</summary>
        public string RolDescripcion { get; set; }

        public bool estado { get; set; }
        public DateTime fechacreacion { get; set; }
    }
}
