using System;

namespace CapaEntidad
{
    public class Cliente
    {
        public int ClienteId { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public DateTime FechaRegistro { get; set; }

        /// <summary>Nombres y apellidos juntos, para mostrar en pantallas que no distinguen entre ambos (p.ej. Mesa de Cambio).</summary>
        public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
    }
}
