using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;


namespace Inmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
		private readonly IRepositorioPropietario repositorio;
		private readonly IConfiguration config;
		private readonly ILogger<PropietarioController> logger;

		public PropietarioController(IRepositorioPropietario repo, IConfiguration config, ILogger<PropietarioController> logger)
		{
			this.repositorio = repo;
			this.config = config;
			this.logger = logger;
		}     
        [Route("[controller]/Index")]
        public ActionResult Index(int pagina=1)
        {
            try
			{
				var tamaño = 5;
				var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño);
				ViewBag.Pagina = pagina;
				var total = repositorio.ObtenerCantidad();
				ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
				ViewBag.Id = TempData["Id"];

				if (TempData.ContainsKey("Mensaje"))
					ViewBag.Mensaje = TempData["Mensaje"];
				return View(lista);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error en Index");
				throw;
			}
        }

		public ActionResult Create()
		{
			try
			{
				return View();
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error en Create");
				throw;
			}
		}

		// POST: Propietario/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Propietario Propietario)
        {
            try
			{
				if (ModelState.IsValid)
				{
					Propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
							password: Propietario.Clave,
							salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
							prf: KeyDerivationPrf.HMACSHA1,
							iterationCount: 1000,
							numBytesRequested: 256 / 8));
					repositorio.Alta(Propietario);
					TempData["Id"] = Propietario.IdPropietario;
					return RedirectToAction(nameof(Index));
				}
				else
					return View(Propietario);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error en Create");
				throw;
			}
        }


		public ActionResult Edit(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error en Edit");
				throw;
			}
		}

		// POST: Propietario/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]		
		public ActionResult Edit(int id, Propietario entidad)
		{			
			try
			{
				var p = repositorio.ObtenerPorId(id);
				if (p == null)
					return NotFound();
				p.Nombre = entidad.Nombre;
				p.Apellido = entidad.Apellido;
				p.Dni = entidad.Dni;
				p.Email = entidad.Email;
				p.Telefono = entidad.Telefono;
				repositorio.Modificacion(p);
				TempData["Mensaje"] = "Datos guardados correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error en Edit");
				throw;
			}
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
		public ActionResult Eliminar(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error en Eliminar");
				throw;
			}
		}

		// POST: Propietario/Delete
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Eliminar(int id, Propietario entidad)
		{
			try
			{
				repositorio.Baja(id);
				TempData["Mensaje"] = "Eliminación realizada correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error en Eliminar");
				throw;
			}
		}
	}
}