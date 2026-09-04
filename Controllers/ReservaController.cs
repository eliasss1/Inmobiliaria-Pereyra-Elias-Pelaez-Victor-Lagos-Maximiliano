using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Inmobiliaria.Models;
using System.Security.Claims;
using System;

namespace Inmobiliaria.Controllers
{
    public class ReservaController : Controller
    {
        private readonly IRepositorioReserva repoReserva;
        private readonly IRepositorioInquilino repoInquilino;
        private readonly IRepositorioInmueble repoInmueble;

        public ReservaController(IRepositorioReserva repoReserva, IRepositorioInquilino repoInquilino, IRepositorioInmueble repoInmueble)
        {
            this.repoReserva = repoReserva;
            this.repoInquilino = repoInquilino;
            this.repoInmueble = repoInmueble;
        }

        public ActionResult Index()
        {
            var lista = repoReserva.ObtenerTodos();
            return View(lista);
        }

        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reserva entidad)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SetViewBag(entidad);
                    return View(entidad);
                }

                if (!IsUserAuthenticated())
                {
                    ModelState.AddModelError("", "Debe iniciar sesión para crear una reserva");
                    return View(entidad);
                }

                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userIdString))
                {
                    entidad.CreadoPorUsuarioId = int.Parse(userIdString);
                    repoReserva.Alta(entidad);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Debe iniciar sesión para crear una reserva");
                return View(entidad);
            }
        }
            catch (FormatException ex)
            {
                HandleException(ex, entidad);
                return View(entidad);
    }
            catch (Exception ex)
        {
                Console.WriteLine($"Error al crear la reserva: {ex.Message}");
                HandleException(ex, entidad);
                return View(entidad);
        }
        }

        private void SetViewBag(Reserva entidad = null)
        {
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "IdInquilino", "Dni", entidad?.IdInquilino);
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "IdInmueble", "IdInmueble", entidad?.IdInmueble);
        }

        private bool IsUserAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }
        private void HandleException(Exception ex, Reserva entidad)
        {
            ModelState.AddModelError("", "Ocurrió un error al crear la reserva.");
            SetViewBag(entidad);
}
    }
}

