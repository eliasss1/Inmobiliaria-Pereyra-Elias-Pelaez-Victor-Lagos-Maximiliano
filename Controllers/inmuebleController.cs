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

        private string? GuardarArchivo(Microsoft.AspNetCore.Http.IFormFile? archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return null;

            string carpeta = System.IO.Path.Combine(env.WebRootPath, "uploads", "inmuebles");
            System.IO.Directory.CreateDirectory(carpeta);

            string nombreArchivo = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(archivo.FileName);
            string rutaFisica = System.IO.Path.Combine(carpeta, nombreArchivo);

            using (var stream = new System.IO.FileStream(rutaFisica, System.IO.FileMode.Create))
            {
                archivo.CopyTo(stream);
            }

            return "/uploads/inmuebles/" + nombreArchivo;
        }

        private void GuardarImagenesAdicionales(int idInmueble, List<Microsoft.AspNetCore.Http.IFormFile>? imagenesFiles)
        {
            if (imagenesFiles == null || imagenesFiles.Count == 0)
                return;

            var repoImg = new RepositorioInmuebleImagen(config);

            foreach (var file in imagenesFiles)
            {
                var rutaImagen = GuardarArchivo(file);
                if (!string.IsNullOrEmpty(rutaImagen))
                {
                    repoImg.Alta(new InmuebleImagen
                    {
                        IdInmueble = idInmueble,
                        Url = rutaImagen
                    });
                }
            }
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
        public ActionResult PorDisponibilidad(bool estado = true, int? idPropietario = null)
        {
            var lista = repositorio.ObtenerPorDisponibilidad(estado, idPropietario);
            if (idPropietario.HasValue) ViewBag.IdPropietario = idPropietario.Value;
            ViewBag.FiltroActivo = $"Disponibilidad: {(estado ? "Activos" : "Inactivos")}";
            ViewBag.Pagina = 1; ViewBag.TotalPaginas = 1;
            return View("Index", lista);
        }

        [Route("[controller]/MasReservados")]
        public ActionResult MasReservados(int? idPropietario = null)
        {
            var lista = repositorio.ObtenerMasReservados(idPropietario);
            if (idPropietario.HasValue) ViewBag.IdPropietario = idPropietario.Value;
            ViewBag.FiltroActivo = "Más reservados (Últimos 365 días)";
            ViewBag.Pagina = 1; ViewBag.TotalPaginas = 1;
            return View("Index", lista);
        }

        [Route("[controller]/SinReservas")]
        public ActionResult SinReservas(int dias = 30, int? idPropietario = null)
        {
            var lista = repositorio.ObtenerSinReservas(dias, idPropietario);
            if (idPropietario.HasValue) ViewBag.IdPropietario = idPropietario.Value;
            ViewBag.FiltroActivo = $"Sin reservas en los últimos {dias} días";
            ViewBag.Pagina = 1; ViewBag.TotalPaginas = 1;
            return View("Index", lista);
        }

        [Route("[controller]/LibresEntreFechas")]
        public ActionResult LibresEntreFechas(DateTime? inicio, DateTime? fin, int? idPropietario = null)
        {
            if (!inicio.HasValue || !fin.HasValue)
            {
                ViewBag.MostrarFormFechas = true;
                ViewBag.Pagina = 1; ViewBag.TotalPaginas = 1;
                return View("Index", new List<Inmueble>());
            }
            
            var lista = repositorio.ObtenerLibresEntreFechas(inicio.Value, fin.Value, idPropietario);
            if (idPropietario.HasValue) ViewBag.IdPropietario = idPropietario.Value;
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
        public ActionResult Create(Inmueble entidad, Microsoft.AspNetCore.Http.IFormFile? portadaFile, List<Microsoft.AspNetCore.Http.IFormFile>? imagenesFiles)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(entidad);
                }

                var rutaPortada = GuardarArchivo(portadaFile);
                if (!string.IsNullOrEmpty(rutaPortada))
                {
                    entidad.ImagenPortada = rutaPortada;
                }

                int idInmueble = repositorio.Alta(entidad);
                entidad.IdInmueble = idInmueble;
                GuardarImagenesAdicionales(entidad.IdInmueble, imagenesFiles);

                TempData["Id"] = entidad.IdInmueble;
                return RedirectToAction(nameof(Index));
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
        public ActionResult Edit(int id, Inmueble entidad, Microsoft.AspNetCore.Http.IFormFile? portadaFile, List<Microsoft.AspNetCore.Http.IFormFile>? imagenesFiles)
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
                var rutaPortada = GuardarArchivo(portadaFile);
                if (!string.IsNullOrEmpty(rutaPortada))
                {
                    entidad.ImagenPortada = rutaPortada;
                }

                repositorio.Modificacion(entidad);
                GuardarImagenesAdicionales(id, imagenesFiles);

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
        [HttpPost]
        public IActionResult EliminarImagen(int idImagen)
        {
            try
            {
                var repoImg = new RepositorioInmuebleImagen(config);
                var img = repoImg.ObtenerPorId(idImagen);
                if (img != null)
                {
                    // Si esta imagen era la portada del inmueble, la blanqueamos
                    var inmueble = repositorio.ObtenerPorId(img.IdInmueble);
                    if (inmueble != null && inmueble.ImagenPortada == img.Url)
                    {
                        inmueble.ImagenPortada = null;
                        repositorio.Modificacion(inmueble);
                    }

                    string pathFisico = System.IO.Path.Combine(env.WebRootPath, img.Url.TrimStart('/'));
                    if (System.IO.File.Exists(pathFisico)) System.IO.File.Delete(pathFisico);
                    repoImg.Eliminar(idImagen);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al eliminar la imagen");
                return BadRequest();
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
            catch (MySqlConnector.MySqlException ex)
            {
                if (ex.Number == 1451)
                {
                    TempData["Error"] = "No se puede eliminar el inmueble porque tiene datos asociados.";
                }
                else
                {
                    TempData["Error"] = "Ocurrió un error en la base de datos al intentar eliminar.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar");
                TempData["Error"] = "Ocurrió un error inesperado al intentar eliminar.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}







