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
    public class InquilinoController : Controller
    {
		// Sin inyección de dependencias (crear dentro del ctor)
		//private readonly RepositorioPropietario repositorio;

		// Con inyección de dependencias (pedir en el ctor como parámetro)
		private readonly IRepositorioInquilino repositorio;
		private readonly IConfiguration config;
		private readonly ILogger<InquilinoController> logger;

		public InquilinoController(IRepositorioInquilino repo, IConfiguration config, ILogger<InquilinoController> logger)
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
		[Authorize]
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

[Authorize]
[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Create(Inquilino inquilino)
{
    try
    {
        if (ModelState.IsValid)
        {
            repositorio.Alta(inquilino);
            TempData["Id"] = inquilino.IdInquilino;
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return View(inquilino);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error en Create");
        throw;
    }
}


		[Authorize]
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

		[Authorize]
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
		[Authorize]
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

		[Authorize]
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
	[Authorize]
    public IActionResult Details(int id)
		{
    	var inquilino = repositorio.ObtenerPorId(id); 
   		 if (inquilino == null)
   		 {
       		 return NotFound();
 		   }	
    	return View(inquilino);
    	}
	}	
}


