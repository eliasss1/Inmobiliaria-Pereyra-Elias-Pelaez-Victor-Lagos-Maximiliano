using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class PropietariosController : Controller
    {
        public IConfiguration Configuration { get; }

        
        public PropietariosController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private string GetConnectionString()
        {
            return Configuration["ConnectionStrings:DefaultConnection"];
        }
        public IActionResult Index()
        {
            List<Propietario> listaPropietarios = new List<Propietario>();
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email FROM Propietario";
                SqlCommand command = new SqlCommand(sql, connection);
                
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Propietario propietario = new Propietario();
                        propietario.IdPropietario = Convert.ToInt32(dataReader["IdPropietario"]);
                        propietario.Nombre = Convert.ToString(dataReader["Nombre"]);
                        propietario.Apellido = Convert.ToString(dataReader["Apellido"]);
                        propietario.Dni = Convert.ToString(dataReader["Dni"]);
                        propietario.Telefono = Convert.ToString(dataReader["Telefono"]) ?? "";
                        propietario.Email = Convert.ToString(dataReader["Email"]);
                        
                        listaPropietarios.Add(propietario);
                    }
                }
                connection.Close();
            }
            return View(listaPropietarios);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                string connectionString = GetConnectionString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    string sql = $"INSERT INTO Propietario (Nombre, Apellido, Dni, Telefono, Email) " +
                                $"VALUES ('{propietario.Nombre}', '{propietario.Apellido}', '{propietario.Dni}', '{propietario.Telefono}', '{propietario.Email}')";
                    
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = CommandType.Text;
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }


        public IActionResult Edit(int id)
        {
            Propietario propietario = null;
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email FROM Propietario WHERE IdPropietario = {id}";
                SqlCommand command = new SqlCommand(sql, connection);
                
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        propietario = new Propietario
                        {
                        IdPropietario = Convert.ToInt32(dataReader["IdPropietario"]),
                            Nombre = Convert.ToString(dataReader["Nombre"]),
                            Apellido = Convert.ToString(dataReader["Apellido"]),
                            Dni = Convert.ToString(dataReader["Dni"]),
                            Telefono = Convert.ToString(dataReader["Telefono"]) ?? "",
                            Email = Convert.ToString(dataReader["Email"])
                        };
                    }
                }
                connection.Close();
            }

            if (propietario == null) return NotFound();

            return View(propietario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Propietario propietario)
        {
            if (ModelState.IsValid)
            {
                string connectionString = GetConnectionString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"UPDATE Propietario SET " +
                                $"Nombre='{propietario.Nombre}', " +
                                $"Apellido='{propietario.Apellido}', " +
                                $"Dni='{propietario.Dni}', " +
                                $"Telefono='{propietario.Telefono}', " +
                                $"Email='{propietario.Email}' " +
                                $"WHERE IdPropietario={propietario IdPropietario}";
                    
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"DELETE FROM Propietario WHERE IdPropietario = {id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        TempData["Error"] = "Error al intentar eliminar el propietario: " + ex.Message;
                    }
                    connection.Close();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
