using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_FormaPago
    {
        private const string SelectBase = "SELECT forma_pago_id, nombre, estado FROM FormaPago ";

        /// <summary>Formas de pago activas, para combos de seleccion (mesa de cambio, ingresos/egresos, etc.).</summary>
        public List<FormaPago> ListarActivas()
        {
            return Listar(SelectBase + "WHERE estado = 1 ORDER BY nombre");
        }

        /// <summary>Todas las formas de pago (activas e inactivas), para la pantalla de administracion.</summary>
        public List<FormaPago> Listar()
        {
            return Listar(SelectBase + "ORDER BY nombre");
        }

        private static List<FormaPago> Listar(string sql)
        {
            var lista = new List<FormaPago>();

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

        private static FormaPago Mapear(SqlDataReader dr)
        {
            return new FormaPago
            {
                FormaPagoId = Convert.ToInt32(dr["forma_pago_id"]),
                Nombre = dr["nombre"] as string,
                Estado = Convert.ToBoolean(dr["estado"])
            };
        }

        /// <summary>True si ya existe una forma de pago con ese nombre, excluyendo (si se indica) la que se esta editando.</summary>
        public bool ExisteNombre(string nombre, int excluirFormaPagoId = 0)
        {
            const string sql = "SELECT 1 FROM FormaPago WHERE nombre = @nombre AND forma_pago_id <> @excluirFormaPagoId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre;
                cmd.Parameters.Add("@excluirFormaPagoId", SqlDbType.Int).Value = excluirFormaPagoId;
                cn.Open();
                return cmd.ExecuteScalar() != null;
            }
        }

        /// <summary>Inserta una nueva forma de pago y devuelve su forma_pago_id generado.</summary>
        public int Registrar(string nombre)
        {
            const string sql =
                "INSERT INTO FormaPago (nombre) OUTPUT INSERTED.forma_pago_id VALUES (@nombre)";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre;
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>Renombra una forma de pago existente.</summary>
        public void Actualizar(int formaPagoId, string nombre)
        {
            const string sql = "UPDATE FormaPago SET nombre = @nombre WHERE forma_pago_id = @formaPagoId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 100).Value = nombre;
                cmd.Parameters.Add("@formaPagoId", SqlDbType.Int).Value = formaPagoId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Activa o desactiva una forma de pago.</summary>
        public void CambiarEstado(int formaPagoId, bool estado)
        {
            const string sql = "UPDATE FormaPago SET estado = @estado WHERE forma_pago_id = @formaPagoId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@estado", SqlDbType.Bit).Value = estado;
                cmd.Parameters.Add("@formaPagoId", SqlDbType.Int).Value = formaPagoId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
