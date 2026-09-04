using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Inmobiliaria.Models;
using System;

namespace Inmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IConfiguration config;
        private readonly ILogger<InmuebleController> logger;

        public InmuebleController(IRepositorioInmueble repo, IConfiguration config, ILogger<InmuebleController> logger)
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

        [Route("[controller]/PorPropietario/{id}")]
        public ActionResult PorPropietario(int id)
        {
            try
            {
                
                var lista = repositorio.ObtenerPorPropietario(id); 
                ViewBag.IdPropietario = id; 
                return View("Index", lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en PorPropietario");
                throw;
            }
        }

        public ActionResult Create(int? idPropietario)
        {
            try
            {
                var inmueble = new Inmueble();
                if (idPropietario.HasValue)
                {
                    inmueble.IdPropietario = idPropietario.Value; 
                }

                var repoTipos = new RepositorioTipoInmueble(config);
                
                
                var listaTipos = repoTipos.ObtenerTodos();
                
                
                ViewBag.Tipos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(listaTipos, "IdTipoInmueble", "Nombre");

                return View(inmueble);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create");
                throw;
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(entidad);
                    TempData["Id"] = entidad.IdInmueble;
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

        public IActionResult Edit(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                    return NotFound();

                
                var repoTipos = new RepositorioTipoInmueble(config);
                var listaTipos = repoTipos.ObtenerTodos();
                
                
                ViewBag.Tipos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(listaTipos, "IdTipoInmueble", "Nombre", entidad.IdTipoInmueble);

                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit");
                throw;
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Inmueble entidad)
        {
            if (!ModelState.IsValid)
            {
                var repoTipos = new RepositorioTipoInmueble(config);
                ViewBag.Tipos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(repoTipos.ObtenerTodos(), "IdTipoInmueble", "Nombre", entidad.IdTipoInmueble);
                return View(entidad);
            }
            try
            {
                repositorio.Modificacion(entidad); 
                
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

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(int id, Inmueble entidad)
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