using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Inmobiliaria.Models;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    [Authorize]
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
            public ActionResult Index(string buscar, int pagina = 1)
    {
        try
        {
            var tamaño = 5;
            ViewBag.Buscar = buscar;
            ViewBag.Pagina = pagina;

        IList<Inmueble> lista;
            int totalRegistros;

        if (string.IsNullOrEmpty(buscar))
            {
                lista = repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño);
                totalRegistros = repositorio.ObtenerCantidad();
            }
        else
            {
                var listaFiltrada = repositorio.Buscar(buscar);
                totalRegistros = listaFiltrada.Count;
                lista = listaFiltrada.Skip((Math.Max(pagina, 1) - 1) * tamaño).Take(tamaño).ToList();
            }

            ViewBag.TotalPaginas = totalRegistros % tamaño == 0 ? totalRegistros / tamaño : totalRegistros / tamaño + 1;
        
            if (TempData.ContainsKey("Mensaje")) ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error")) ViewBag.Error = TempData["Error"];

            return View(lista);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error en Index Inmueble");
            throw;
        }
    }

        [Route("[controller]/PorDisponibilidad")]
        public ActionResult PorDisponibilidad(bool estado = true)
        {
            var lista = repositorio.ObtenerPorDisponibilidad(estado);
            ViewBag.FiltroActivo = $"Disponibilidad: {(estado ? "Activos" : "Inactivos")}";
            ViewBag.Pagina = 1; ViewBag.TotalPaginas = 1;
            return View("Index", lista);
        }

        [Route("[controller]/MasReservados")]
        public ActionResult MasReservados()
        {
            var lista = repositorio.ObtenerMasReservados();
            ViewBag.FiltroActivo = "Más reservados (Últimos 365 días)";
            ViewBag.Pagina = 1; ViewBag.TotalPaginas = 1;
            return View("Index", lista);
        }

        [Route("[controller]/SinReservas")]
        public ActionResult SinReservas(int dias = 30)
        {
            var lista = repositorio.ObtenerSinReservas(dias);
            ViewBag.FiltroActivo = $"Sin reservas en los últimos {dias} días";
            ViewBag.Pagina = 1; ViewBag.TotalPaginas = 1;
            return View("Index", lista);
        }

        [Route("[controller]/LibresEntreFechas")]
        public ActionResult LibresEntreFechas(DateTime? inicio, DateTime? fin)
        {
            if (!inicio.HasValue || !fin.HasValue)
            {
                ViewBag.MostrarFormFechas = true;
                ViewBag.Pagina = 1; ViewBag.TotalPaginas = 1;
                return View("Index", new List<Inmueble>());
            }
            
            var lista = repositorio.ObtenerLibresEntreFechas(inicio.Value, fin.Value);
            ViewBag.FiltroActivo = $"Libres entre {inicio.Value.ToShortDateString()} y {fin.Value.ToShortDateString()}";
            ViewBag.Pagina = 1; ViewBag.TotalPaginas = 1;
            return View("Index", lista);
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
        [Authorize]
        public ActionResult Details(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            
            if (entidad == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(entidad);
        }

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize(Roles = "Administrador")]
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
        
        [Authorize(Roles = "Administrador")]
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