using MySqlConnector;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Models;

public class RepositorioInmuebleImagen
{
    private readonly string connectionString;

    public RepositorioInmuebleImagen(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        CrearTablaSiNoExiste();
    }

    private void CrearTablaSiNoExiste()
    {
        using var conexion = new MySqlConnection(connectionString);
        conexion.Open();
        var sql = @"
            CREATE TABLE IF NOT EXISTS InmuebleImagen (
                IdImagen INT AUTO_INCREMENT PRIMARY KEY,
                IdInmueble INT NOT NULL,
                Url VARCHAR(255) NOT NULL,
                FOREIGN KEY (IdInmueble) REFERENCES Inmueble(IdInmueble) ON DELETE CASCADE
            );";
        using var cmd = new MySqlCommand(sql, conexion);
        cmd.ExecuteNonQuery();
    }

    public void Alta(InmuebleImagen imagen)
    {
        using var conexion = new MySqlConnection(connectionString);
        conexion.Open();
        var sql = "INSERT INTO InmuebleImagen (IdInmueble, Url) VALUES (@IdInmueble, @Url);";
        using var cmd = new MySqlCommand(sql, conexion);
        cmd.Parameters.AddWithValue("@IdInmueble", imagen.IdInmueble);
        cmd.Parameters.AddWithValue("@Url", imagen.Url);
        cmd.ExecuteNonQuery();
    }

    public IList<InmuebleImagen> ObtenerPorInmueble(int idInmueble)
    {
        var lista = new List<InmuebleImagen>();
        using var conexion = new MySqlConnection(connectionString);
        conexion.Open();
        var sql = "SELECT * FROM InmuebleImagen WHERE IdInmueble = @Id";
        using var cmd = new MySqlCommand(sql, conexion);
        cmd.Parameters.AddWithValue("@Id", idInmueble);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new InmuebleImagen
            {
                IdImagen = reader.GetInt32("IdImagen"),
                IdInmueble = reader.GetInt32("IdInmueble"),
                Url = reader.GetString("Url")
            });
        }
        return lista;
    }

    public void Eliminar(int idImagen)
    {
        using var conexion = new MySqlConnection(connectionString);
        conexion.Open();
        var sql = "DELETE FROM InmuebleImagen WHERE IdImagen = @Id";
        using var cmd = new MySqlCommand(sql, conexion);
        cmd.Parameters.AddWithValue("@Id", idImagen);
        cmd.ExecuteNonQuery();
    }
    
    public InmuebleImagen? ObtenerPorId(int idImagen)
    {
        using var conexion = new MySqlConnection(connectionString);
        conexion.Open();
        var sql = "SELECT * FROM InmuebleImagen WHERE IdImagen = @Id";
        using var cmd = new MySqlCommand(sql, conexion);
        cmd.Parameters.AddWithValue("@Id", idImagen);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new InmuebleImagen
            {
                IdImagen = reader.GetInt32("IdImagen"),
                IdInmueble = reader.GetInt32("IdInmueble"),
                Url = reader.GetString("Url")
            };
        }
        return null;
    }
}
