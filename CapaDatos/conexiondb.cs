using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using CapaEntidad;
using System.Configuration;

namespace CapaDatos
{
    public class conexiondb
    {
        public static string cadena = ConfigurationManager.ConnectionStrings["cadena_conexion"].ToString();

    }
}
