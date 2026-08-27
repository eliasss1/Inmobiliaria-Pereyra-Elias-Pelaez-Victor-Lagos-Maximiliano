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
    public class InquilinosController : Controller
    {
		// Sin inyección de dependencias (crear dentro del ctor)
		//private readonly RepositorioPropietario repositorio;

		// Con inyección de dependencias (pedir en el ctor como parámetro)
		private readonly IRepositorioInquilino repositorio;
		private readonly IConfiguration config;
		private readonly ILogger<InquilinosController> logger;

		public InquilinosController(IRepositorioInquilino repo, IConfiguration config, ILogger<InquilinosController> logger)
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

		// POST: Inquilino/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Inquilino inquilino)
        {
            try
			{
				if (ModelState.IsValid)
				{
					inquilino.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
							password: inquilino.Clave,
							salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
							prf: KeyDerivationPrf.HMACSHA1,
							iterationCount: 1000,
							numBytesRequested: 256 / 8));
					repositorio.Alta(inquilino);
					TempData["Id"] = inquilino.IdInquilino;
					return RedirectToAction(nameof(Index));
				}
				else
					return View(inquilino);
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

		// POST: Inquilino/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]		
		public ActionResult Edit(int id, Inquilino entidad)
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
		[HttpGet]
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

		// POST: Inquilino/Eliminar/5
		[HttpPost, ActionName("Eliminar")]
		[ValidateAntiForgeryToken]
		public ActionResult EliminarConfirmado(int id, Inquilino entidad)
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

