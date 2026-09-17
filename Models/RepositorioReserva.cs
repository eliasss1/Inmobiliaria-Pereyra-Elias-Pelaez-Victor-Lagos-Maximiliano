using System;
using System.Data;
using MySqlConnector;

namespace Inmobiliaria.Models;

public class RepositorioReserva : RepositorioBase, IRepositorioReserva

{
    public RepositorioReserva(IConfiguration configuration) : base(configuration)
    {
        
    }

    public int Alta(Reserva p)
{
    int res = -1;

    using(MySqlConnection conexion = new MySqlConnection(connectionString))
    {
        try 
        {
            conexion.Open();
            
            string queryInsert = @"INSERT INTO Reserva (FechaDesde, FechaHasta, MontoPorDia, FechaEfectivaTerminacion, IdInquilino, IdInmueble, IdUsuarioCreador, IdUsuarioTerminador)
            VALUES (@FechaDesde, @FechaHasta, @MontoPorDia, @FechaEfectivaTerminacion, @IdInquilino, @IdInmueble, @IdUsuarioCreador, @IdUsuarioTerminador);
            SELECT LAST_INSERT_ID();";

            using(MySqlCommand comando = new MySqlCommand(queryInsert, conexion))
            {
                comando.Parameters.AddWithValue("@FechaDesde", p.FechaDesde);
                comando.Parameters.AddWithValue("@FechaHasta", p.FechaHasta);
                comando.Parameters.AddWithValue("@MontoPorDia", p.MontoPorDia);
                comando.Parameters.AddWithValue("@FechaEfectivaTerminacion", p.FechaEfectivaTerminacion);
                comando.Parameters.AddWithValue("@IdInquilino", p.IdInquilino);
                comando.Parameters.AddWithValue("@IdInmueble", p.IdInmueble);
                comando.Parameters.AddWithValue("@IdUsuarioCreador", p.IdUsuarioCreador);
                comando.Parameters.AddWithValue("@IdUsuarioTerminador", p.IdUsuarioTerminador);
                
                res = Convert.ToInt32(comando.ExecuteScalar());
                p.IdReserva = res;
            }

            if (res > 0)
            {
                string queryUpdate = "UPDATE Inmueble SET Estado = FALSE WHERE IdInmueble = @IdInmueble";
                
                using (MySqlCommand cmdUpdate = new MySqlCommand(queryUpdate, conexion))
                {
                    cmdUpdate.Parameters.AddWithValue("@IdInmueble", p.IdInmueble);
                    cmdUpdate.ExecuteNonQuery();
                }
            }
        }
        catch(Exception ex)
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
            conexion.Open();

            int idInmueble = 0;
            string querySelect = "SELECT IdInmueble FROM Reserva WHERE IdReserva = @idReserva";
            
            using (MySqlCommand cmdSelect = new MySqlCommand(querySelect, conexion))
            {
                cmdSelect.Parameters.AddWithValue("@idReserva", id);
                var result = cmdSelect.ExecuteScalar();
                if (result != null)
                {
                    idInmueble = Convert.ToInt32(result);
                }
            }

            if (idInmueble > 0)
            {
                string queryUpdate = "UPDATE Inmueble SET Estado = TRUE WHERE IdInmueble = @IdInmueble";
                
                using (MySqlCommand cmdUpdate = new MySqlCommand(queryUpdate, conexion))
                {
                    cmdUpdate.Parameters.AddWithValue("@IdInmueble", idInmueble);
                    cmdUpdate.ExecuteNonQuery();
                }

                string queryDelete = "DELETE FROM Reserva WHERE IdReserva = @idReserva";
                
                using (MySqlCommand cmdDelete = new MySqlCommand(queryDelete, conexion))
                {
                    cmdDelete.Parameters.AddWithValue("@idReserva", id);
                    res = cmdDelete.ExecuteNonQuery();
                }
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
                SET FechaDesde = @FechaDesde, FechaHasta = @FechaHasta, MontoPorDia = @MontoPorDia, FechaEfectivaTerminacion = @FechaEfectivaTerminacion, IdInquilino = @IdInquilino, IdInmueble = @IdInmueble, IdUsuarioCreador = @IdUsuarioCreador, IdUsuarioTerminador = @IdUsuarioTerminador
                WHERE IdReserva = @IdReserva";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    comando.Parameters.AddWithValue("@IdReserva", p.IdReserva);
                    comando.Parameters.AddWithValue("@FechaDesde", p.FechaDesde);
                    comando.Parameters.AddWithValue("@FechaHasta", p.FechaHasta);
                    comando.Parameters.AddWithValue("@MontoPorDia", p.MontoPorDia);
                    comando.Parameters.AddWithValue("@FechaEfectivaTerminacion", p.FechaEfectivaTerminacion);
                    comando.Parameters.AddWithValue("@IdInquilino", p.IdInquilino);
                    comando.Parameters.AddWithValue("@IdInmueble", p.IdInmueble);
                    comando.Parameters.AddWithValue("@IdUsuarioCreador", p.IdUsuarioCreador);
                    comando.Parameters.AddWithValue("@IdUsuarioTerminador", p.IdUsuarioTerminador);
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

    public IList<Reserva> ObtenerTodos()
    {
        IList<Reserva> res = new List<Reserva>();
        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
            try
            {
                string query = @"SELECT r.IdReserva, r.FechaDesde, r.FechaHasta, r.MontoPorDia, 
                                r.FechaEfectivaTerminacion, r.IdInquilino, r.IdInmueble, 
                                r.IdUsuarioCreador, r.IdUsuarioTerminador,
                                i.Nombre AS InqNombre, i.Apellido AS InqApellido, i.Dni AS InqDni, m.IdInmueble AS InmId, m.Direccion AS InmDireccion
                                FROM Reserva r
                                INNER JOIN Inquilino i ON r.IdInquilino = i.IdInquilino
                                INNER JOIN Inmueble m ON r.idInmueble = m.IdInmueble";

                using (MySqlCommand comando = new MySqlCommand(query, conexion))
                {
                    comando.CommandType = CommandType.Text;
                    conexion.Open();
                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        Reserva p = new Reserva {
                            IdReserva = reader.GetInt32("IdReserva"),
                            FechaDesde = reader.GetDateTime("FechaDesde"),
                            FechaHasta = reader.GetDateTime("FechaHasta"),
                            MontoPorDia = reader.GetDecimal("MontoPorDia"),
                            FechaEfectivaTerminacion = reader.IsDBNull(reader.GetOrdinal("FechaEfectivaTerminacion")) ? null : reader.GetDateTime("FechaEfectivaTerminacion"),
                            IdInquilino = reader.GetInt32("IdInquilino"),
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            IdUsuarioCreador = reader.GetInt32("IdUsuarioCreador"),
                            IdUsuarioTerminador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioTerminador")) ? null : reader.GetInt32("IdUsuarioTerminador"),
                            InquilinoAsociado = new Inquilino {
                                Nombre = reader.GetString("InqNombre"),
                                Apellido = reader.GetString("InqApellido"),
                                Dni = reader.GetString("InqDni")
                            },
                            InmuebleAsociado = new Inmueble {
                                IdInmueble = reader.GetInt32("InmId"),
                                Direccion = reader.GetString("InmDireccion")
                            }
                        };
                        res.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener todas las reservas: {ex.Message}");
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
                string query = @"SELECT r.IdReserva, r.FechaDesde, r.FechaHasta, r.MontoPorDia, 
                                r.FechaEfectivaTerminacion, r.IdInquilino, r.IdInmueble, 
                                r.IdUsuarioCreador, r.IdUsuarioTerminador,
                                i.Nombre AS InqNombre, i.Apellido AS InqApellido, i.Dni AS InqDni, m.IdInmueble AS InmId, m.Direccion AS InmDireccion
                                FROM Reserva r
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
                        Reserva p = new Reserva {
                            IdReserva = reader.GetInt32("IdReserva"),
                            FechaDesde = reader.GetDateTime("FechaDesde"),
                            FechaHasta = reader.GetDateTime("FechaHasta"),
                            MontoPorDia = reader.GetDecimal("MontoPorDia"),
                            FechaEfectivaTerminacion = reader.IsDBNull(reader.GetOrdinal("FechaEfectivaTerminacion")) ? null : reader.GetDateTime("FechaEfectivaTerminacion"),
                            IdInquilino = reader.GetInt32("IdInquilino"),
                            IdInmueble = reader.GetInt32("IdInmueble"),
                            IdUsuarioCreador = reader.GetInt32("IdUsuarioCreador"),
                            IdUsuarioTerminador = reader.IsDBNull(reader.GetOrdinal("IdUsuarioTerminador")) ? null : reader.GetInt32("IdUsuarioTerminador"),
                            InquilinoAsociado = new Inquilino {
                                Nombre = reader.GetString("InqNombre"),
                                Apellido = reader.GetString("InqApellido"),
                                Dni = reader.GetString("InqDni")
                            },
                            InmuebleAsociado = new Inmueble {
                                IdInmueble = reader.GetInt32("InmId"),
                                Direccion = reader.GetString("InmDireccion")
                            }
                        };
                        res.Add(p);
                    }
                }
            }
            catch (Exception ex)
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
            string query = @"SELECT IdReserva, FechaDesde, FechaHasta, MontoPorDia, FechaEfectivaTerminacion, IdInquilino, IdInmueble, IdUsuarioCreador, IdUsuarioTerminador 
            FROM Reserva 
            WHERE IdReserva = @IdReserva";

            using (MySqlCommand comando = new MySqlCommand(query, conexion))
            {
                comando.Parameters.AddWithValue("@IdReserva", id);
                conexion.Open();
                var reader = comando.ExecuteReader();
                if (reader.Read())
                {
                    res = new Reserva
                    {
                        IdReserva = reader.GetInt32(nameof(Reserva.IdReserva)),
                        FechaDesde = reader.GetDateTime(nameof(Reserva.FechaDesde)),
                        FechaHasta = reader.GetDateTime(nameof(Reserva.FechaHasta)),
                        MontoPorDia = reader.GetDecimal(nameof(Reserva.MontoPorDia)),
                        FechaEfectivaTerminacion = reader.IsDBNull(nameof(Reserva.FechaEfectivaTerminacion)) ? null : reader.GetDateTime(nameof(Reserva.FechaEfectivaTerminacion)),
                        IdInquilino = reader.GetInt32(nameof(Reserva.IdInquilino)),
                        IdInmueble = reader.GetInt32(nameof(Reserva.IdInmueble)),
                        IdUsuarioCreador = reader.GetInt32(nameof(Reserva.IdUsuarioCreador)),
                        IdUsuarioTerminador = reader.IsDBNull(nameof(Reserva.IdUsuarioTerminador)) ? null : reader.GetInt32(nameof(Reserva.IdUsuarioTerminador))
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
    public bool ExisteSolapamiento(Reserva reserva) 
    {
        bool estaOcupado = false; 
        using (MySqlConnection conexion = new MySqlConnection(connectionString))
        {
        string sql = 
            @"SELECT COUNT(\*) 
            FROM Reserva 
            WHERE IdInmueble = @IdInmueble 
            AND IdReserva != @IdReserva 
            AND (FechaDesde <= @FechaHasta AND FechaHasta >= @FechaDesde)"; 
        using (var command = new MySqlCommand(sql, conexion))
        { 
            command.Parameters.AddWithValue("@IdInmueble", reserva.IdInmueble); 
            command.Parameters.AddWithValue("@IdReserva", reserva.IdReserva); 
            command.Parameters.AddWithValue("@FechaDesde", reserva.FechaDesde); 
            command.Parameters.AddWithValue("@FechaHasta", reserva.FechaHasta); 
            conexion.Open(); 
            int cantidad = Convert.ToInt32(command.ExecuteScalar()); 
            conexion.Close(); 
            estaOcupado = cantidad > 0; 
            } 
        } 
        return estaOcupado; 
    }
    
    public bool RegistrarTerminacionAnticipadaConPago(int idReserva, DateTime fechaTerminacion, decimal montoMulta, int idUsuario) {
            bool exito = false; 
            string connectionString = GetConnectionString(); 
            using(MySqlConnection conexion = new MySqlConnection(connectionString)) 
            { 
            conexion.Open();
                using (var transaction = conexion.BeginTransaction()) 
                { 
                    try 
                    { 
                    string sqlPago = @"INSERT INTO pago (IdReserva, Concepto, FechaPago, Importe) 
                    VALUES (@IdReserva, @Concepto, @FechaPago, @Importe);"; 
                    using (var cmdPago = new MySqlCommand(sqlPago, conexion, transaction)) 
                    { 
                        cmdPago.Parameters.AddWithValue("@IdReserva", idReserva); 
                        cmdPago.Parameters.AddWithValue("@Concepto", "Multa por terminación anticipada"); 
                        cmdPago.Parameters.AddWithValue("@FechaPago", DateTime.Now); 
                        cmdPago.Parameters.AddWithValue("@Importe", montoMulta); 
                        cmdPago.ExecuteNonQuery(); } // (Estado = 3: Terminada Anticipada) 
                        string sqlReserva = @"UPDATE reserva 
                        SET FechaRealTerminacion = 
                        @FechaTerminacion, 
                            Estado = 3, 
                            IdUsuarioTerminacion = @IdUsuario 
                            WHERE IdReserva = @IdReserva;"; 
                        using (var cmdReserva = new MySqlCommand(sqlReserva, conexion, transaction)) 
                        {
                            cmdReserva.Parameters.AddWithValue("@FechaTerminacion", fechaTerminacion); 
                            cmdReserva.Parameters.AddWithValue("@IdUsuario", idUsuario); 
                            cmdReserva.Parameters.AddWithValue("@IdReserva", idReserva); 
                            cmdReserva.ExecuteNonQuery(); 
                        }
                    transaction.Commit(); 
                    exito = true; 
                    }
                    catch (Exception) 
                    {  
                    transaction.Rollback(); throw; 
                    } 
                    finally 
                    { conexion.Close(); 
                    } 
                } 
            } 
            return exito; 
    } 

    public IList<Reserva> Buscar(string busqueda)
{
    var lista = new List<Reserva>();
    using (MySqlConnection conexion = new MySqlConnection(connectionString))
    {
        string sql = @"SELECT r.IdReserva, r.IdInmueble, r.IdInquilino, r.FechaDesde, r.FechaHasta, r.MontoPorDia, r.Estado,
                            inm.Direccion AS InmuebleDireccion, 
                            inq.Nombre AS InquilinoNombre, 
                            inq.Apellido AS InquilinoApellido 
                    FROM Reserva r
                    INNER JOIN Inmueble inm ON r.IdInmueble = inm.IdInmueble
                    INNER JOIN Inquilino inq ON r.IdInquilino = inq.IdInquilino
                    WHERE inq.Apellido LIKE @busqueda OR inm.Direccion LIKE @busqueda";

        using (MySqlCommand comando = new MySqlCommand(sql, conexion))
        {
            comando.Parameters.AddWithValue("@busqueda", "%" + busqueda + "%");
            conexion.Open();
            using (MySqlDataReader reader = comando.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new Reserva
                    {
                        IdReserva = reader.GetInt32(nameof(Reserva.IdReserva)),
                        IdInmueble = reader.GetInt32(nameof(Reserva.IdInmueble)),
                        IdInquilino = reader.GetInt32(nameof(Reserva.IdInquilino)),
                        FechaDesde = reader.GetDateTime(nameof(Reserva.FechaDesde)),
                        FechaHasta = reader.GetDateTime(nameof(Reserva.FechaHasta)),
                        MontoPorDia = reader.GetDecimal(nameof(Reserva.MontoPorDia)),
                        Estado = reader.GetInt32(nameof(Reserva.Estado)),
                        
                        InmuebleAsociado = new Inmueble 
                        { 
                            Direccion = reader.GetString("InmuebleDireccion") 
                        },
                        InquilinoAsociado = new Inquilino 
                        { 
                            Nombre = reader.GetString("InquilinoNombre"), 
                            Apellido = reader.GetString("InquilinoApellido") 
                        }
                    });
                }
            }
        }
    }
    return lista;
}
}


