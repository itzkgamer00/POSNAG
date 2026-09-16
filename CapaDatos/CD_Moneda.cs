using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Moneda
    {
        private const string SelectBase =
            "SELECT moneda_id, nombre, codigo, simbolo, estado, FechaCreacion FROM Moneda ";

        /// <summary>Monedas activas, para combos de seleccion (apertura de caja, mesa de cambio, etc.).</summary>
        public List<Moneda> ListarActivas()
        {
            return Listar(SelectBase + "WHERE estado = 1 ORDER BY moneda_id");
        }

        /// <summary>Todas las monedas (activas e inactivas), para la pantalla de administracion.</summary>
        public List<Moneda> Listar()
        {
            return Listar(SelectBase + "ORDER BY nombre");
        }

        private static List<Moneda> Listar(string sql)
        {
            var lista = new List<Moneda>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
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

        private static Moneda Mapear(SqlDataReader dr)
        {
            return new Moneda
            {
                MonedaId = Convert.ToInt32(dr["moneda_id"]),
                Nombre = dr["nombre"] as string,
                Codigo = dr["codigo"] as string,
                Simbolo = dr["simbolo"] as string,
                Estado = Convert.ToBoolean(dr["estado"]),
                FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
            };
        }

        /// <summary>True si ya existe una moneda con ese codigo, excluyendo (si se indica) la propia moneda que se esta editando.</summary>
        public bool ExisteCodigo(string codigo, int excluirMonedaId = 0)
        {
            const string sql = "SELECT 1 FROM Moneda WHERE codigo = @codigo AND moneda_id <> @excluirMonedaId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 10).Value = codigo;
                cmd.Parameters.Add("@excluirMonedaId", SqlDbType.Int).Value = excluirMonedaId;
                cn.Open();
                return cmd.ExecuteScalar() != null;
            }
        }

        /// <summary>Inserta una nueva moneda y devuelve su moneda_id generado.</summary>
        public int Registrar(string nombre, string codigo, string simbolo)
        {
            const string sql =
                "INSERT INTO Moneda (nombre, codigo, simbolo) " +
                "OUTPUT INSERTED.moneda_id " +
                "VALUES (@nombre, @codigo, @simbolo)";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre;
                cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 10).Value = codigo;
                cmd.Parameters.Add("@simbolo", SqlDbType.VarChar, 10).Value = (object)simbolo ?? DBNull.Value;

                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>Actualiza nombre, codigo y simbolo de una moneda existente.</summary>
        public void Actualizar(int monedaId, string nombre, string codigo, string simbolo)
        {
            const string sql =
                "UPDATE Moneda SET nombre = @nombre, codigo = @codigo, simbolo = @simbolo " +
                "WHERE moneda_id = @monedaId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre;
                cmd.Parameters.Add("@codigo", SqlDbType.VarChar, 10).Value = codigo;
                cmd.Parameters.Add("@simbolo", SqlDbType.VarChar, 10).Value = (object)simbolo ?? DBNull.Value;
                cmd.Parameters.Add("@monedaId", SqlDbType.Int).Value = monedaId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Activa o desactiva una moneda.</summary>
        public void CambiarEstado(int monedaId, bool estado)
        {
            const string sql = "UPDATE Moneda SET estado = @estado WHERE moneda_id = @monedaId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@estado", SqlDbType.Bit).Value = estado;
                cmd.Parameters.Add("@monedaId", SqlDbType.Int).Value = monedaId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
