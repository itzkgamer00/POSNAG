using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Cliente
    {
        private const string SelectBase =
            "SELECT cliente_id, tipo_identificacion, numero_identificacion, nombres, apellidos, telefono, direccion, correo, fecha_registro FROM Clientes ";

        /// <summary>Todos los clientes registrados, para la pestaña Clientes.</summary>
        public List<Cliente> Listar()
        {
            var lista = new List<Cliente>();
            const string sql = SelectBase + "ORDER BY nombres, apellidos";

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

        /// <summary>Busca un cliente por tipo y numero de identificacion. Null si no existe.</summary>
        public Cliente BuscarPorIdentificacion(string tipoIdentificacion, string numeroIdentificacion)
        {
            const string sql = SelectBase + "WHERE tipo_identificacion = @tipo AND numero_identificacion = @numero";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@tipo", SqlDbType.NVarChar, 50).Value = tipoIdentificacion;
                cmd.Parameters.Add("@numero", SqlDbType.NVarChar, 50).Value = numeroIdentificacion;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    return dr.Read() ? Mapear(dr) : null;
                }
            }
        }

        private static Cliente Mapear(SqlDataReader dr)
        {
            return new Cliente
            {
                ClienteId = Convert.ToInt32(dr["cliente_id"]),
                TipoIdentificacion = dr["tipo_identificacion"] as string,
                NumeroIdentificacion = dr["numero_identificacion"] as string,
                Nombres = dr["nombres"] as string,
                Apellidos = dr["apellidos"] as string,
                Telefono = dr["telefono"] as string,
                Direccion = dr["direccion"] as string,
                Correo = dr["correo"] as string,
                FechaRegistro = Convert.ToDateTime(dr["fecha_registro"])
            };
        }

        /// <summary>Inserta un nuevo cliente y devuelve su cliente_id generado.</summary>
        public int Registrar(Cliente cliente)
        {
            const string sql =
                "INSERT INTO Clientes (tipo_identificacion, numero_identificacion, nombres, apellidos, telefono, direccion, correo) " +
                "OUTPUT INSERTED.cliente_id " +
                "VALUES (@tipo, @numero, @nombres, @apellidos, @telefono, @direccion, @correo)";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@tipo", SqlDbType.NVarChar, 50).Value = cliente.TipoIdentificacion;
                cmd.Parameters.Add("@numero", SqlDbType.NVarChar, 50).Value = cliente.NumeroIdentificacion;
                cmd.Parameters.Add("@nombres", SqlDbType.NVarChar, 100).Value = cliente.Nombres;
                cmd.Parameters.Add("@apellidos", SqlDbType.NVarChar, 100).Value = (object)cliente.Apellidos ?? DBNull.Value;
                cmd.Parameters.Add("@telefono", SqlDbType.NVarChar, 50).Value = (object)cliente.Telefono ?? DBNull.Value;
                cmd.Parameters.Add("@direccion", SqlDbType.NVarChar, 200).Value = (object)cliente.Direccion ?? DBNull.Value;
                cmd.Parameters.Add("@correo", SqlDbType.NVarChar, 150).Value = (object)cliente.Correo ?? DBNull.Value;
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>Elimina definitivamente un cliente.</summary>
        public void Eliminar(int clienteId)
        {
            const string sql = "DELETE FROM Clientes WHERE cliente_id = @clienteId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@clienteId", SqlDbType.Int).Value = clienteId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
