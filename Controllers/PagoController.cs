using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Inmobiliaria.Models;
using System;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly IRepositorioPago repositorio;
        private readonly ILogger<PagosController> logger;

        public PagosController(IRepositorioPago repo, ILogger<PagosController> logger)
        {
            this.repositorio = repo;
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

                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                    
                return View(lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Index de Pagos");
                throw;
            }
        }

        public ActionResult Create(int? idReserva)
        {
            var pago = new Pago { FechaPago = DateTime.Now };
            if (idReserva.HasValue) pago.IdReserva = idReserva.Value;
            
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.IdUsuarioCreador = int.Parse(User.FindFirstValue("Id") ?? "0");
                    entidad.Estado = "Activo";
                    
                    repositorio.Alta(entidad);
                    TempData["Mensaje"] = "Pago registrado correctamente";
                    return RedirectToAction(nameof(Index));
                }
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create de Pagos");
                throw;
            }
        }

        public ActionResult Edit(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null) return NotFound();
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit GET de Pagos");
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago entidad)
        {
            if (!ModelState.IsValid) return View(entidad);
            
            try
            {
                var pagoOriginal = repositorio.ObtenerPorId(id);
                if (pagoOriginal == null) return NotFound();
                
                pagoOriginal.Concepto = entidad.Concepto;
                
                repositorio.Modificacion(pagoOriginal);
                TempData["Mensaje"] = "Concepto modificado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit POST de Pagos");
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
                if (entidad == null) return NotFound();
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar GET");
                throw;
            }
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarConfirmado(int id)
        {
            try
            {
                var pagoDb = repositorio.ObtenerPorId(id);
                if (pagoDb != null)
                {
                    pagoDb.Estado = "Anulado";
                    pagoDb.IdUsuarioAnulador = int.Parse(User.FindFirstValue("Id") ?? "0");
                    
                    
                    repositorio.Modificacion(pagoDb); 
                }
                
                TempData["Mensaje"] = "Pago anulado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Eliminar POST");
                throw;
            }
        }
    }
}