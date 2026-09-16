using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_TasaCambio
    {
        private const string SelectBase =
            "SELECT t.tasa_id, t.moneda_id, t.tipo_operacion, t.valor, t.fecha_hora, t.estado, " +
            "       m.nombre AS MonedaNombre, m.codigo AS MonedaCodigo " +
            "FROM TasaCambio t " +
            "JOIN Moneda m ON m.moneda_id = t.moneda_id ";

        /// <summary>Tasa vigente para una moneda y tipo de operacion ("COMPRA" o "VENTA"), o null si no hay ninguna configurada.</summary>
        public TasaCambio ObtenerVigente(int monedaId, string tipoOperacion)
        {
            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(
                SelectBase + "WHERE t.estado = 1 AND t.moneda_id = @monedaId AND t.tipo_operacion = @tipo", cn))
            {
                cmd.Parameters.Add("@monedaId", SqlDbType.Int).Value = monedaId;
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = tipoOperacion;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    return dr.Read() ? Mapear(dr) : null;
                }
            }
        }

        /// <summary>Todas las tasas registradas (vigentes e historicas), para la pantalla de administracion.</summary>
        public List<TasaCambio> Listar()
        {
            var lista = new List<TasaCambio>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(SelectBase + "ORDER BY t.fecha_hora DESC", cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        lista.Add(Mapear(dr));
                }
            }

            return lista;
        }

        private static TasaCambio Mapear(SqlDataReader dr)
        {
            return new TasaCambio
            {
                TasaId = Convert.ToInt32(dr["tasa_id"]),
                MonedaId = Convert.ToInt32(dr["moneda_id"]),
                TipoOperacion = dr["tipo_operacion"] as string,
                Valor = Convert.ToDecimal(dr["valor"]),
                FechaHora = Convert.ToDateTime(dr["fecha_hora"]),
                Estado = Convert.ToBoolean(dr["estado"]),
                MonedaNombre = dr["MonedaNombre"] as string,
                MonedaCodigo = dr["MonedaCodigo"] as string
            };
        }

        /// <summary>
        /// Registra una nueva tasa vigente: desactiva la tasa vigente anterior (si existe) para la misma
        /// moneda y tipo de operacion, e inserta la nueva en una sola transaccion. Devuelve el tasa_id generado.
        /// </summary>
        public int Registrar(int monedaId, string tipoOperacion, decimal valor)
        {
            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        using (var cmdDesactivar = new SqlCommand(
                            "UPDATE TasaCambio SET estado = 0 WHERE moneda_id = @monedaId AND tipo_operacion = @tipo AND estado = 1",
                            cn, tx))
                        {
                            cmdDesactivar.Parameters.Add("@monedaId", SqlDbType.Int).Value = monedaId;
                            cmdDesactivar.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = tipoOperacion;
                            cmdDesactivar.ExecuteNonQuery();
                        }

                        int tasaId;
                        using (var cmdInsertar = new SqlCommand(
                            "INSERT INTO TasaCambio (moneda_id, tipo_operacion, valor) " +
                            "OUTPUT INSERTED.tasa_id VALUES (@monedaId, @tipo, @valor)",
                            cn, tx))
                        {
                            cmdInsertar.Parameters.Add("@monedaId", SqlDbType.Int).Value = monedaId;
                            cmdInsertar.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = tipoOperacion;
                            cmdInsertar.Parameters.Add("@valor", SqlDbType.Decimal).Value = valor;
                            tasaId = (int)cmdInsertar.ExecuteScalar();
                        }

                        tx.Commit();
                        return tasaId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Activa o desactiva una tasa puntual. Falla con SqlException (violacion de indice unico) si se
        /// intenta activar una tasa cuando ya existe otra vigente para la misma moneda y tipo de operacion.
        /// </summary>
        public void CambiarEstado(int tasaId, bool estado)
        {
            const string sql = "UPDATE TasaCambio SET estado = @estado WHERE tasa_id = @tasaId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@estado", SqlDbType.Bit).Value = estado;
                cmd.Parameters.Add("@tasaId", SqlDbType.Int).Value = tasaId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
