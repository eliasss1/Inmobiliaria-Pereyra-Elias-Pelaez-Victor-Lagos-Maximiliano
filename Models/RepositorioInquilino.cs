using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria.Models;

namespace Inmobiliaria.Models
{
	public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
	{
		public RepositorioInquilino(IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Inquilino p)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = @"INSERT INTO Inquilinos 
					(Nombre, Apellido, Dni, Telefono, Email, Clave)
					VALUES (@nombre, @apellido, @dni, @telefono, @email, @clave);
					SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@clave", p.Clave);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					p.IdPropietario = res;
					connection.Close();
				}
			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "DELETE FROM Inquilinos WHERE IdInquilino = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
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
		public int Modificacion(Inquilino p)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = @"UPDATE Inquilinos 
					SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email, Clave=@clave
					WHERE IdPropietario = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", p.Nombre);
					command.Parameters.AddWithValue("@apellido", p.Apellido);
					command.Parameters.AddWithValue("@dni", p.Dni);
					command.Parameters.AddWithValue("@telefono", p.Telefono);
					command.Parameters.AddWithValue("@email", p.Email);
					command.Parameters.AddWithValue("@clave", p.Clave);
					command.Parameters.AddWithValue("@id", p.IdPropietario);
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
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = @$"
					SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Clave
					FROM Inquilinos
					ORDER BY IdInquilino
					OFFSET {(paginaNro - 1) * tamPagina} ROW
					FETCH NEXT {tamPagina} ROWS ONLY
				";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inquilino p = new Inquilino
						{
							IdPropietario = reader.GetInt32(nameof(Inquilino.IdPropietario)),//más seguro
							Nombre = reader.GetString(nameof(Inquilino.Nombre)),
							Apellido = reader.GetString(nameof(Inquilino.Apellido)),
							Dni = reader.GetString(nameof(Inquilino.Dni)),
							Telefono = reader.GetString(nameof(Inquilino.Telefono)),
							Email = reader.GetString(nameof(Inquilino.Email)),
							Clave = reader.GetString(nameof(Inquilino.Clave)),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

		public int ObtenerCantidad()
		{
			int res = 0;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = @$"
					SELECT COUNT(IdInquilino)
					FROM Inquilinos
				";
				using (SqlCommand command = new SqlCommand(sql, connection))
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
			Inquilino? p = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Clave 
					FROM Inquilinos
					WHERE IdInquilino=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
						};
					}
					connection.Close();
				}
			}
			return p;
		}

		public Inquilino? ObtenerPorEmail(string email)
		{
			Inquilino? p = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Clave 
					FROM Inquilinos
					WHERE Email=@email";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						p = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
						};
					}
					connection.Close();
				}
			}
			return p;
		}

		public IList<Inquilino> BuscarPorNombre(string nombre)
		{
			List<Inquilino> res = new List<Inquilino>();
			nombre = "%" + nombre + "%";
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = @"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email, Clave 
					FROM Inquilinos
					WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@nombre", SqlDbType.VarChar).Value = nombre;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						var p = new Inquilino
						{
							IdInquilino = reader.GetInt32(nameof(Inquilino.IdInquilino)),
							Nombre = reader.GetString("Nombre"),
							Apellido = reader.GetString("Apellido"),
							Dni = reader.GetString("Dni"),
							Telefono = reader.GetString("Telefono"),
							Email = reader.GetString("Email"),
							Clave = reader.GetString("Clave"),
						};
						res.Add(p);
					}
					connection.Close();
				}
			}
			return res;
		}

	}
}
