using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Concepto
    {
        /// <summary>Conceptos activos de un tipo ("INGRESO" o "EGRESO").</summary>
        public List<Concepto> ListarPorTipo(string tipo)
        {
            const string sql =
                "SELECT concepto_id, operacion, nombre, tipo, estado " +
                "FROM Concepto WHERE estado = 1 AND tipo = @tipo ORDER BY nombre";

            var lista = new List<Concepto>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = tipo;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Concepto
                        {
                            ConceptoId = Convert.ToInt32(dr["concepto_id"]),
                            Operacion = dr["operacion"] as string,
                            Nombre = dr["nombre"] as string,
                            Tipo = dr["tipo"] as string,
                            Estado = Convert.ToBoolean(dr["estado"])
                        });
                    }
                }
            }

            return lista;
        }
    }
}
