using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_AperturaCaja
    {
        /// <summary>Apertura en estado 'ABIERTA' para la caja indicada, o null si no hay ninguna.</summary>
        public AperturaCaja ObtenerAperturaAbierta(int cajaId)
        {
            const string sql =
                "SELECT a.apertura_id, a.caja_id, a.usuario_id, a.fecha_hora, a.estado, a.observaciones, c.nombre AS CajaNombre " +
                "FROM AperturaCaja a " +
                "JOIN Caja c ON c.caja_id = a.caja_id " +
                "WHERE a.caja_id = @cajaId AND a.estado = 'ABIERTA'";

            AperturaCaja apertura = LeerAperturaUnica(sql, cmd =>
                cmd.Parameters.Add("@cajaId", SqlDbType.Int).Value = cajaId);

            if (apertura != null)
                apertura.Montos = ObtenerMontos(apertura.AperturaId);

            return apertura;
        }

        /// <summary>La apertura ABIERTA mas reciente, sin importar la caja (para restaurar el estado de la UI al iniciar).</summary>
        public AperturaCaja ObtenerUltimaAperturaAbierta()
        {
            const string sql =
                "SELECT TOP 1 a.apertura_id, a.caja_id, a.usuario_id, a.fecha_hora, a.estado, a.observaciones, c.nombre AS CajaNombre " +
                "FROM AperturaCaja a " +
                "JOIN Caja c ON c.caja_id = a.caja_id " +
                "WHERE a.estado = 'ABIERTA' " +
                "ORDER BY a.fecha_hora DESC";

            AperturaCaja apertura = LeerAperturaUnica(sql, cmd => { });

            if (apertura != null)
                apertura.Montos = ObtenerMontos(apertura.AperturaId);

            return apertura;
        }

        private static AperturaCaja LeerAperturaUnica(string sql, Action<SqlCommand> configurarParametros)
        {
            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                configurarParametros(cmd);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if (!dr.Read())
                        return null;

                    return new AperturaCaja
                    {
                        AperturaId = Convert.ToInt32(dr["apertura_id"]),
                        CajaId = Convert.ToInt32(dr["caja_id"]),
                        UsuarioId = Convert.ToInt32(dr["usuario_id"]),
                        FechaHora = Convert.ToDateTime(dr["fecha_hora"]),
                        Estado = dr["estado"].ToString(),
                        Observaciones = dr["observaciones"] as string,
                        CajaNombre = dr["CajaNombre"] as string
                    };
                }
            }
        }

        private static List<AperturaCajaMoneda> ObtenerMontos(int aperturaId)
        {
            const string sql =
                "SELECT am.apertura_moneda_id, am.apertura_id, am.moneda_id, am.monto_inicial, " +
                "       m.nombre AS MonedaNombre, m.codigo AS MonedaCodigo, m.simbolo AS MonedaSimbolo " +
                "FROM AperturaCajaMoneda am " +
                "JOIN Moneda m ON m.moneda_id = am.moneda_id " +
                "WHERE am.apertura_id = @aperturaId " +
                "ORDER BY m.codigo";

            var lista = new List<AperturaCajaMoneda>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@aperturaId", SqlDbType.Int).Value = aperturaId;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new AperturaCajaMoneda
                        {
                            AperturaMonedaId = Convert.ToInt32(dr["apertura_moneda_id"]),
                            AperturaId = Convert.ToInt32(dr["apertura_id"]),
                            MonedaId = Convert.ToInt32(dr["moneda_id"]),
                            MontoInicial = Convert.ToDecimal(dr["monto_inicial"]),
                            MonedaNombre = dr["MonedaNombre"] as string,
                            MonedaCodigo = dr["MonedaCodigo"] as string,
                            MonedaSimbolo = dr["MonedaSimbolo"] as string
                        });
                    }
                }
            }

            return lista;
        }

        /// <summary>Inserta la apertura y su detalle por moneda en una sola transaccion. Devuelve el apertura_id generado.</summary>
        public int Abrir(AperturaCaja apertura)
        {
            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        int aperturaId;
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO AperturaCaja (caja_id, usuario_id, observaciones) " +
                            "OUTPUT INSERTED.apertura_id " +
                            "VALUES (@cajaId, @usuarioId, @observaciones)", cn, tx))
                        {
                            cmd.Parameters.Add("@cajaId", SqlDbType.Int).Value = apertura.CajaId;
                            cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = apertura.UsuarioId;
                            cmd.Parameters.Add("@observaciones", SqlDbType.NVarChar, 255).Value =
                                (object)apertura.Observaciones ?? DBNull.Value;

                            aperturaId = (int)cmd.ExecuteScalar();
                        }

                        foreach (AperturaCajaMoneda monto in apertura.Montos)
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                "INSERT INTO AperturaCajaMoneda (apertura_id, moneda_id, monto_inicial) " +
                                "VALUES (@aperturaId, @monedaId, @monto)", cn, tx))
                            {
                                cmd.Parameters.Add("@aperturaId", SqlDbType.Int).Value = aperturaId;
                                cmd.Parameters.Add("@monedaId", SqlDbType.Int).Value = monto.MonedaId;
                                cmd.Parameters.Add("@monto", SqlDbType.Decimal, 0).Value = monto.MontoInicial;
                                cmd.Parameters["@monto"].Precision = 18;
                                cmd.Parameters["@monto"].Scale = 2;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                        return aperturaId;
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
        /// Monto esperado en sistema por moneda: monto_inicial + ingresos - egresos
        /// (transacciones activas de esa apertura). Clave = moneda_id.
        /// </summary>
        public Dictionary<int, decimal> CalcularMontoSistema(int aperturaId)
        {
            const string sql =
                "SELECT am.moneda_id, " +
                "  am.monto_inicial " +
                "  + ISNULL((SELECT SUM(t.monto) FROM Transacciones t " +
                "            WHERE t.apertura_id = am.apertura_id AND t.moneda_id = am.moneda_id " +
                "              AND t.tipo = 'INGRESO' AND t.estado = 1), 0) " +
                "  - ISNULL((SELECT SUM(t.monto) FROM Transacciones t " +
                "            WHERE t.apertura_id = am.apertura_id AND t.moneda_id = am.moneda_id " +
                "              AND t.tipo = 'EGRESO' AND t.estado = 1), 0) AS MontoSistema " +
                "FROM AperturaCajaMoneda am " +
                "WHERE am.apertura_id = @aperturaId";

            var resultado = new Dictionary<int, decimal>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@aperturaId", SqlDbType.Int).Value = aperturaId;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        resultado[Convert.ToInt32(dr["moneda_id"])] = Convert.ToDecimal(dr["MontoSistema"]);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Historial de sesiones de caja cerradas, una fila por (cierre, moneda),
        /// del cierre mas reciente al mas antiguo.
        /// </summary>
        public List<HistorialCierre> ListarHistorialCierres()
        {
            const string sql =
                "SELECT a.apertura_id, c.cierre_id, caja.nombre AS CajaNombre, " +
                "       ua.NombreCompleto AS UsuarioAperturaNombre, uc.NombreCompleto AS UsuarioCierreNombre, " +
                "       a.fecha_hora AS FechaApertura, c.fecha_hora AS FechaCierre, " +
                "       m.nombre AS MonedaNombre, m.codigo AS MonedaCodigo, m.simbolo AS MonedaSimbolo, " +
                "       ISNULL(am.monto_inicial, 0) AS MontoInicial, cm.monto_sistema AS MontoSistema, " +
                "       cm.monto_final AS MontoFinal, cm.diferencia AS Diferencia, " +
                "       a.observaciones AS ObservacionesApertura, c.observaciones AS ObservacionesCierre " +
                "FROM CierreCaja c " +
                "JOIN AperturaCaja a ON a.apertura_id = c.apertura_id " +
                "JOIN Caja caja ON caja.caja_id = a.caja_id " +
                "JOIN Usuario ua ON ua.usuario_id = a.usuario_id " +
                "JOIN Usuario uc ON uc.usuario_id = c.usuario_id " +
                "JOIN CierreCajaMoneda cm ON cm.cierre_id = c.cierre_id " +
                "JOIN Moneda m ON m.moneda_id = cm.moneda_id " +
                "LEFT JOIN AperturaCajaMoneda am ON am.apertura_id = a.apertura_id AND am.moneda_id = cm.moneda_id " +
                "ORDER BY c.fecha_hora DESC, m.codigo";

            var lista = new List<HistorialCierre>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new HistorialCierre
                        {
                            AperturaId = Convert.ToInt32(dr["apertura_id"]),
                            CierreId = Convert.ToInt32(dr["cierre_id"]),
                            CajaNombre = dr["CajaNombre"] as string,
                            UsuarioAperturaNombre = dr["UsuarioAperturaNombre"] as string,
                            UsuarioCierreNombre = dr["UsuarioCierreNombre"] as string,
                            FechaApertura = Convert.ToDateTime(dr["FechaApertura"]),
                            FechaCierre = Convert.ToDateTime(dr["FechaCierre"]),
                            MonedaNombre = dr["MonedaNombre"] as string,
                            MonedaCodigo = dr["MonedaCodigo"] as string,
                            MonedaSimbolo = dr["MonedaSimbolo"] as string,
                            MontoInicial = Convert.ToDecimal(dr["MontoInicial"]),
                            MontoSistema = Convert.ToDecimal(dr["MontoSistema"]),
                            MontoFinal = Convert.ToDecimal(dr["MontoFinal"]),
                            Diferencia = Convert.ToDecimal(dr["Diferencia"]),
                            ObservacionesApertura = dr["ObservacionesApertura"] as string,
                            ObservacionesCierre = dr["ObservacionesCierre"] as string
                        });
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Inserta el cierre y su detalle por moneda, y marca la apertura como CERRADA,
        /// todo en una sola transaccion. Devuelve el cierre_id generado.
        /// </summary>
        public int Cerrar(CierreCaja cierre)
        {
            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            {
                cn.Open();
                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        int cierreId;
                        using (SqlCommand cmd = new SqlCommand(
                            "INSERT INTO CierreCaja (apertura_id, usuario_id, observaciones) " +
                            "OUTPUT INSERTED.cierre_id " +
                            "VALUES (@aperturaId, @usuarioId, @observaciones)", cn, tx))
                        {
                            cmd.Parameters.Add("@aperturaId", SqlDbType.Int).Value = cierre.AperturaId;
                            cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = cierre.UsuarioId;
                            cmd.Parameters.Add("@observaciones", SqlDbType.NVarChar, 255).Value =
                                (object)cierre.Observaciones ?? DBNull.Value;

                            cierreId = (int)cmd.ExecuteScalar();
                        }

                        foreach (CierreCajaMoneda monto in cierre.Montos)
                        {
                            using (SqlCommand cmd = new SqlCommand(
                                "INSERT INTO CierreCajaMoneda (cierre_id, moneda_id, monto_sistema, monto_final) " +
                                "VALUES (@cierreId, @monedaId, @sistema, @final)", cn, tx))
                            {
                                cmd.Parameters.Add("@cierreId", SqlDbType.Int).Value = cierreId;
                                cmd.Parameters.Add("@monedaId", SqlDbType.Int).Value = monto.MonedaId;

                                cmd.Parameters.Add("@sistema", SqlDbType.Decimal).Value = monto.MontoSistema;
                                cmd.Parameters["@sistema"].Precision = 18;
                                cmd.Parameters["@sistema"].Scale = 2;

                                cmd.Parameters.Add("@final", SqlDbType.Decimal).Value = monto.MontoFinal;
                                cmd.Parameters["@final"].Precision = 18;
                                cmd.Parameters["@final"].Scale = 2;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(
                            "UPDATE AperturaCaja SET estado = 'CERRADA' WHERE apertura_id = @aperturaId", cn, tx))
                        {
                            cmd.Parameters.Add("@aperturaId", SqlDbType.Int).Value = cierre.AperturaId;
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        return cierreId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
