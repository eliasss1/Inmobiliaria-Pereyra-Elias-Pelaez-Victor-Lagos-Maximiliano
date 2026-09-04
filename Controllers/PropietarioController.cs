using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authorization;


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
        public IActionResult Index(int pagina=1)
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
	 	[Authorize]
		public IActionResult Create()
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

		[Authorize]
		[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Propietario propietario)
{
    try
    {
        if (ModelState.IsValid)
        {
            repositorio.Alta(propietario);
            TempData["Id"] = propietario.IdPropietario;
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return View(propietario);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error en Create");
        throw;
    }
}

		[Authorize]
		public IActionResult Edit(int id)
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

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]		
		public IActionResult Edit(int id, Propietario entidad)

		{			

			if (!ModelState.IsValid)
			{
				return View(entidad);
			}

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
		[Authorize]
		[HttpGet]
		public IActionResult Eliminar(int id)
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

		[Authorize]
		[HttpPost, ActionName("Eliminar")]
		[ValidateAntiForgeryToken]
		public IActionResult EliminarConfirmado(int id, Propietario entidad)
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