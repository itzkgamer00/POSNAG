
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using CapaEntidad;


namespace CapaDatos
{
    public class CD_Usuario
    {
        public List<Usuario> listar()

        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection oconexion = new SqlConnection(conexiondb.cadena))
            {
                try
                {
                    string query = "select usuario_id, NombreCompleto, usuario, password_hash, IdRol, estado FROM Usuario";
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();
                    
                    using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                            
                            lista.Add(new Usuario
                                {
                                    usuario_id = Convert.ToInt32(dr["usuario_id"]),
                                    NombreCompleto = dr["NombreCompleto"].ToString(),
                                    usuario = dr["usuario"].ToString(),
                                    password_hash = dr["password_hash"].ToString(),                               
                                    estado = Convert.ToBoolean(dr["estado"])
                                });
                           
                            }
                        }
                    

                }
                catch (Exception ex)
                {
                  lista = new List<Usuario>();
                }
            }
            return lista;   
        }
    }
}
