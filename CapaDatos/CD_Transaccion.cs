using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Transaccion
    {
        /// <summary>Movimientos (ingresos/egresos) de una apertura, del mas reciente al mas antiguo.</summary>
        public List<Transaccion> ListarPorApertura(int aperturaId)
        {
            const string sql =
                "SELECT t.transaccion_id, t.apertura_id, t.caja_id, t.usuario_id, t.concepto_id, t.moneda_id, " +
                "       t.forma_pago_id, t.tipo, t.monto, t.descripcion, t.fecha_hora, t.estado, " +
                "       c.nombre AS ConceptoNombre, m.nombre AS MonedaNombre, m.codigo AS MonedaCodigo, " +
                "       fp.nombre AS FormaPagoNombre " +
                "FROM Transacciones t " +
                "JOIN Concepto c ON c.concepto_id = t.concepto_id " +
                "JOIN Moneda m ON m.moneda_id = t.moneda_id " +
                "LEFT JOIN FormaPago fp ON fp.forma_pago_id = t.forma_pago_id " +
                "WHERE t.apertura_id = @aperturaId " +
                "ORDER BY t.fecha_hora DESC";

            var lista = new List<Transaccion>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@aperturaId", SqlDbType.Int).Value = aperturaId;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Transaccion
                        {
                            TransaccionId = Convert.ToInt32(dr["transaccion_id"]),
                            AperturaId = Convert.ToInt32(dr["apertura_id"]),
                            CajaId = Convert.ToInt32(dr["caja_id"]),
                            UsuarioId = Convert.ToInt32(dr["usuario_id"]),
                            ConceptoId = Convert.ToInt32(dr["concepto_id"]),
                            MonedaId = Convert.ToInt32(dr["moneda_id"]),
                            FormaPagoId = dr["forma_pago_id"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["forma_pago_id"]),
                            Tipo = dr["tipo"] as string,
                            Monto = Convert.ToDecimal(dr["monto"]),
                            Descripcion = dr["descripcion"] as string,
                            FechaHora = Convert.ToDateTime(dr["fecha_hora"]),
                            Estado = Convert.ToBoolean(dr["estado"]),
                            ConceptoNombre = dr["ConceptoNombre"] as string,
                            MonedaNombre = dr["MonedaNombre"] as string,
                            MonedaCodigo = dr["MonedaCodigo"] as string,
                            FormaPagoNombre = dr["FormaPagoNombre"] as string
                        });
                    }
                }
            }

            return lista;
        }
        /// <summary>Inserta un ingreso o egreso de efectivo. Devuelve el transaccion_id generado.</summary>
        public int Registrar(Transaccion transaccion)
        {
            const string sql =
                "INSERT INTO Transacciones (apertura_id, caja_id, usuario_id, concepto_id, moneda_id, forma_pago_id, tipo, monto, descripcion) " +
                "OUTPUT INSERTED.transaccion_id " +
                "VALUES (@aperturaId, @cajaId, @usuarioId, @conceptoId, @monedaId, @formaPagoId, @tipo, @monto, @descripcion)";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@aperturaId", SqlDbType.Int).Value = transaccion.AperturaId;
                cmd.Parameters.Add("@cajaId", SqlDbType.Int).Value = transaccion.CajaId;
                cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = transaccion.UsuarioId;
                cmd.Parameters.Add("@conceptoId", SqlDbType.Int).Value = transaccion.ConceptoId;
                cmd.Parameters.Add("@monedaId", SqlDbType.Int).Value = transaccion.MonedaId;
                cmd.Parameters.Add("@formaPagoId", SqlDbType.Int).Value = (object)transaccion.FormaPagoId ?? DBNull.Value;
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = transaccion.Tipo;

                cmd.Parameters.Add("@monto", SqlDbType.Decimal).Value = transaccion.Monto;
                cmd.Parameters["@monto"].Precision = 18;
                cmd.Parameters["@monto"].Scale = 2;

                cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar, 255).Value =
                    (object)transaccion.Descripcion ?? DBNull.Value;

                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
