using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers
{
    public class InquilinosController : Controller
    {
        public IConfiguration Configuration { get; }

        
        public InquilinosController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private string GetConnectionString()
        {
            return Configuration["ConnectionStrings:DefaultConnection"];
        }
        public IActionResult Index()
        {
            List<Inquilino> listaInquilinos = new List<Inquilino>();
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email FROM Inquilino";
                SqlCommand command = new SqlCommand(sql, connection);
                
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Inquilino inquilino = new Inquilino();
                        inquilino.IdInquilino = Convert.ToInt32(dataReader["IdInquilino"]);
                        inquilino.Nombre = Convert.ToString(dataReader["Nombre"]);
                        inquilino.Apellido = Convert.ToString(dataReader["Apellido"]);
                        inquilino.Dni = Convert.ToString(dataReader["Dni"]);
                        inquilino.Telefono = Convert.ToString(dataReader["Telefono"]) ?? "";
                        inquilino.Email = Convert.ToString(dataReader["Email"]);
                        
                        listaInquilinos.Add(inquilino);
                    }
                }
                connection.Close();
            }
            return View(listaInquilinos);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                string connectionString = GetConnectionString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    string sql = $"INSERT INTO Inquilino (Nombre, Apellido, Dni, Telefono, Email) " +
                                 $"VALUES ('{inquilino.Nombre}', '{inquilino.Apellido}', '{inquilino.Dni}', '{inquilino.Telefono}', '{inquilino.Email}')";
                    
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
            return View(inquilino);
        }


        public IActionResult Edit(int id)
        {
            Inquilino inquilino = null;
            string connectionString = GetConnectionString();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT IdInquilino, Nombre, Apellido, Dni, Telefono, Email FROM Inquilino WHERE IdInquilino = {id}";
                SqlCommand command = new SqlCommand(sql, connection);
                
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        inquilino = new Inquilino
                        {
                            IdInquilino = Convert.ToInt32(dataReader["IdInquilino"]),
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

            if (inquilino == null) return NotFound();

            return View(inquilino);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Inquilino inquilino)
        {
            if (ModelState.IsValid)
            {
                string connectionString = GetConnectionString();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"UPDATE Inquilino SET " +
                                 $"Nombre='{inquilino.Nombre}', " +
                                 $"Apellido='{inquilino.Apellido}', " +
                                 $"Dni='{inquilino.Dni}', " +
                                 $"Telefono='{inquilino.Telefono}', " +
                                 $"Email='{inquilino.Email}' " +
                                 $"WHERE IdInquilino={inquilino.IdInquilino}";
                    
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(inquilino);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"DELETE FROM Inquilino WHERE IdInquilino = {id}";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        TempData["Error"] = "Error al intentar eliminar el inquilino: " + ex.Message;
                    }
                    connection.Close();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}