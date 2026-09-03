using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Inmobiliaria.Models;
using System;

namespace Inmobiliaria.Controllers
{
    public class TipoInmueblesController : Controller
    {
        private readonly IRepositorioTipoInmueble repositorio;
        private readonly IConfiguration config;
        private readonly ILogger<TipoInmueblesController> logger;

        public TipoInmueblesController(IRepositorioTipoInmueble repo, IConfiguration config, ILogger<TipoInmueblesController> logger)
        {
            this.repositorio = repo;
            this.config = config;
            this.logger = logger;
        }

        [Route("[controller]/Index")]
        public ActionResult Index(int pagina = 1)
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

        // POST: TipoInmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TipoInmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(entidad);
                    TempData["Id"] = entidad.IdTipoInmueble;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(entidad);
                }
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

        // POST: TipoInmuebles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, TipoInmueble entidad)
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

        // POST: TipoInmuebles/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(int id, TipoInmueble entidad)
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