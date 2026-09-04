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

}
}

