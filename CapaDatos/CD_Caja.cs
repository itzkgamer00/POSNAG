using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Caja
    {
        public List<Caja> ListarActivas()
        {
            const string sql =
                "SELECT caja_id, nombre, estado, FechaCreacion " +
                "FROM Caja WHERE estado = 1 ORDER BY nombre";

            var lista = new List<Caja>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Caja
                        {
                            CajaId = Convert.ToInt32(dr["caja_id"]),
                            Nombre = dr["nombre"] as string,
                            Estado = Convert.ToBoolean(dr["estado"]),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        });
                    }
                }
            }

            return lista;
        }

        /// <summary>Todas las cajas (activas e inactivas), para la pantalla de administracion.</summary>
        public List<Caja> ListarTodas()
        {
            const string sql =
                "SELECT caja_id, nombre, estado, FechaCreacion " +
                "FROM Caja ORDER BY nombre";

            var lista = new List<Caja>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Caja
                        {
                            CajaId = Convert.ToInt32(dr["caja_id"]),
                            Nombre = dr["nombre"] as string,
                            Estado = Convert.ToBoolean(dr["estado"]),
                            FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                        });
                    }
                }
            }

            return lista;
        }

        /// <summary>True si ya existe una caja con ese nombre exacto, excluyendo (si se indica) la propia caja que se esta editando.</summary>
        public bool ExisteNombre(string nombre, int excluirCajaId = 0)
        {
            const string sql = "SELECT 1 FROM Caja WHERE nombre = @nombre AND caja_id <> @excluirCajaId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 200).Value = nombre;
                cmd.Parameters.Add("@excluirCajaId", SqlDbType.Int).Value = excluirCajaId;
                cn.Open();
                return cmd.ExecuteScalar() != null;
            }
        }

        /// <summary>Inserta una nueva caja registradora y devuelve la fila recien creada.</summary>
        public Caja Registrar(string nombre)
        {
            const string sql =
                "INSERT INTO Caja (nombre) " +
                "OUTPUT INSERTED.caja_id, INSERTED.nombre, INSERTED.estado, INSERTED.FechaCreacion " +
                "VALUES (@nombre)";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 200).Value = nombre;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    dr.Read();
                    return new Caja
                    {
                        CajaId = Convert.ToInt32(dr["caja_id"]),
                        Nombre = dr["nombre"] as string,
                        Estado = Convert.ToBoolean(dr["estado"]),
                        FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
                    };
                }
            }
        }

        /// <summary>Renombra una caja existente.</summary>
        public void Actualizar(int cajaId, string nombre)
        {
            const string sql = "UPDATE Caja SET nombre = @nombre WHERE caja_id = @cajaId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@nombre", SqlDbType.NVarChar, 200).Value = nombre;
                cmd.Parameters.Add("@cajaId", SqlDbType.Int).Value = cajaId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Activa o desactiva una caja.</summary>
        public void CambiarEstado(int cajaId, bool estado)
        {
            const string sql = "UPDATE Caja SET estado = @estado WHERE caja_id = @cajaId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@estado", SqlDbType.Bit).Value = estado;
                cmd.Parameters.Add("@cajaId", SqlDbType.Int).Value = cajaId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
