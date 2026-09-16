using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Roles
    {
        private const string SelectBase = "SELECT IdRol, Descripcion, estado, FechaCreacion FROM Roles ";

        /// <summary>Roles activos, para combos de seleccion al crear/editar un usuario.</summary>
        public List<Roles> ListarActivos()
        {
            return Listar(SelectBase + "WHERE estado = 1 ORDER BY Descripcion");
        }

        /// <summary>Todos los roles (activos e inactivos), para la pantalla de administracion.</summary>
        public List<Roles> Listar()
        {
            return Listar(SelectBase + "ORDER BY Descripcion");
        }

        private static List<Roles> Listar(string sql)
        {
            var lista = new List<Roles>();

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

        private static Roles Mapear(SqlDataReader dr)
        {
            return new Roles
            {
                IdRol = Convert.ToInt32(dr["IdRol"]),
                Descripcion = dr["Descripcion"] as string,
                estado = Convert.ToBoolean(dr["estado"]),
                FechaCreacion = Convert.ToDateTime(dr["FechaCreacion"])
            };
        }

        /// <summary>True si ya existe un rol con esa descripcion, excluyendo (si se indica) el propio rol que se esta editando.</summary>
        public bool ExisteDescripcion(string descripcion, int excluirIdRol = 0)
        {
            const string sql = "SELECT 1 FROM Roles WHERE Descripcion = @descripcion AND IdRol <> @excluirIdRol";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar, 50).Value = descripcion;
                cmd.Parameters.Add("@excluirIdRol", SqlDbType.Int).Value = excluirIdRol;
                cn.Open();
                return cmd.ExecuteScalar() != null;
            }
        }

        /// <summary>Inserta un nuevo rol y devuelve su IdRol generado.</summary>
        public int Registrar(string descripcion)
        {
            const string sql =
                "INSERT INTO Roles (Descripcion) OUTPUT INSERTED.IdRol VALUES (@descripcion)";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar, 50).Value = descripcion;
                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>Actualiza la descripcion de un rol existente.</summary>
        public void Actualizar(int idRol, string descripcion)
        {
            const string sql = "UPDATE Roles SET Descripcion = @descripcion WHERE IdRol = @idRol";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar, 50).Value = descripcion;
                cmd.Parameters.Add("@idRol", SqlDbType.Int).Value = idRol;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Activa o desactiva un rol.</summary>
        public void CambiarEstado(int idRol, bool estado)
        {
            const string sql = "UPDATE Roles SET estado = @estado WHERE IdRol = @idRol";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@estado", SqlDbType.Bit).Value = estado;
                cmd.Parameters.Add("@idRol", SqlDbType.Int).Value = idRol;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
