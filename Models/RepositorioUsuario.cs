using System;
using System.Data;
using MySqlConnector;


namespace Inmobiliaria.Models;

public class RepositorioUsuario : RepositorioBase, IRepositorio<Usuario>

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
                                (email, nombre, contraseña, avatarUrl, rol)
                                VALUES (@email, @nombre, @contraseña, @avatarUrl, @rol); 
                                SELECT LAST_INSERT_ID()";
                
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@email", p.Email);
                    comando.Parameters.AddWithValue("@nombre", p.Nombre);
                    comando.Parameters.AddWithValue("@contraseña", p.Contraseña);
                    comando.Parameters.AddWithValue("@avatarUrl", p.AvatarUrl);
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
                string query = @"DELETE FROM Usuario WHERE idUsuario = @idUsuario";
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@idUsuario", id);
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
                SET email = @email, nombre = @nombre, contraseña = @contraseña, avatar Url = @avatarUrl, rol = @rol 
                WHERE idUsuario = @idUsuario";
                
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@idUsuario", p.IdUsuario);
                    comando.Parameters.AddWithValue("@email", p.Email);
                    comando.Parameters.AddWithValue("@nombre", p.Nombre);
                    comando.Parameters.AddWithValue("@contraseña", p.Contraseña);
                    comando.Parameters.AddWithValue("@avatarUrl", p.AvatarUrl);
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
                string query = "SELECT COUNT(idUsuario) FROM Usuario";

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
                                SELECT idUsuario, nombre, email, avatarUrl, rol
                                FROM Usuario
                                ORDER BY idUsuario
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
                            Email = reader.GetString(nameof(Usuario.Email)),
                            AvatarUrl = reader.IsDBNull(nameof(Usuario.AvatarUrl)) ? null : reader.GetString(nameof(Usuario.AvatarUrl)),
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
                string query = @$"SELECT idUsuario, nombre, email, avatarUrl, rol 
                FROM Usuario 
                WHERE idUsuario = @idUsuario";
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.Add("@idUsuario", MySqlDbType.Int32).Value = id;
                    conexion.Open();
                    var reader = comando.ExecuteReader();
                    if(reader.Read())
                    {
                        p = new Usuario
                        {
                            IdUsuario = reader.GetInt32(nameof(Usuario.IdUsuario)),
                            Email = reader.GetString(nameof(Usuario.Email)),
                            Nombre = reader.GetString(nameof(Usuario.Nombre)),
                            AvatarUrl = reader.IsDBNull(nameof(Usuario.AvatarUrl)) ? null : reader.GetString(nameof(Usuario.AvatarUrl)),
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
}
