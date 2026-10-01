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
                string query = @"INSERT INTO Pago (Concepto, FechaPago, Importe, Estado, IdReserva, IdUsuarioCreador)
                VALUES (@Concepto, @FechaPago, @Importe, @Estado, @IdReserva, @IdUsuarioCreador);
                SELECT LAST_INSERT_ID();";
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@Concepto", p.Concepto);
                    comando.Parameters.AddWithValue("@FechaPago", p.FechaPago);
                    comando.Parameters.AddWithValue("@Importe", p.Importe);
                    comando.Parameters.AddWithValue("@Estado", p.Estado);
                    comando.Parameters.AddWithValue("@IdReserva", p.IdReserva);
                    comando.Parameters.AddWithValue("@IdUsuarioCreador", p.IdUsuarioCreador);
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

public int Baja(int id, int idUsuarioAnulador) 
    {
        int res = -1;
        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            
            string query = @"UPDATE Pago 
                            SET Estado = 'Anulado', IdUsuarioAnulador = @IdAnulador 
                            WHERE IdPago = @IdPago";
            using (MySqlCommand comando = new MySqlCommand(query, conexion))
            {
                comando.Parameters.AddWithValue("@IdAnulador", idUsuarioAnulador);
                comando.Parameters.AddWithValue("@IdPago", id);
                conexion.Open();
                res = comando.ExecuteNonQuery();
            }
        }
        return res;
    }

    public int Baja(int id)
    {
        throw new NotSupportedException("Para anular un pago es obligatorio usar la sobrecarga Baja(int id, int idUsuarioAnulador) para registrar la auditoria.");
    }

    public int Modificacion(Pago p)
    {
        int res = -1;
        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            string query = @"UPDATE Pago 
                            SET Concepto = @Concepto, Estado = @Estado
                            WHERE IdPago = @IdPago";
            using (MySqlCommand comando = new MySqlCommand(query, conexion))
            {
                comando.Parameters.AddWithValue("@Concepto", p.Concepto);
                comando.Parameters.AddWithValue("@Estado", p.Estado);
                comando.Parameters.AddWithValue("@IdPago", p.IdPago);
                conexion.Open();
                res = comando.ExecuteNonQuery();
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
                string query = @"SELECT p.*, 
                                r.IdInquilino, r.IdInmueble, 
                                i.Nombre AS InqNombre, i.Apellido AS InqApellido,
                                m.Direccion AS InmDireccion
                FROM Pago p
                INNER JOIN Reserva r ON p.IdReserva = r.IdReserva
                INNER JOIN Inquilino i ON r.IdInquilino = i.IdInquilino
                INNER JOIN Inmueble m ON r.IdInmueble = m.IdInmueble
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
                            Estado = reader.GetString(nameof(p.Estado)),
                            IdReserva = reader.GetInt32(nameof(p.IdReserva)),
                            IdUsuarioCreador = reader.GetInt32(nameof(p.IdUsuarioCreador)),
                            IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal(nameof(p.IdUsuarioAnulador))) ? null : reader.GetInt32(nameof(p.IdUsuarioAnulador)),
                            ReservaAsociada = new Reserva 
                            {
                                IdReserva = reader.GetInt32(nameof(p.IdReserva)),
                                InquilinoAsociado = new Inquilino 
                                {
                                    Nombre = reader.GetString("InqNombre"),
                                    Apellido = reader.GetString("InqApellido")
                                },
                                InmuebleAsociado = new Inmueble 
                                {
                                    Direccion = reader.GetString("InmDireccion")
                                }
                            }
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
                            Estado = reader.GetString(nameof(res.Estado)),
                            IdReserva = reader.GetInt32(nameof(res.IdReserva)),
                            IdUsuarioCreador = reader.GetInt32(nameof(res.IdUsuarioCreador)),
                            IdUsuarioAnulador = reader.IsDBNull(nameof(res.IdUsuarioAnulador)) ? null : reader.GetInt32(nameof(res.IdUsuarioAnulador))
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

    public IList<Pago> ObtenerPorReserva(int idReserva)
    {
        var res = new List<Pago>();
        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            string query = @"SELECT p.*, 
                            r.IdInquilino, r.IdInmueble, 
                            i.Nombre AS InqNombre, i.Apellido AS InqApellido,
                            m.Direccion AS InmDireccion
            FROM Pago p
            INNER JOIN Reserva r ON p.IdReserva = r.IdReserva
            INNER JOIN Inquilino i ON r.IdInquilino = i.IdInquilino
            INNER JOIN Inmueble m ON r.IdInmueble = m.IdInmueble
            WHERE p.IdReserva = @idReserva";
            using (MySqlCommand comando = new MySqlCommand(query, conexion))
            {
                comando.Parameters.AddWithValue("@idReserva", idReserva);
                conexion.Open();
                var reader = comando.ExecuteReader();
                while (reader.Read())
                {
                    res.Add(new Pago {
                        IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                        Concepto = reader.GetString(nameof(Pago.Concepto)),
                        FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                        Importe = reader.GetDecimal(nameof(Pago.Importe)),
                        Estado = reader.GetString(nameof(Pago.Estado)),
                        IdReserva = reader.GetInt32(nameof(Pago.IdReserva)),
                        IdUsuarioCreador = reader.GetInt32(nameof(Pago.IdUsuarioCreador)),
                        IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal(nameof(Pago.IdUsuarioAnulador))) ? null : reader.GetInt32(nameof(Pago.IdUsuarioAnulador)),
                        ReservaAsociada = new Reserva 
                        {
                            IdReserva = reader.GetInt32(nameof(Pago.IdReserva)),
                            InquilinoAsociado = new Inquilino 
                            {
                                Nombre = reader.GetString("InqNombre"),
                                Apellido = reader.GetString("InqApellido")
                            },
                            InmuebleAsociado = new Inmueble 
                            {
                                Direccion = reader.GetString("InmDireccion")
                            }
                        }
                    });
                }
            }
        }
        return res;
    }
}