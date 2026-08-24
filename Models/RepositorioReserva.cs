using System;
using System.Data;
using MySqlConnector;

namespace Inmobiliaria.Models;

public class RepositorioReserva : RepositorioBase, IRepositorio<Reserva>

{
    public RepositorioReserva(IConfiguration configuration) : base(configuration)
    {
        
    }

    public int Alta(Reserva p)
    {
        int res = -1;

        using(MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try {
            string query = @"INSERT INTO Reserva (fechaInicio, fechaFin, montoDiario, fechaEfectivaTerminacion, multa, idInquilino, idInmueble, creadoPorUsuarioId, terminadoPorUsuarioId)
            VALUES (@fechaInicio, @fechaFin, @montoDiario, @fechaEfectivaTerminacion, @multa, @idInquilino, @idInmueble, @creadoPorUsuarioId, @terminadoPorUsuarioId)
            SELECT LAST_INSERT_ID()";

            using(MySqlCommand comando = new MySqlCommand(query, conexion))
            {
                comando.CommandType = CommandType.Text;
                comando.Parameters.AddWithValue("@fechaInicio", p.FechaInicio);
                comando.Parameters.AddWithValue("@fechaFin", p.FechaFin);
                comando.Parameters.AddWithValue("@montoDiario", p.MontoDiario);
                comando.Parameters.AddWithValue("@fechaEfectivaTerminacion", p.FechaEfectivaTerminacion);
                comando.Parameters.AddWithValue("@multa", p.Multa);
                comando.Parameters.AddWithValue("@idInquilino", p.IdInquilino);
                comando.Parameters.AddWithValue("@idInmueble", p.IdInmueble);
                comando.Parameters.AddWithValue("@creadoPorUsuarioId", p.CreadoPorUsuarioId);
                comando.Parameters.AddWithValue("@terminadoPorUsuarioId", p.TerminadoPorUsuarioId);
                conexion.Open();
                res = Convert.ToInt32(comando.ExecuteScalar());
                p.IdReserva = res;
            }
            }catch(Exception ex)
            {
                Console.WriteLine($"Error al insertar reserva: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }

            return res;
        }
    }

    public int Baja(int id)
    {
        int res = -1;

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = "DELETE FROM Reserva Where idReserva = @idReserva";
                
                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@idReserva", id);
                    conexion.Open();
                    res = comando.ExecuteNonQuery();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error al eliminar reserva: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }

        }
        return res;
    }

