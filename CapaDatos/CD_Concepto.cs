using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Concepto
    {
        private const string SelectBase = "SELECT concepto_id, operacion, nombre, tipo, estado FROM Concepto ";

        /// <summary>Conceptos activos de un tipo ("INGRESO" o "EGRESO").</summary>
        public List<Concepto> ListarPorTipo(string tipo)
        {
            var lista = new List<Concepto>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(SelectBase + "WHERE estado = 1 AND tipo = @tipo ORDER BY nombre", cn))
            {
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = tipo;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        lista.Add(Mapear(dr));
                }
            }

            return lista;
        }

        /// <summary>Todos los conceptos (activos e inactivos, ingreso y egreso), para la pantalla de administracion.</summary>
        public List<Concepto> Listar()
        {
            var lista = new List<Concepto>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(SelectBase + "ORDER BY tipo, nombre", cn))
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

        /// <summary>Busca un concepto (activo o inactivo) por su concepto_id, o null si no existe.</summary>
        public Concepto ObtenerPorId(int conceptoId)
        {
            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(SelectBase + "WHERE concepto_id = @conceptoId", cn))
            {
                cmd.Parameters.Add("@conceptoId", SqlDbType.Int).Value = conceptoId;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    return dr.Read() ? Mapear(dr) : null;
                }
            }
        }

        /// <summary>Busca un concepto activo por su codigo de operacion unico (p.ej. "CAMBIO_DIVISA_RECIBIDO").</summary>
        public Concepto ObtenerPorOperacion(string operacion)
        {
            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(SelectBase + "WHERE estado = 1 AND operacion = @operacion", cn))
            {
                cmd.Parameters.Add("@operacion", SqlDbType.VarChar, 30).Value = operacion;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    return dr.Read() ? Mapear(dr) : null;
                }
            }
        }

        private static Concepto Mapear(SqlDataReader dr)
        {
            return new Concepto
            {
                ConceptoId = Convert.ToInt32(dr["concepto_id"]),
                Operacion = dr["operacion"] as string,
                Nombre = dr["nombre"] as string,
                Tipo = dr["tipo"] as string,
                Estado = Convert.ToBoolean(dr["estado"])
            };
        }

        /// <summary>True si ya existe un concepto con ese codigo de operacion, excluyendo (si se indica) el que se esta editando.</summary>
        public bool ExisteOperacion(string operacion, int excluirConceptoId = 0)
        {
            const string sql = "SELECT 1 FROM Concepto WHERE operacion = @operacion AND concepto_id <> @excluirConceptoId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@operacion", SqlDbType.VarChar, 30).Value = operacion;
                cmd.Parameters.Add("@excluirConceptoId", SqlDbType.Int).Value = excluirConceptoId;
                cn.Open();
                return cmd.ExecuteScalar() != null;
            }
        }

        /// <summary>Inserta un nuevo concepto y devuelve su concepto_id generado.</summary>
        public int Registrar(string operacion, string nombre, string tipo)
        {
            const string sql =
                "INSERT INTO Concepto (operacion, nombre, tipo) OUTPUT INSERTED.concepto_id VALUES (@operacion, @nombre, @tipo)";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@operacion", SqlDbType.VarChar, 30).Value = operacion;
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 200).Value = nombre;
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = tipo;
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>Actualiza el codigo, nombre y tipo de un concepto existente.</summary>
        public void Actualizar(int conceptoId, string operacion, string nombre, string tipo)
        {
            const string sql =
                "UPDATE Concepto SET operacion = @operacion, nombre = @nombre, tipo = @tipo WHERE concepto_id = @conceptoId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@operacion", SqlDbType.VarChar, 30).Value = operacion;
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 200).Value = nombre;
                cmd.Parameters.Add("@tipo", SqlDbType.VarChar, 10).Value = tipo;
                cmd.Parameters.Add("@conceptoId", SqlDbType.Int).Value = conceptoId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Activa o desactiva un concepto.</summary>
        public void CambiarEstado(int conceptoId, bool estado)
        {
            const string sql = "UPDATE Concepto SET estado = @estado WHERE concepto_id = @conceptoId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@estado", SqlDbType.Bit).Value = estado;
                cmd.Parameters.Add("@conceptoId", SqlDbType.Int).Value = conceptoId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Elimina definitivamente un concepto. Falla con SqlException (violacion de llave foranea) si
        /// existen transacciones registradas con ese concepto_id.
        /// </summary>
        public void Eliminar(int conceptoId)
        {
            const string sql = "DELETE FROM Concepto WHERE concepto_id = @conceptoId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@conceptoId", SqlDbType.Int).Value = conceptoId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
