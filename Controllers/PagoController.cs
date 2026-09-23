using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Inmobiliaria.Models;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IRepositorioPago repositorio;
        private readonly ILogger<PagoController> logger;
        private readonly IRepositorioReserva repoReserva;

        public PagoController(IRepositorioPago repo, ILogger<PagoController> logger, IRepositorioReserva rr)
        {
            this.repoReserva = rr;
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

            var ListaReserva = repoReserva.ObtenerTodos();
            ViewBag.Reservas = new SelectList(ListaReserva, "IdReserva", "IdReserva", idReserva);


            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pago entidad)
        {
            try
            {
                ModelState.Remove(nameof(entidad.Estado));
                ModelState.Remove(nameof(entidad.IdUsuarioCreador));
                ModelState.Remove(nameof(entidad.ReservaAsociada));
                if (ModelState.IsValid)
                {
                    entidad.IdUsuarioCreador = int.Parse(User.FindFirstValue("Id") ?? "0");
                    entidad.Estado = "Activo";
                    
                    repositorio.Alta(entidad);
                    TempData["Mensaje"] = "Pago registrado correctamente";
                    return RedirectToAction(nameof(Index));
                }

                var listaReservas = repoReserva.ObtenerTodos();
                ViewBag.Reservas = new SelectList(listaReservas, "IdReserva", "IdReserva", entidad.IdReserva);

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
            ModelState.Remove(nameof(entidad.IdUsuarioCreador));
            ModelState.Remove(nameof(entidad.ReservaAsociada));
            ModelState.Remove(nameof(entidad.FechaPago));
            ModelState.Remove(nameof(entidad.Importe));
            if (!ModelState.IsValid) return View(entidad);
            
            try
            {
                var pagoOriginal = repositorio.ObtenerPorId(id);
                if (pagoOriginal == null) return NotFound();
                
                pagoOriginal.Concepto = entidad.Concepto;
                
                // Si el estado cambia a Anulado desde la edición, podríamos querer registrar quién lo anuló, 
                // pero por ahora solo actualizamos el estado.
                if (entidad.Estado == "Anulado" && pagoOriginal.Estado != "Anulado")
                {
                    var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("Id");
                    if (int.TryParse(idClaim, out int idUsuarioAnulador))
                    {
                        pagoOriginal.IdUsuarioAnulador = idUsuarioAnulador;
                    }
                }
                else if (entidad.Estado == "Activo" && pagoOriginal.Estado == "Anulado")
                {
                    pagoOriginal.IdUsuarioAnulador = null;
                }
                pagoOriginal.Estado = entidad.Estado;

                repositorio.Modificacion(pagoOriginal);
                TempData["Mensaje"] = "Pago modificado correctamente";
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
                if (pagoDb == null)
                {
                    TempData["Error"] = "El pago que intenta anular no existe.";
                    return RedirectToAction(nameof(Index));
                }
                if (pagoDb.Estado == "Anulado")
                {
                    TempData["Error"] = "El pago ya se encuentra anulado.";
                    return RedirectToAction(nameof(Index));
                }
                var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("Id");
                if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int idUsuarioAnulador))
                {
                    TempData["Error"] = "No se pudo identificar al usuario administrador para registrar la auditoría.";
                    return RedirectToAction(nameof(Index));
                }
                repositorio.Baja(id, idUsuarioAnulador);
                TempData["Mensaje"] = "Pago anulado correctamente.";
                return RedirectToAction(nameof(Index));
        }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al anular el pago {Id}", id);
                TempData["Error"] = "Ocurrió un error al intentar anular el pago.";
                return RedirectToAction(nameof(Index));
            }
    }
        
        [HttpGet]
        [Authorize]
        public ActionResult Details(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                if (entidad == null)
                {
                    return NotFound();
                }
                return View(entidad);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener los detalles del pago {Id}", id);
                TempData["Error"] = "Ocurrió un error al cargar los detalles del pago.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        [Route("[controller]/PorReserva/{id}")]
        public ActionResult PorReserva(int id)
        {
            try
            {
                var lista = repositorio.ObtenerPorReserva(id);
                
                // Guardamos el ID de la reserva para poder ofrecer el botón "Cargar nuevo pago"
                ViewBag.IdReserva = id;
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                if (TempData.ContainsKey("Error"))
                    ViewBag.Error = TempData["Error"];
                return View("Index", lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener los pagos de la reserva {Id}", id);
                TempData["Error"] = "Ocurrió un error al cargar los pagos de la reserva.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}