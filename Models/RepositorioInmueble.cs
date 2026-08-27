
using System.Data;
using System.Runtime.InteropServices.Marshalling;
using MySqlConnector;

namespace Inmobiliaria.Models;

public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
{
    public RepositorioInmueble(IConfiguration configuration) : base(configuration)
    {
        
    }

    public int Alta(Inmueble p)
    {
        int res = -1;

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"INSERT INTO Inmueble (Direccion, Cupo,  Tipo, Latitud, Longitud, PrecioPorDia, Estado, IdPropietario)
                VALUES (@Direccion, @Cupo, @Tipo, @Latitud, @Longitud, @PrecioPorDia, @Estado, @IdPropietario)
                SELECT LAST_INSERT_ID()";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@Direccion", p.Direccion);
                    comando.Parameters.AddWithValue("@Cupo", p.Cupo);
                    comando.Parameters.AddWithValue("@Tipo", p.Tipo);
                    comando.Parameters.AddWithValue("@Latitud", p.Latitud);
                    comando.Parameters.AddWithValue("@Longitud", p.Longitud);
                    comando.Parameters.AddWithValue("@PrecioPorDia", p.PrecioPorDia);
                    comando.Parameters.AddWithValue("@Estado", p.Estado);
                    comando.Parameters.AddWithValue("@IdPropietario", p.IdPropietario);
                    conexion.Open();
                    res = Convert.ToInt32(comando.ExecuteScalar());
                    p.IdInmueble = res;
                }

            }catch(Exception ex)
            {
                Console.WriteLine($"Error al insertar inmueble: {ex.Message}");
            }
            finally
            {
                conexion.Close();
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

    public int Modificacion(Inmueble p)
    {
        int res = -1;
        
        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"UPDATE Inmueble
                SET Direccion = @Direccion, Cupo = @Cupo,  Tipo = @Tipo, Latitud = @Latitud, Longitud = @Longitud, PrecioPorDia = @PrecioPorDia, Estado = @Estado, IdPropietario = @IdPropietario
                WHERE IdInmueble = @IdInmueble";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@Direccion", p.Direccion);
                    comando.Parameters.AddWithValue("@Cupo", p.Cupo);
                    comando.Parameters.AddWithValue("@Tipo", p.Tipo);
                    comando.Parameters.AddWithValue("@Latitud", p.Latitud);
                    comando.Parameters.AddWithValue("@Longitud", p.Longitud);
                    comando.Parameters.AddWithValue("@PrecioPorDia", p.PrecioPorDia);
                    comando.Parameters.AddWithValue("@Estado", p.Estado);
                    comando.Parameters.AddWithValue("@IdPropietario", p.IdPropietario);
                    comando.Parameters.AddWithValue("@IdInmueble", p.IdInmueble);
                    conexion.Open();
                    res = comando.ExecuteNonQuery();

                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error al modificar la tabla inmueble {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }
        return res;
    }

    public int ObtenerCantidad()
    {
        int res = -1;
        
        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"SELECT COUNT(IdInmueble) FROM Inmueble";

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
            }catch(Exception ex)
            {
                Console.WriteLine($"Error al obtener la cantidad de la tabla inmueble {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }
        return res;
    }

    public IList<Inmueble> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
    {
        IList<Inmueble> res = new List<Inmueble>();
        
        int offset = (paginaNro - 1) * tamPagina;

        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"SELECT IdInmueble, Direccion, Cupo, Tipo, Latitud, Longitud, PrecioPorDia, Estado, IdPropietario
                FROM Inmueble
                LIMIT @tamPagina OFFSET @offset";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@tamPagina", tamPagina);
                    comando.Parameters.AddWithValue("@offset", offset);
                    conexion.Open();
                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        Inmueble p = new Inmueble
                        {
                            IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                            Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                            Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
                            Tipo = reader.GetString(nameof(Inmueble.Tipo)),
                            Latitud = reader.GetDouble(nameof(Inmueble.Latitud)),
                            Longitud = reader.GetDouble(nameof(Inmueble.Longitud)),
                            PrecioPorDia = reader.GetDecimal(nameof(Inmueble.PrecioPorDia)),
                            Estado = reader.GetBoolean(nameof(Inmueble.Estado)),
                            IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario))
                        };
                        res.Add(p);
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error al obtener la lista de la tabla inmueble {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }
        return res;
    }

    public Inmueble? ObtenerPorId(int id)
    {
        Inmueble? res = null;

        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"SELECT *
                FROM Inmueble
                WHERE IdInmueble = @IdInmueble";

                using(MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@IdInmueble", id);
                    conexion.Open();
                    var reader = comando.ExecuteReader();
                    res = new Inmueble {
                        IdInmueble = reader.GetInt32(nameof(Inmueble.IdInmueble)),
                        Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                        Cupo = reader.GetInt32(nameof(Inmueble.Cupo)),
                        Tipo = reader.GetString(nameof(Inmueble.Tipo)),
                        Latitud = reader.GetDouble(nameof(Inmueble.Latitud)),
                        Longitud = reader.GetDouble(nameof(Inmueble.Longitud)),
                        PrecioPorDia = reader.GetDecimal(nameof(Inmueble.PrecioPorDia)),
                        Estado = reader.GetBoolean(nameof(Inmueble.Estado)),
                        IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario))
                    };
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al obtener por ID de la tabla Inmueble {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
            
        }
        return res;
    }
}