    public int Modificacion(Reserva p)
    {
        int res = -1;

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"UPDATE Reserva
                SET fechaInicio = @fechaInicio, fechaFin = @fechaFin, montoDiario = @montoDiario, fechaEfectivaTerminacion = @fechaEfectivaTerminacion, multa = @multa, idInquilino = @idInquilino, idInmueble = @idInmueble, creadoPorUsuarioId = @creadoPorUsuarioId, terminadoPorUsuarioId = @terminadoPorUsuarioId
                WHERE idReserva = @idReserva";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@idReserva", p.IdReserva);
                    comando.Parameters.AddWithValue("@fechaInicio", p.FechaInicio);
                    comando.Parameters.AddWithValue("@fechaFin", p.FechaFin);
                    comando.Parameters.AddWithValue("@montoDiario", p.MontoDiario);
                    comando.Parameters.AddWithValue("@fechaEfectivaTerminacion", p.FechaEfectivaTerminacion);
                    comando.Parameters.AddWithValue("@multa", p.Multa);
                    comando.Parameters.AddWithValue("@idInquilino", p.IdInquilino);
                    comando.Parameters.AddWithValue("@idInmueble", p.IdInmueble);
                    comando.Parameters.AddWithValue("@creadoPorUsuarioId", p.CreadoPorUsuarioId);
                    comando.Parameters.AddWithValue("@terminadoPorUsuarioId", p.TerminadoPorUsuarioId);
                    conexion.Open();
                    res = comando.ExecuteNonQuery();
                    
                }

            }catch(Exception ex)
            {
                Console.WriteLine($"Error al modificar reserva: {ex.Message}");
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

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {   
            try
            {
                string query = "SELECT COUNT(idReserva) FROM Reserva";
                
                using(MySqlCommand comando = new MySqlCommand(query, conexion))
                {
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
                Console.WriteLine($"Error al obtener cantidad de reservas: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }
        }
        return res;
    }

    public IList<Reserva> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
    {
        IList<Reserva> res = new List<Reserva>();

        int offset = (paginaNro - 1) * tamPagina;

        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {   
                string query = @"SELECT idReserva, fechaInicio, fechaFin, montoDiario, fechaEfectivaTerminacion, multa, idInquilino, idInmueble, creadoPorUsuarioId, terminadoPorUsuarioId
                FROM Reserva LIMIT @tamPagina OFFSET @offset";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@tamPagina", tamPagina);
                    comando.Parameters.AddWithValue("@offset", offset);
                    conexion.Open();
                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        Reserva p = new Reserva {
                            IdReserva = reader.GetInt32(nameof(Reserva.IdReserva)),
                            FechaInicio = reader.GetDateTime(nameof(Reserva.FechaInicio)),
                            FechaFin = reader.GetDateTime(nameof(Reserva.FechaFin)),
                            MontoDiario = reader.GetDecimal(nameof(Reserva.MontoDiario)),
                            FechaEfectivaTerminacion = reader.IsDBNull(nameof(Reserva.FechaEfectivaTerminacion)) ? null : reader.GetDateTime(nameof(Reserva.FechaEfectivaTerminacion)),
                            Multa = reader.IsDBNull(nameof(Reserva.Multa)) ? null : reader.GetDecimal(nameof(Reserva.Multa)),
                            IdInquilino = reader.GetInt32(nameof(Reserva.IdInquilino)),
                            IdInmueble = reader.GetInt32(nameof(Reserva.IdInmueble)),
                            CreadoPorUsuarioId = reader.GetInt32(nameof(Reserva.CreadoPorUsuarioId)),
                            TerminadoPorUsuarioId = reader.IsDBNull(nameof(Reserva.TerminadoPorUsuarioId)) ? null : reader.GetInt32(nameof(Reserva.TerminadoPorUsuarioId))
                        };
                        res.Add(p);
                    }
                }
            }catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener lista de reservas: {ex.Message}");
            }
            finally
            {
                conexion.Close();
            }

        }
        return res;
    }

    public Reserva? ObtenerPorId(int id)
    {
        Reserva? res = null;

    using (MySqlConnection conexion = new MySqlConnection(connectionString))
    {
        try
        {
            string query = @"SELECT idReserva, fechaInicio, fechaFin, montoDiario, fechaEfectivaTerminacion, multa, idInquilino, idInmueble, creadoPorUsuarioId, terminadoPorUsuarioId 
            FROM Reserva 
            WHERE idReserva = @idReserva";

            using (MySqlCommand comando = new MySqlCommand(query, conexion))
            {
                comando.Parameters.AddWithValue("@idReserva", id);
                conexion.Open();
                var reader = comando.ExecuteReader();
                if (reader.Read())
                {
                    res = new Reserva
                    {
                        IdReserva = reader.GetInt32(nameof(Reserva.IdReserva)),
                        FechaInicio = reader.GetDateTime(nameof(Reserva.FechaInicio)),
                        FechaFin = reader.GetDateTime(nameof(Reserva.FechaFin)),
                        MontoDiario = reader.GetDecimal(nameof(Reserva.MontoDiario)),
                        FechaEfectivaTerminacion = reader.IsDBNull(nameof(Reserva.FechaEfectivaTerminacion)) ? null : reader.GetDateTime(nameof(Reserva.FechaEfectivaTerminacion)),
                        Multa = reader.IsDBNull(nameof(Reserva.Multa)) ? null : reader.GetDecimal(nameof(Reserva.Multa)),
                        IdInquilino = reader.GetInt32(nameof(Reserva.IdInquilino)),
                        IdInmueble = reader.GetInt32(nameof(Reserva.IdInmueble)),
                        CreadoPorUsuarioId = reader.GetInt32(nameof(Reserva.CreadoPorUsuarioId)),
                        TerminadoPorUsuarioId = reader.IsDBNull(nameof(Reserva.TerminadoPorUsuarioId)) ? null : reader.GetInt32(nameof(Reserva.TerminadoPorUsuarioId))
                    };
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener reserva por ID: {ex.Message}");
        }
        finally
        {
            conexion.Close();
        }
    }

    return res;
    }

}