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
            "       u.IdRol, u.estado, u.fechacreacion, r.Descripcion AS RolDescripcion " +
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
                estado = dr["estado"] != DBNull.Value && Convert.ToBoolean(dr["estado"]),
                fechacreacion = Convert.ToDateTime(dr["fechacreacion"])
            };
        }

        /// <summary>True si ya existe un usuario con ese nombre de usuario, excluyendo (si se indica) el propio usuario que se esta editando.</summary>
        public bool ExisteUsuario(string usuario, int excluirUsuarioId = 0)
        {
            const string sql = "SELECT 1 FROM Usuario WHERE usuario = @usuario AND usuario_id <> @excluirUsuarioId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@usuario", SqlDbType.NVarChar, 50).Value = usuario;
                cmd.Parameters.Add("@excluirUsuarioId", SqlDbType.Int).Value = excluirUsuarioId;
                cn.Open();
                return cmd.ExecuteScalar() != null;
            }
        }

        /// <summary>Inserta un nuevo usuario y devuelve su usuario_id generado.</summary>
        public int Registrar(Usuario usuario)
        {
            const string sql =
                "INSERT INTO Usuario (NombreCompleto, usuario, password_hash, IdRol, estado) " +
                "OUTPUT INSERTED.usuario_id " +
                "VALUES (@nombreCompleto, @usuario, @passwordHash, @idRol, @estado)";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@nombreCompleto", SqlDbType.NVarChar, 100).Value = usuario.NombreCompleto;
                cmd.Parameters.Add("@usuario", SqlDbType.NVarChar, 50).Value = usuario.usuario;
                cmd.Parameters.Add("@passwordHash", SqlDbType.VarChar, 200).Value = usuario.password_hash;
                cmd.Parameters.Add("@idRol", SqlDbType.Int).Value = usuario.IdRol == 0 ? (object)DBNull.Value : usuario.IdRol;
                cmd.Parameters.Add("@estado", SqlDbType.Bit).Value = usuario.estado;

                cn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>Actualiza nombre completo, nombre de usuario y rol. La contraseña se cambia con ActualizarPassword.</summary>
        public void Actualizar(int usuarioId, string nombreCompleto, string nombreUsuario, int? idRol)
        {
            const string sql =
                "UPDATE Usuario SET NombreCompleto = @nombreCompleto, usuario = @usuario, IdRol = @idRol " +
                "WHERE usuario_id = @usuarioId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@nombreCompleto", SqlDbType.NVarChar, 100).Value = nombreCompleto;
                cmd.Parameters.Add("@usuario", SqlDbType.NVarChar, 50).Value = nombreUsuario;
                cmd.Parameters.Add("@idRol", SqlDbType.Int).Value = (object)idRol ?? DBNull.Value;
                cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Reemplaza el hash de contraseña de un usuario existente.</summary>
        public void ActualizarPassword(int usuarioId, string passwordHash)
        {
            const string sql = "UPDATE Usuario SET password_hash = @passwordHash WHERE usuario_id = @usuarioId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@passwordHash", SqlDbType.VarChar, 200).Value = passwordHash;
                cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Activa o desactiva un usuario.</summary>
        public void CambiarEstado(int usuarioId, bool estado)
        {
            const string sql = "UPDATE Usuario SET estado = @estado WHERE usuario_id = @usuarioId";

            using (SqlConnection cn = new SqlConnection(conexiondb.cadena))
            using (SqlCommand cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.Add("@estado", SqlDbType.Bit).Value = estado;
                cmd.Parameters.Add("@usuarioId", SqlDbType.Int).Value = usuarioId;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
