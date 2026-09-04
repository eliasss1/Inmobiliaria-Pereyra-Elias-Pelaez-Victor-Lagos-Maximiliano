using System.Data;
using MySqlConnector;

namespace Inmobiliaria.Models;

public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario

{
    public RepositorioUsuario(IConfiguration configuration) : base(configuration)
    {
        
    }

    public int Alta(Usuario p)
    {
        int res = -1;

        using (MySqlConnection conexion = new MySqlConnection (connectionString))
        {
            try {
                string query = @"INSERT INTO Usuario 
                                (Email, Nombre, Apellido, Clave, Avatar, Rol)
                                VALUES (@email, @nombre, @apellido, @contraseña, @avatar, @rol); 
                                SELECT LAST_INSERT_ID()";
                
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@email", p.Email);
                    comando.Parameters.AddWithValue("@nombre", p.Nombre);
                    comando.Parameters.AddWithValue("@apellido", p.Apellido);
                    comando.Parameters.AddWithValue("@contraseña", p.Clave);
                    comando.Parameters.AddWithValue("@avatar", p.Avatar);
                    comando.Parameters.AddWithValue("@rol", p.Rol);
                    conexion.Open();
                    res = Convert.ToInt32(comando.ExecuteScalar());
                    p.IdUsuario = res;
                    
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error al insertar usuario: {ex.Message}");

            }
            finally {
                conexion.Close();   
            }

            return res;
        }
    }

    public int Baja(int id)
    {
        int res = -1;

        using (MySqlConnection conexion = new MySqlConnection (connectionString))
        {
            try {
                string query = @"DELETE FROM Usuario WHERE IdUsuario = @IdUsuario";
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@IdUsuario", id);
                    conexion.Open();
                    res = comando.ExecuteNonQuery();
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
            }
            finally {
                conexion.Close();   
            }

            return res;
        }
    }

    public int Modificacion(Usuario p)
    {
        int res = -1;
        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"UPDATE Usuario 
                SET Email = @email, Nombre = @nombre, Apellido = @apellido, Contraseña = @contraseña, Avatar = @avatar, Rol = @rol 
                WHERE IdUsuario = @IdUsuario";
                
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@IdUsuario", p.IdUsuario);
                    comando.Parameters.AddWithValue("@email", p.Email);
                    comando.Parameters.AddWithValue("@nombre", p.Nombre);
                    comando.Parameters.AddWithValue("@apellido", p.Apellido);
                    comando.Parameters.AddWithValue("@contraseña", p.Clave);
                    comando.Parameters.AddWithValue("@avatar", p.Avatar);
                    comando.Parameters.AddWithValue("@rol", p.Rol);
                    conexion.Open();
                    res = comando.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al modificar usuario: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
            return res;
        }
    }

    public int ObtenerCantidad()
    {
        int res = -1;
        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = "SELECT COUNT(IdUsuario) FROM Usuario";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    conexion.Open();
                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        res = reader.GetInt32(0);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al obtener cantidad de usuarios: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }
        
        return res;
    }

    public IList<Usuario> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
    {
        IList<Usuario> res = new List<Usuario>();
        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @$"
                                SELECT IdUsuario, Nombre, Apellido, Email, Avatar, Rol
                                FROM Usuario
                                ORDER BY IdUsuario
                                OFFSET {(paginaNro - 1) * tamPagina} ROW 
                                FETCH NEXT {tamPagina} ROWS ONLY
                ";
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    conexion.Open();
                    var reader = comando.ExecuteReader();
                    while(reader.Read())
                    {
                        Usuario p = new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            Apellido = reader.GetString(nameof(Usuario.Apellido)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Avatar = reader.IsDBNull(nameof(Usuario.Avatar)) ? null : reader.GetString(nameof(Usuario.Avatar)),
                            Rol = reader.GetString(nameof(Usuario.Rol))
                        };
                        res.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener lista de usuarios: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }
        return res;
    }

    public Usuario? ObtenerPorId(int id)
    {
        Usuario? p = null;
        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @$"SELECT IdUsuario, Nombre, Apellido, Email, Avatar, Rol 
                FROM Usuario 
                WHERE IdUsuario = @IdUsuario";
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.Add("@IdUsuario", MySqlDbType.Int32).Value = id;
                    conexion.Open();
                    var reader = comando.ExecuteReader();
                    if(reader.Read())
                    {
                        p = new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            Apellido = reader.GetString(nameof(Usuario.Apellido)),
                            Avatar = reader.IsDBNull(nameof(Usuario.Avatar)) ? null : reader.GetString(nameof(Usuario.Avatar)),
                            Rol = reader.GetString(nameof(Usuario.Rol)),
                        };
                        
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario por id: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }

        }
        return p;
    }

    public Usuario ObtenerPorEmail(String email)
    {
        Usuario? res = null;

        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @$"SELECT IdUsuario, Nombre, Apellido, Email, Clave,Avatar, Rol 
                FROM Usuario 
                WHERE Email = @Email";
                using (MySqlCommand comando = new MySqlCommand(query, conexion)) 
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.Add("@Email", MySqlDbType.VarChar).Value = email;
                    conexion.Open();
                    var reader = comando.ExecuteReader();
                    if(reader.Read())
                    {
                        res = new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            Apellido = reader.GetString(nameof(Usuario.Apellido)),
                            Clave = reader.GetString(nameof(Usuario.Clave)),
                            Avatar = reader.IsDBNull(nameof(Usuario.Avatar)) ? null : reader.GetString(nameof(Usuario.Avatar)),
                            Rol = reader.GetString(nameof(Usuario.Rol)),
                        };
                        
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario por Email: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }

        return res;
    }
}
