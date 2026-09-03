
using System.Data;
using System.Runtime.InteropServices.Marshalling;
using MySqlConnector;

namespace Inmobiliaria.Models;

public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
{
    public RepositorioInmueble(IConfiguration configuration) : base(configuration)
    {
        
    }

public int Alta(Inmueble entidad)
{
    int res = -1;
    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = @"INSERT INTO Inmueble 
                    (Direccion, Cupo, Latitud, Longitud, PrecioPorDia, Estado, ImagenPortada, IdTipoInmueble, IdPropietario) 
                    VALUES (@direccion, @cupo, @latitud, @longitud, @precio, @estado, @imagen, @idTipo, @idPropietario); 
                    SELECT LAST_INSERT_ID();";
        
        using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@direccion", entidad.Direccion);
            command.Parameters.AddWithValue("@cupo", entidad.Cupo);
            command.Parameters.AddWithValue("@latitud", entidad.Latitud);
            command.Parameters.AddWithValue("@longitud", entidad.Longitud);
            command.Parameters.AddWithValue("@precio", entidad.PrecioPorDia);
            command.Parameters.AddWithValue("@estado", entidad.Estado);
            command.Parameters.AddWithValue("@imagen", entidad.ImagenPortada ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@idTipo", entidad.IdTipoInmueble);
            command.Parameters.AddWithValue("@idPropietario", entidad.IdPropietario);
            
            connection.Open();
            res = Convert.ToInt32(command.ExecuteScalar());
            entidad.IdInmueble = res;
        }
    }
    return res;
}

    public int Baja(int id)
    {
        int res = -1;
        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"DELETE FROM Inmueble 
                WHERE IdInmueble = @IdInmueble";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue(@"IdInmueble", id);
                    conexion.Open();
                    res = comando.ExecuteNonQuery();
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error al eliminar inmueble: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }

        }
        return res;
    }

    public int Modificacion(Inmueble entidad)
{
    int res = -1;
    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = @"UPDATE Inmueble 
                    SET Direccion = @direccion, Cupo = @cupo, Latitud = @latitud, Longitud = @longitud, 
                    PrecioPorDia = @precio, Estado = @estado, ImagenPortada = @imagen, 
                    IdTipoInmueble = @idTipo, IdPropietario = @idPropietario 
                    WHERE IdInmueble = @id";
        
        using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@direccion", entidad.Direccion);
            command.Parameters.AddWithValue("@cupo", entidad.Cupo);
            command.Parameters.AddWithValue("@latitud", entidad.Latitud);
            command.Parameters.AddWithValue("@longitud", entidad.Longitud);
            command.Parameters.AddWithValue("@precio", entidad.PrecioPorDia);
            command.Parameters.AddWithValue("@estado", entidad.Estado);
            command.Parameters.AddWithValue("@imagen", entidad.ImagenPortada ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@idTipo", entidad.IdTipoInmueble);
            command.Parameters.AddWithValue("@idPropietario", entidad.IdPropietario);
            command.Parameters.AddWithValue("@id", entidad.IdInmueble);
            
            connection.Open();
            res = command.ExecuteNonQuery();
        }
    }
    return res;
}

    public int ObtenerCantidad()
{
    int res = 0;
    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = "SELECT COUNT(*) FROM Inmueble";
        using (var command = new MySqlCommand(sql, connection))
        {
            connection.Open();
            res = Convert.ToInt32(command.ExecuteScalar());
        }
    }
    return res;
}

    public IList<Inmueble> ObtenerTodos()
{
    var res = new List<Inmueble>();
    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioPorDia, i.Estado, i.ImagenPortada,
                            i.IdTipoInmueble, t.Nombre AS TipoNombre,
                            i.IdPropietario, p.Nombre AS PropNombre, p.Apellido AS PropApellido
                    FROM Inmueble i
                    INNER JOIN TipoInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                    INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario";
        
        using (var command = new MySqlCommand(sql, connection))
        {
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Inmueble inmueble = new Inmueble
                    {
                        IdInmueble = reader.GetInt32(0),
                        Direccion = reader.GetString(1),
                        Cupo = reader.GetInt32(2),
                        Latitud = reader.GetDouble(3),
                        Longitud = reader.GetDouble(4),
                        PrecioPorDia = reader.GetDecimal(5),
                        Estado = reader.GetBoolean(6),
                        ImagenPortada = reader.IsDBNull(7) ? null : reader.GetString(7),
                        IdTipoInmueble = reader.GetInt32(8),
                        Tipo = new TipoInmueble
                        {
                            IdTipoInmueble = reader.GetInt32(8),
                            Nombre = reader.GetString(9)
                        },
                        
                        IdPropietario = reader.GetInt32(10),
                        Dueño = new Propietario
                        {
                            IdPropietario = reader.GetInt32(10),
                            Nombre = reader.GetString(11),
                            Apellido = reader.GetString(12)
                        }
                    };
                    res.Add(inmueble);
                }
            }
        }
    }
    return res;
}

    public Inmueble? ObtenerPorId(int id)
{
    Inmueble? inmueble = null;
    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioPorDia, i.Estado, i.ImagenPortada,
                            i.IdTipoInmueble, t.Nombre AS TipoNombre,
                            i.IdPropietario, p.Nombre AS PropNombre, p.Apellido AS PropApellido
                    FROM Inmueble i
                    INNER JOIN TipoInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                    INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                    WHERE i.IdInmueble = @id";
        
        using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    inmueble = new Inmueble
                    {
                        IdInmueble = reader.GetInt32(0),
                        Direccion = reader.GetString(1),
                        Cupo = reader.GetInt32(2),
                        Latitud = reader.GetDouble(3),
                        Longitud = reader.GetDouble(4),
                        PrecioPorDia = reader.GetDecimal(5),
                        Estado = reader.GetBoolean(6),
                        ImagenPortada = reader.IsDBNull(7) ? null : reader.GetString(7),
                        IdTipoInmueble = reader.GetInt32(8),
                        Tipo = new TipoInmueble
                        {
                            IdTipoInmueble = reader.GetInt32(8),
                            Nombre = reader.GetString(9)
                        },
                        IdPropietario = reader.GetInt32(10),
                        Dueño = new Propietario
                        {
                            IdPropietario = reader.GetInt32(10),
                            Nombre = reader.GetString(11),
                            Apellido = reader.GetString(12)
                        }
                    };
                }
            }
        }
    }
    return inmueble;
    }

    public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
{
    var res = new List<Inmueble>();
    using (var connection = new MySqlConnection(connectionString))
    {
        string sql = @"SELECT i.IdInmueble, i.Direccion, i.Cupo, i.Latitud, i.Longitud, i.PrecioPorDia, i.Estado, i.ImagenPortada,
                            i.IdTipoInmueble, t.Nombre AS TipoNombre,
                            i.IdPropietario, p.Nombre AS PropNombre, p.Apellido AS PropApellido
                    FROM Inmueble i
                    INNER JOIN TipoInmueble t ON i.IdTipoInmueble = t.IdTipoInmueble
                    INNER JOIN Propietarios p ON i.IdPropietario = p.IdPropietario
                    LIMIT @tamPagina OFFSET @offset";
        
        using (var command = new MySqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@tamPagina", tamPagina);
            command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);
            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    res.Add(new Inmueble
                    {
                        IdInmueble = reader.GetInt32(0),
                        Direccion = reader.GetString(1),
                        Cupo = reader.GetInt32(2),
                        Latitud = reader.GetDouble(3),
                        Longitud = reader.GetDouble(4),
                        PrecioPorDia = reader.GetDecimal(5),
                        Estado = reader.GetBoolean(6),
                        ImagenPortada = reader.IsDBNull(7) ? null : reader.GetString(7),
                        IdTipoInmueble = reader.GetInt32(8),
                        Tipo = new TipoInmueble
                        {
                            IdTipoInmueble = reader.GetInt32(8),
                            Nombre = reader.GetString(9)
                        },
                        IdPropietario = reader.GetInt32(10),
                        Dueño = new Propietario
                        {
                            IdPropietario = reader.GetInt32(10),
                            Nombre = reader.GetString(11),
                            Apellido = reader.GetString(12)
                        }
                    });
                }
            }
        }
    }
    return res;
}
}