using System.Data;
using MySqlConnector;

namespace Inmobiliaria.Models;

public class RepositorioPago : RepositorioBase, IRepositorioPago
{
    public RepositorioPago(IConfiguration configuration) : base(configuration)
    {
        
    }

    public int Alta(Pago p)
    {
        int res = -1;

        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"INSERT INTO Pago (Concepto, FechaPago, Importe, Estado, IdReserva, ReservaAsociada, CreadoPorUsuarioId, AnuladoPorUsuarioId)
                VALUES (@Concepto, @FechaPago, @Importe, @Estado, @IdReserva, @ReservaAsociada, @CreadoPorUsuarioId, @AnuladoPorUsuarioId)
                SELECT LAST_INSERT_ID()";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@Concepto", p.Concepto);
                    comando.Parameters.AddWithValue("@FechaPago", p.FechaPago);
                    comando.Parameters.AddWithValue("@Importe", p.Importe);
                    comando.Parameters.AddWithValue("@Estado", p.Estado);
                    comando.Parameters.AddWithValue("@IdReserva", p.IdReserva);
                    comando.Parameters.AddWithValue("@ReservaAsociada", p.ReservaAsociada);
                    comando.Parameters.AddWithValue("@CreadoPorUsuarioId", p.CreadoPorUsuarioId);
                    comando.Parameters.AddWithValue("@AnuladoPorUsuarioId", p.AnuladoPorUsuarioId);
                    conexion.Open();
                    res = Convert.ToInt32(comando.ExecuteScalar());
                    p.IdPago = res;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al crear un pago: {ex.Message}");
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
        
        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"DELETE FROM Pago WHERE IdPago = @IdPago";
                
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@IdPago", id);
                    conexion.Open();
                    res = comando.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al eliminar un pago: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }
        
        return res;
    }

    public int Modificacion(Pago p)
    {
        int res = -1;
        
        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"UPDATE Pago
                SET Concepto = @Concepto, FechaPago = @FechaPago, Importe = @Importe, Estado = @Estado, IdReserva = @IdReserva, ReservaAsociada = @ReservaAsociada, CreadoPorUsuarioId = @CreadoPorUsuarioId, AnuladoPorUsuarioId = @AnuladoPorUsuarioId
                WHERE IdPago = @IdPago";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@Concepto", p.Concepto);
                    comando.Parameters.AddWithValue("@FechaPago", p.FechaPago);
                    comando.Parameters.AddWithValue("@Importe", p.Importe);
                    comando.Parameters.AddWithValue("@Estado", p.Estado);
                    comando.Parameters.AddWithValue("@IdReserva", p.IdReserva);
                    comando.Parameters.AddWithValue("@ReservaAsociada", p.ReservaAsociada);
                    comando.Parameters.AddWithValue("@CreadoPorUsuarioId", p.CreadoPorUsuarioId);
                    comando.Parameters.AddWithValue("@AnuladoPorUsuarioId", p.AnuladoPorUsuarioId);
                    comando.Parameters.AddWithValue("@IdPago", p.IdPago);
                    conexion.Open();
                    res = comando.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al modificar la tabla pago: {ex.Message}");
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
                string query = @"SELECT COUNT(IdPago) 
                FROM Pago";

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
                Console.WriteLine($"Error al obtener la cantidad de la tabla pago: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }
        
        return res;
    }

    public IList<Pago> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
    {
        IList<Pago> res = new List<Pago>();
        
        int offset = (paginaNro - 1) * tamPagina;

        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"SELECT * 
                FROM Pago
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
                        Pago p = new Pago
                        {
                            IdPago = reader.GetInt32(nameof(p.IdPago)),
                            Concepto = reader.GetString(nameof(p.Concepto)),
                            FechaPago = reader.GetDateTime(nameof(p.FechaPago)),
                            Importe = reader.GetDecimal(nameof(p.Importe)),
                            Estado = reader.GetBoolean(nameof(p.Estado)),
                            IdReserva = reader.GetInt32(nameof(p.IdReserva)),
                            CreadoPorUsuarioId = reader.GetInt32(nameof(p.CreadoPorUsuarioId)),
                            AnuladoPorUsuarioId = reader.IsDBNull(nameof(p.AnuladoPorUsuarioId)) ? null : reader.GetInt32(nameof(p.AnuladoPorUsuarioId))
                        };
                        res.Add(p); 
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }
        
        return res;
    }

    public Pago? ObtenerPorId(int id)
    {
        Pago? res = null;
        
        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"SELECT * FROM Pago WHERE IdPago = @id";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@id", id);
                    conexion.Open();
                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        res = new Pago
                        {
                            IdPago = reader.GetInt32(nameof(res.IdPago)),
                            Concepto = reader.GetString(nameof(res.Concepto)),
                            FechaPago = reader.GetDateTime(nameof(res.FechaPago)),
                            Importe = reader.GetDecimal(nameof(res.Importe)),
                            Estado = reader.GetBoolean(nameof(res.Estado)),
                            IdReserva = reader.GetInt32(nameof(res.IdReserva)),
                            CreadoPorUsuarioId = reader.GetInt32(nameof(res.CreadoPorUsuarioId)),
                            AnuladoPorUsuarioId = reader.IsDBNull(nameof(res.AnuladoPorUsuarioId)) ? null : reader.GetInt32(nameof(res.AnuladoPorUsuarioId))
                        };
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }
        
        return res;
    }
}