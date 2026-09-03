using MySqlConnector;

namespace Inmobiliaria.Models;

public class RepositorioTipoInmueble : RepositorioBase, IRepositorioTipoInmueble
{
    public RepositorioTipoInmueble(IConfiguration configuration) : base(configuration) {}

    public int Alta(TipoInmueble entidad)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO TipoInmueble (Nombre) VALUES (@nombre);
                            SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", entidad.Nombre);
                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
                entidad.IdTipoInmueble =  res;
            }
        }
        return res;
    }

    public int Baja(int id)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = "DELETE FROM TipoInmueble WHERE IdTipoInmueble = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    public int Modificacion(TipoInmueble entidad)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = "UPDATE TipoInmueble SET Nombre = @nombre WHERE IdTipoInmueble = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@nombre", entidad.Nombre);
                command.Parameters.AddWithValue("@id", entidad.IdTipoInmueble);
                connection.Open();
                res = command.ExecuteNonQuery();
            }
        }
        return res;
    }

    public TipoInmueble? ObtenerPorId(int id)
    {
        TipoInmueble? entidad = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = "SELECT IdTipoInmueble, Nombre FROM TipoInmueble WHERE IdTipoInmueble = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        entidad = new TipoInmueble
                        {
                            IdTipoInmueble = reader.GetInt32(0),
                            Nombre = reader.GetString(1)
                        };
                    }
                }
            }
        }
        return entidad;
    }

    public IList<TipoInmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
    {
        var res = new List<TipoInmueble>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = "SELECT IdTipoInmueble, Nombre FROM TipoInmueble LIMIT @tamPagina OFFSET @offset";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@tamPagina", tamPagina);
                command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new TipoInmueble
                        {
                            IdTipoInmueble = reader.GetInt32(0),
                            Nombre = reader.GetString(1)
                        });
                    }
                }
            }
        }
        return res;
    }

    public int ObtenerCantidad()
    {
        int res = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = "SELECT COUNT(*) FROM TipoInmueble";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                res = Convert.ToInt32(command.ExecuteScalar());
            }
        }
        return res;
    }

    public IList<TipoInmueble> ObtenerTodos()
    {
        var res = new List<TipoInmueble>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = "SELECT IdTipoInmueble, Nombre FROM TipoInmueble";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(new TipoInmueble
                        {
                            IdTipoInmueble = reader.GetInt32(0),
                            Nombre = reader.GetString(1)
                        });
                    }
                }
            }
        }
        return res;
    }
}