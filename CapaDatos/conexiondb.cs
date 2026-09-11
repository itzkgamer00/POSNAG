using System;
using System.Configuration;

namespace CapaDatos
{
    public static class conexiondb
    {
        /// <summary>
        /// Cadena de conexión a SQL Server. Se lee una sola vez desde el archivo de
        /// configuración de la aplicación (clave "cadena_conexion" en connectionStrings).
        /// </summary>
        public static string cadena { get; } = LeerCadena();

        private static string LeerCadena()
        {
            ConnectionStringSettings config = ConfigurationManager.ConnectionStrings["cadena_conexion"];

            if (config == null || string.IsNullOrWhiteSpace(config.ConnectionString))
            {
                throw new ConfigurationErrorsException(
                    "No se encontró la cadena de conexión 'cadena_conexion'. " +
                    "Revise el archivo App.config del ejecutable.");
            }

            return config.ConnectionString;
        }
    }
}
