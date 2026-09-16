using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Transaccion
    {
        private const string SelectBase =
            "SELECT t.transaccion_id, t.apertura_id, t.caja_id, t.usuario_id, t.concepto_id, t.moneda_id, " +
            "       t.forma_pago_id, t.tipo, t.monto, t.descripcion, t.fecha_hora, t.estado, " +
            "       c.nombre AS ConceptoNombre, c.operacion AS ConceptoOperacion, " +
            "       m.nombre AS MonedaNombre, m.codigo AS MonedaCodigo, m.simbolo AS MonedaSimbolo, " +
            "       fp.nombre AS FormaPagoNombre, cj.nombre AS CajaNombre, u.NombreCompleto AS UsuarioNombre " +
            "FROM Transacciones t " +
            "JOIN Concepto c ON c.concepto_id = t.concepto_id " +
            "JOIN Moneda m ON m.moneda_id = t.moneda_id " +
            "JOIN Caja cj ON cj.caja_id = t.caja_id " +
            "JOIN Usuario u ON u.usuario_id = t.usuario_id " +
            "LEFT JOIN FormaPago fp ON fp.forma_pago_id = t.forma_pago_id ";

        /// <summary>Movimientos (ingresos/egresos) de una apertura, del mas reciente al mas antiguo.</summary>
        public List<Transaccion> ListarPorApertura(int aperturaId) =>
            EjecutarPorApertura(SelectBase + "WHERE t.apertura_id = @aperturaId ORDER BY t.fecha_hora DESC", aperturaId);

        /// <summary>Solo los movimientos de Mesa de Cambio (conceptos CAMBIO_DIVISA_*) de una apertura.</summary>
        public List<Transaccion> ListarCambiosDivisaPorApertura(int aperturaId) =>
            EjecutarPorApertura(SelectBase +
                "WHERE t.apertura_id = @aperturaId AND c.operacion LIKE 'CAMBIO_DIVISA_%' " +
                "ORDER BY t.fecha_hora DESC", aperturaId);

        /// <summary>
        /// Movimientos filtrados por rango de fecha (hasta exclusivo) y, opcionalmente, caja/usuario/concepto.
        /// Para reportes: no esta limitado a una sola apertura.
        /// </summary>
        public List<Transaccion> ListarParaReporte(DateTime desde, DateTime hastaExclusiva, int? cajaId, int? usuarioId, int? conceptoId)
        {
            var condiciones = new List<string> { "t.fecha_hora >= @desde", "t.fecha_hora < @hasta" };
            if (cajaId.HasValue) condiciones.Add("t.caja_id = @cajaId");
            if (usuarioId.HasValue) condiciones.Add("t.usuario_id = @usuarioId");
            if (conceptoId.HasValue) condiciones.Add("t.concepto_id = @conceptoId");

            string sql = SelectBase + "WHERE " + string.Join(" AND ", condiciones) + " ORDER BY t.fecha_hora DESC";

            return Ejecutar(sql, cmd =>
            {
                cmd.Parameters.Add("@desde", SqlDbType.DateTime).Value = desde;
                cmd.Parameters.Add("@hasta", SqlDbType.DateTime).Value = hastaExclusiva;
                if (cajaId.HasValue) cmd.Parameters.Add("@cajaId", SqlDbType.Int).Value = cajaId.Value;
                if (usuarioId.HasValue) cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = usuarioId.Value;
                if (conceptoId.HasValue) cmd.Parameters.Add("@conceptoId", SqlDbType.Int).Value = conceptoId.Value;
            });
        }

        /// <summary>
        /// Solo movimientos de Mesa de Cambio (conceptos CAMBIO_DIVISA_*), filtrados por rango de fecha
        /// (hasta exclusivo) y, opcionalmente, caja/usuario. Para el reporte de Mesa de Cambio.
        /// </summary>
        public List<Transaccion> ListarCambiosDivisaParaReporte(DateTime desde, DateTime hastaExclusiva, int? cajaId, int? usuarioId)
        {
            var condiciones = new List<string>
            {
                "t.fecha_hora >= @desde", "t.fecha_hora < @hasta", "c.operacion LIKE 'CAMBIO_DIVISA_%'"
            };
            if (cajaId.HasValue) condiciones.Add("t.caja_id = @cajaId");
            if (usuarioId.HasValue) condiciones.Add("t.usuario_id = @usuarioId");

            string sql = SelectBase + "WHERE " + string.Join(" AND ", condiciones) + " ORDER BY t.fecha_hora DESC";

            return Ejecutar(sql, cmd =>
            {
                cmd.Parameters.Add("@desde", SqlDbType.DateTime).Value = desde;
                cmd.Parameters.Add("@hasta", SqlDbType.DateTime).Value = hastaExclusiva;
                if (cajaId.HasValue) cmd.Parameters.Add("@cajaId", SqlDbType.Int).Value = cajaId.Value;
                if (usuarioId.HasValue) cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = usuarioId.Value;
            });
        }

        private List<Transaccion> EjecutarPorApertura(string sql, int aperturaId) =>
            Ejecutar(sql, cmd => cmd.Parameters.Add("@aperturaId", SqlDbType.Int).Value = aperturaId);

        private List<Transaccion> Ejecutar(string sql, Action<SqlCommand> agregarParametros)
        {
            var lista = new List<Transaccion>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                agregarParametros(cmd);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        lista.Add(Mapear(dr));
                }
            }

            return lista;
        }

        private static Transaccion Mapear(SqlDataReader dr)
        {
            return new Transaccion
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
                ConceptoOperacion = dr["ConceptoOperacion"] as string,
                MonedaNombre = dr["MonedaNombre"] as string,
                MonedaCodigo = dr["MonedaCodigo"] as string,
                MonedaSimbolo = dr["MonedaSimbolo"] as string,
                FormaPagoNombre = dr["FormaPagoNombre"] as string,
                CajaNombre = dr["CajaNombre"] as string,
                UsuarioNombre = dr["UsuarioNombre"] as string
            };
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

        /// <summary>Anula (estado = 0) una transaccion activa. Devuelve false si no existe o ya estaba anulada.</summary>
        public bool Anular(int transaccionId)
        {
            const string sql = "UPDATE Transacciones SET estado = 0 WHERE transaccion_id = @transaccionId AND estado = 1";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@transaccionId", SqlDbType.Int).Value = transaccionId;
                cn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
