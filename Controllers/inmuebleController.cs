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
        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment env;

        public InmuebleController(IRepositorioInmueble repo, IConfiguration config, ILogger<InmuebleController> logger, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env)
        {
            this.repositorio = repo;
            this.config = config;
            this.logger = logger;
            this.env = env;
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
            var repoImg = new RepositorioInmuebleImagen(config);
            ViewBag.Imagenes = repoImg.ObtenerPorInmueble(id);
            return View(entidad);
        }

        [Authorize]
        public ActionResult Create(int? idPropietario)
        {
            try
            {
                if (!idPropietario.HasValue)
                {
                    TempData["Error"] = "Debe crear el inmueble desde la vista de un propietario.";
                    return RedirectToAction("Index", "Propietario");
                }

                var inmueble = new Inmueble();
                inmueble.IdPropietario = idPropietario.Value; 

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
                {
                    return RedirectToAction(nameof(Index));
                }
                var repoTipos = new RepositorioTipoInmueble(config);
                ViewBag.Tipos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(repoTipos.ObtenerTodos(), "IdTipoInmueble", "Nombre", entidad.IdTipoInmueble);
                var repoImg = new RepositorioInmuebleImagen(config);
                ViewBag.Imagenes = repoImg.ObtenerPorInmueble(id);
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
                public ActionResult Edit(int id, Inmueble entidad, System.Collections.Generic.List<Microsoft.AspNetCore.Http.IFormFile> imagenesFiles)
        {
            if (!ModelState.IsValid)
            {
                var repoTipos = new RepositorioTipoInmueble(config);
                ViewBag.Tipos = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(repoTipos.ObtenerTodos(), "IdTipoInmueble", "Nombre", entidad.IdTipoInmueble);
                var repoImg = new RepositorioInmuebleImagen(config);
                ViewBag.Imagenes = repoImg.ObtenerPorInmueble(id);
                return View(entidad);
            }
            try
            {
                repositorio.Modificacion(entidad); 
                
                if (imagenesFiles != null && imagenesFiles.Count > 0)
                {
                    var repoImg = new RepositorioInmuebleImagen(config);
                    string pathDir = System.IO.Path.Combine(env.WebRootPath, "uploads", "inmuebles");
                    if (!System.IO.Directory.Exists(pathDir)) System.IO.Directory.CreateDirectory(pathDir);

                    bool primeraImagen = string.IsNullOrEmpty(entidad.ImagenPortada);

                    foreach (var file in imagenesFiles)
                    {
                        if (file.Length > 0)
                        {
                            string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                            string pathRel = "/uploads/inmuebles/" + fileName;
                            string pathFisico = System.IO.Path.Combine(pathDir, fileName);

                            using (var stream = new System.IO.FileStream(pathFisico, System.IO.FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            if (primeraImagen)
                            {
                                entidad.ImagenPortada = pathRel;
                                repositorio.Modificacion(entidad);
                                primeraImagen = false;
                            }

                            repoImg.Alta(new InmuebleImagen { IdInmueble = id, Url = pathRel });
                        }
                    }
                }

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




