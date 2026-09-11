using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_FormaPago
    {
        public List<FormaPago> ListarActivas()
        {
            const string sql =
                "SELECT forma_pago_id, nombre, estado FROM FormaPago WHERE estado = 1 ORDER BY nombre";

            var lista = new List<FormaPago>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new FormaPago
                        {
                            FormaPagoId = Convert.ToInt32(dr["forma_pago_id"]),
                            Nombre = dr["nombre"] as string,
                            Estado = Convert.ToBoolean(dr["estado"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
