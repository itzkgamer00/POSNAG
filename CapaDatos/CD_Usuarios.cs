using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Usuario
    {
        public List<Usuario> Listar()

        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection oconexion = new SqlConnection(conexiondb.cadena))
            {
                try
                {

                    string query = "SELECT usuario_id, NombreCompleto, usuario, password_hash, IdRol, estado, fechacreacion FROM Usuarios";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = System.Data.CommandType.Text;

                    oconexion.Open();
                    
                    using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Usuario obj = new Usuario
                                {
                                    usuario_id = Convert.ToInt32(dr["usuario_id"]),
                                    NombreCompleto = dr["NombreCompleto"].ToString(),
                                    usuario = dr["usuario"].ToString(),
                                    password_hash = dr["password_hash"].ToString(),
                                    IdRol = Convert.ToInt32(dr["IdRol"]),
                                    estado = Convert.ToBoolean(dr["estado"]),
                                    fechacreacion = Convert.ToDateTime(dr["fechacreacion"])
                                };
                                lista.Add(obj);
                            }
                        }
                    

                }
                catch (Exception ex)
                {
                    throw new Exception("Error al listar usuarios: " + ex.Message);
                }
                finally
                {
                    if (oconexion.State == System.Data.ConnectionState.Open)
                    {
                        oconexion.Close();
                    }
                }
            }
            return lista;   
        }
    }
}
