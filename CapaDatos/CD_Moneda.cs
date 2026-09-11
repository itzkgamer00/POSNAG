using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Moneda
    {
        public List<Moneda> ListarActivas()
        {
            const string sql =
                "SELECT moneda_id, nombre, codigo, simbolo, estado " +
                "FROM Moneda WHERE estado = 1 ORDER BY moneda_id";

            var lista = new List<Moneda>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Moneda
                        {
                            MonedaId = Convert.ToInt32(dr["moneda_id"]),
                            Nombre = dr["nombre"] as string,
                            Codigo = dr["codigo"] as string,
                            Simbolo = dr["simbolo"] as string,
                            Estado = Convert.ToBoolean(dr["estado"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
