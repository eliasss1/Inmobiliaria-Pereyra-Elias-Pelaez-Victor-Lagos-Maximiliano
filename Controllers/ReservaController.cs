using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inmobiliaria.Models;
using System.Security.Claims;
using System;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repoReserva;
        private readonly IRepositorioInquilino repoInquilino;
        private readonly IRepositorioInmueble repoInmueble;

        private readonly ILogger<ReservaController> logger;


        public ReservaController(IRepositorioReserva repoReserva, IRepositorioInquilino repoInquilino, IRepositorioInmueble repoInmueble, ILogger<ReservaController> logger)
        {
            this.repoReserva = repoReserva;
            this.repoInquilino = repoInquilino;
            this.repoInmueble = repoInmueble;
            this.logger = logger;
        }

        public ActionResult Index()
        {
            var lista = repoReserva.ObtenerTodos();

            return View(lista);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var entidad = repoReserva.ObtenerPorId(id);

            if (entidad == null)
            {
                return RedirectToAction(nameof(Index));
            }

            SetViewBag(entidad); 
            
            return View(entidad);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Reserva entidad)
        {
            try
            {
                entidad.IdReserva = id;

                ModelState.Remove("InquilinoAsociado");
                ModelState.Remove("InmuebleAsociado");
                ModelState.Remove("CreadoPorUsuarioId"); 

                if (ModelState.IsValid)
                {
                    repoReserva.Modificacion(entidad); 

                    return RedirectToAction(nameof(Index));
                }

                SetViewBag(entidad);
                return View(entidad);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al editar la reserva: {ex.Message}");
                ModelState.AddModelError("", "Ocurrio un error inesperado al intentar modificar la reserva.");
                
                SetViewBag(entidad);
                return View(entidad);
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {

            SetViewBag();
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva entidad)
        {
            try
            {
                var IdEmpleado = User.FindFirstValue(ClaimTypes.NameIdentifier);

                entidad.IdUsuarioCreador = int.Parse(IdEmpleado);

                if (ModelState.IsValid)
                {
                    repoReserva.Alta(entidad);
                    return RedirectToAction(nameof(Index));
                }

                SetViewBag(entidad);
                return View(entidad);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear una reserva: {ex.Message}");
                ModelState.AddModelError("", "Ocurrio un error inesperado al intentar guardar la reserva en la base de datos.");
                
                SetViewBag(entidad);
                return View(entidad);
            }
            

        }
        private void SetViewBag(Reserva? entidad = null)
        {   
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "IdInquilino", "Dni", entidad?.IdInquilino);
            
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "IdInmueble", "IdInmueble", entidad?.IdInmueble);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            try
            {
                var entidad = repoReserva.ObtenerPorId(id);

                return View(entidad);
            }catch(Exception ex)
            {
                logger.LogError(ex, "Error en eliminar");
                return View("Index");
            }
            

        }

        [Authorize]
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarConfirmacion(int id, Reserva entidad)
        {
            try
            {
                repoReserva.Baja(id);
                TempData["Mensaje"] = "Se elimino correctamente";
                return RedirectToAction(nameof(Index));
            }catch(Exception ex)
            {
                logger.LogError(ex, "No se pudo eliminar la reserva");
                throw;
            }
        }

        [Authorize]
        public ActionResult Details(int id)
        {
            var entidad = repoReserva.ObtenerPorId(id);
            
            if (entidad == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(entidad);
        }

}
}

