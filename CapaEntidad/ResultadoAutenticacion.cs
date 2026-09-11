namespace CapaEntidad
{
    /// <summary>Motivo del resultado de un intento de inicio de sesión.</summary>
    public enum MotivoAutenticacion
    {
        Exito = 0,
        CredencialesInvalidas = 1,
        UsuarioInactivo = 2,
        DatosIncompletos = 3,
        ErrorInterno = 4
    }

    /// <summary>Resultado de un intento de autenticación devuelto por la capa de negocio.</summary>
    public class ResultadoAutenticacion
    {
        public bool Exitoso { get; set; }
        public MotivoAutenticacion Motivo { get; set; }
        public string Mensaje { get; set; }

        /// <summary>Usuario autenticado. Solo tiene valor cuando <see cref="Exitoso"/> es true.</summary>
        public Usuario Usuario { get; set; }

        public static ResultadoAutenticacion Ok(Usuario usuario)
        {
            return new ResultadoAutenticacion
            {
                Exitoso = true,
                Motivo = MotivoAutenticacion.Exito,
                Mensaje = "Autenticación correcta.",
                Usuario = usuario
            };
        }

        public static ResultadoAutenticacion Fallo(MotivoAutenticacion motivo, string mensaje)
        {
            return new ResultadoAutenticacion
            {
                Exitoso = false,
                Motivo = motivo,
                Mensaje = mensaje,
                Usuario = null
            };
        }
    }
}
