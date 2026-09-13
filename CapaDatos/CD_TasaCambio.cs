using System;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_TasaCambio
    {
        /// <summary>Tasa vigente para una moneda y tipo de operacion ("COMPRA" o "VENTA"), o null si no hay ninguna configurada.</summary>
        public TasaCambio ObtenerVigente(int monedaId, string tipoOperacion)
        {
            const string sql =
                "SELECT tasa_id, moneda_id, tipo_operacion, valor, estado " +
                "FROM TasaCambio WHERE estado = 1 AND moneda_id = @monedaId AND tipo_operacion = @tipo";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@monedaId", SqlDbType.Int).Value = monedaId;
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = tipoOperacion;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read()) return null;

                    return new TasaCambio
                    {
                        TasaId = Convert.ToInt32(dr["tasa_id"]),
                        MonedaId = Convert.ToInt32(dr["moneda_id"]),
                        TipoOperacion = dr["tipo_operacion"] as string,
                        Valor = Convert.ToDecimal(dr["valor"]),
                        Estado = Convert.ToBoolean(dr["estado"])
                    };
                }
            }
        }
    }
}
