using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Inmobiliaria.Models;


namespace Inmobiliaria.Models
{
	public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
	{
		public RepositorioInquilino(IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Inquilino i)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"INSERT INTO Inquilino 
					(Nombre, Apellido, Dni, Telefono, Email)
					VALUES (@nombre, @apellido, @dni, @telefono, @email);
					SELECT LAST_INSERT_ID();";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", i.Nombre);
					command.Parameters.AddWithValue("@apellido", i.Apellido);
					command.Parameters.AddWithValue("@dni", i.Dni);
					command.Parameters.AddWithValue("@telefono", i.Telefono);
					command.Parameters.AddWithValue("@email", i.Email);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					i.IdInquilino = res;
					connection.Close();
				}
			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "DELETE FROM Inquilino WHERE IdInquilino = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Inquilino i)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"UPDATE Inquilino 
					SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email
					WHERE IdInquilino = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", i.Nombre);
					command.Parameters.AddWithValue("@apellido", i.Apellido);
					command.Parameters.AddWithValue("@dni", i.Dni);
					command.Parameters.AddWithValue("@telefono", i.Telefono);
					command.Parameters.AddWithValue("@email", i.Email);
					command.Parameters.AddWithValue("@id", i.IdInquilino);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inquilino> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
		{
			IList<Inquilino> res = new List<Inquilino>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email
					FROM Inquilino
					ORDER BY IdInquilino
					LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}
				";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inquilino i = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),//más seguro
							Nombre = reader.GetString(nameof(Inquilino.Nombre)),
							Apellido = reader.GetString(nameof(Inquilino.Apellido)),
							Dni = reader.GetString(nameof(Inquilino.Dni)),
							Telefono = reader.GetString(nameof(Inquilino.Telefono)),
							Email = reader.GetString(nameof(Inquilino.Email)),
						};
						res.Add(i);
					}
					connection.Close();
				}
			}
			return res;
		}

		public int ObtenerCantidad()
		{
			int res = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT COUNT(IdInquilino)
					FROM Inquilino
				";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						res = reader.GetInt32(0);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inquilino? ObtenerPorId(int id)
		{
			Inquilino? i = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email 
					FROM Inquilino
					WHERE IdInquilino=@id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						i = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
						};
					}
					connection.Close();
				}
			}
			return i;
		}

		public Inquilino? ObtenerPorEmail(string email)
		{
			Inquilino? i = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email 
					FROM Inquilino
					WHERE Email=@email";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						i = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
						};
					}
					connection.Close();
				}
			}
			return i;
		}

		public IList<Inquilino> BuscarPorNombre(string nombre)
		{
			List<Inquilino> res = new List<Inquilino>();
			nombre = "%" + nombre + "%";
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email 
					FROM Inquilino
					WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = nombre;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						var i = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
						};
						res.Add(i);
					}
					connection.Close();
				}
			}
			return res;
		}


	public IList<Inquilino> ObtenerTodos()
	{
		IList<Inquilino> res = new List<Inquilino>();
		using (MySqlConnection connection = new MySqlConnection(connectionString))
		{
			string sql = @"
				SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email
				FROM Inquilino
				ORDER BY IdInquilino";
			using (MySqlCommand command = new MySqlCommand(sql, connection))
			{
				command.CommandType = CommandType.Text;
				connection.Open();
				var reader = command.ExecuteReader();
				while (reader.Read())
				{
					Inquilino i = new Inquilino
					{
						IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
						Nombre = reader.GetString(nameof(Inquilino.Nombre)),
						Apellido = reader.GetString(nameof(Inquilino.Apellido)),
						Dni = reader.GetString(nameof(Inquilino.Dni)),
						Telefono = reader.GetString(nameof(Inquilino.Telefono)),
						Email = reader.GetString(nameof(Inquilino.Email)),
					};
					res.Add(i);
				}
				connection.Close();
			}
		}
		return res;
	}

}

}
