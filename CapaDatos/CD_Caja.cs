using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Caja
    {
        public List<Caja> ListarActivas()
        {
            const string sql =
                "SELECT caja_id, nombre, estado, FechaCreacion " +
                "FROM Caja WHERE estado = 1 ORDER BY nombre";

            var lista = new List<Caja>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Caja
                        {
                            CajaId = Convert.ToInt32(dr["caja_id"]),
                            Nombre = dr["nombre"] as string,
                            Estado = Convert.ToBoolean(dr["estado"]),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
