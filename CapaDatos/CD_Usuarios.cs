using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Usuario
    {
        private const string SelectBase =
            "SELECT u.usuario_id, u.NombreCompleto, u.usuario, u.password_hash, " +
            "       u.IdRol, u.estado, r.Descripcion AS RolDescripcion " +
            "FROM Usuario u " +
            "LEFT JOIN Roles r ON r.IdRol = u.IdRol ";

        /// <summary>
        /// Devuelve un único usuario por su nombre de usuario, o null si no existe.
        /// La búsqueda se hace en el servidor con parámetros (no trae toda la tabla).
        /// </summary>
        public Usuario ObtenerPorUsuario(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
                return null;

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(SelectBase + "WHERE u.usuario = @usuario", cn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@usuario", SqlDbType.NVarChar, 50).Value = usuario.Trim();

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    return dr.Read() ? Mapear(dr) : null;
                }
            }
        }

        /// <summary>Lista todos los usuarios. Pensado para pantallas de administración.</summary>
        public List<Usuario> listar()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(SelectBase + "ORDER BY u.NombreCompleto", cn))
            {
                cmd.CommandType = CommandType.Text;
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                        lista.Add(Mapear(dr));
                }
            }

            return lista;
        }

        private static Usuario Mapear(SqlDataReader dr)
        {
            return new Usuario
            {
                usuario_id = Convert.ToInt32(dr["usuario_id"]),
                NombreCompleto = dr["NombreCompleto"] as string,
                usuario = dr["usuario"] as string,
                password_hash = dr["password_hash"] as string,
                IdRol = dr["IdRol"] == DBNull.Value ? 0 : Convert.ToInt32(dr["IdRol"]),
                RolDescripcion = dr["RolDescripcion"] as string,
                estado = dr["estado"] != DBNull.Value && Convert.ToBoolean(dr["estado"])
            };
        }
    }
}
