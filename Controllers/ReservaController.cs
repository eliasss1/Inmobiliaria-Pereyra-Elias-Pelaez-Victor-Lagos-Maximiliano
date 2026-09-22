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

        public ActionResult Index(string buscar, int pagina = 1)
        {
            try
            {
                var tamaño = 5;
                ViewBag.Buscar = buscar;
                ViewBag.Pagina = pagina;

                IList<Reserva> lista;
                int totalRegistros;

                if (string.IsNullOrEmpty(buscar))
                {
                    lista = repoReserva.ObtenerLista(Math.Max(pagina, 1), tamaño);
                    totalRegistros = repoReserva.ObtenerCantidad();
                }
                else
                {
                    var listaFiltrada = repoReserva.Buscar(buscar);
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
                logger.LogError(ex, "Error en Index Reserva");
                throw;
            }
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
            entidad.IdUsuarioCreador = int.Parse(User.FindFirstValue("Id") ?? "0");

                if (entidad.FechaHasta < entidad.FechaDesde)
                {
                    ModelState.AddModelError("FechaHasta", "La fecha de fin debe ser posterior a la fecha de inicio.");
                    SetViewBag(entidad);
                    return View();
                }               
                else if(entidad.FechaDesde < DateTime.Today)
                {
                    ModelState.AddModelError("FechaDesde", "La fecha de inicio debe ser posterior a la fecha actual.");
                    SetViewBag(entidad);
                    return View();
                }
                else
                {
                    try
                    { 
                        Inmueble? inmueble = repoInmueble.ObtenerPorId(entidad.IdInmueble);
                        if (inmueble == null) 
                        { 
                            ModelState.AddModelError("IdInmueble", "El inmueble seleccionado no es válido o no existe."); 
                        }
                        if (inmueble != null && repoReserva.ExisteSolapamiento(entidad)) 
                        {
                            ModelState.AddModelError("", "El inmueble seleccionado ya se encuentra reservado en el rango de fechas elegido."); 
                        } 
                        if (!ModelState.IsValid)
                        { 
                            SetViewBag(entidad);
                            return View(entidad); 
                        }  
                        entidad.MontoPorDia = inmueble.PrecioPorDia;
                        repoReserva.Alta(entidad); 
                        TempData["Mensaje"] = "Reserva creada exitosamente y pago de seña registrado."; 
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al crear una reserva: {ex.Message}");
                        ModelState.AddModelError("", "Ocurrio un error inesperado al intentar guardar la reserva en la base de datos.");
                        
                        SetViewBag(entidad);
                        return View(entidad);
                    }                  
                }
        }
        private void SetViewBag(Reserva? entidad = null)
        {   
            ViewBag.Inquilinos = new SelectList(repoInquilino.ObtenerTodos(), "IdInquilino", "Dni", entidad?.IdInquilino);
            
            ViewBag.Inmuebles = new SelectList(repoInmueble.ObtenerTodos(), "IdInmueble", "IdInmueble", entidad?.IdInmueble);
        }

        [Authorize(Roles = "Administrador")]
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

        [Authorize(Roles = "Administrador")]
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

        public IActionResult FinalizarAnticipado(int idReserva, DateTime fechaTerminacion, decimal montoMulta, bool pagoEfectuado) 
        { 
            if (!pagoEfectuado) 
            { 
                ModelState.AddModelError("", "No se puede finalizar la reserva si no se confirma el cobro de la multa en el momento."); 
                var reserva = repoReserva.ObtenerPorId(idReserva); 
                ViewBag.MontoMulta = montoMulta; 
                return View(reserva); 
            } 
            try 
            { 
                int idUsuarioLogueado = User.Identity.IsAuthenticated ? Convert.ToInt32(User.FindFirst("Id")?.Value ?? "1") : 1;
                bool resultado = repoReserva.RegistrarTerminacionAnticipadaConPago( idReserva, fechaTerminacion, montoMulta, idUsuarioLogueado ); 
                if (resultado) 
                { 
                    TempData["Mensaje"] = "La reserva se finalizó correctamente y el pago de la multa fue registrado."; 
                    return RedirectToAction(nameof(Index)); 
                } 
            } 
            catch (Exception ex) 
            { 
                ModelState.AddModelError("", "Ocurrió un error al procesar la transacción: " + ex.Message); 
            } 
            var reservaOriginal = repoReserva.ObtenerPorId(idReserva); 
            ViewBag.MontoMulta = montoMulta;
            return View(reservaOriginal); 
        }
    [HttpGet] 
        public IActionResult Renovar(int id) 
        { 
            var reservaOriginal = repoReserva.ObtenerPorId(id);
            if (reservaOriginal == null) 
            {
                return NotFound("La reserva original no existe."); 
            } 
            var nuevaReserva = new Reserva 
            { 
                IdInmueble = reservaOriginal.IdInmueble,
                IdInquilino = reservaOriginal.IdInquilino, 
                InmuebleAsociado = reservaOriginal.InmuebleAsociado, 
                InquilinoAsociado = reservaOriginal.InquilinoAsociado,
                FechaDesde = reservaOriginal.FechaHasta, 
                FechaHasta = reservaOriginal.FechaHasta.AddDays(1) 
                }; 
                ViewBag.ReservaOriginal = reservaOriginal; 
                return View(nuevaReserva); 
        } 
    [HttpPost]
    [ValidateAntiForgeryToken] 
        public IActionResult Renovar(Reserva nuevaReserva) 
        {
            if (nuevaReserva.FechaHasta <= nuevaReserva.FechaDesde) 
            { 
                ModelState.AddModelError("FechaHasta", "La fecha de finalización debe ser posterior a la fecha de inicio de la extensión."); 
                }
            var inmueble = repoInmueble.ObtenerPorId(nuevaReserva.IdInmueble); 
            if (inmueble == null) 
            {
                ModelState.AddModelError("", "El inmueble asociado no fue encontrado."); 
            } 
            if (inmueble != null && repoReserva.ExisteSolapamiento(nuevaReserva)) 
            { 
                ModelState.AddModelError("", "No se puede extender la reserva: el inmueble ya posee otra reserva confirmada en las fechas seleccionadas."); 
            } 
            if (!ModelState.IsValid) 
            {
                ViewBag.ReservaOriginal = repoReserva.ObtenerPorId(nuevaReserva.IdReserva); 
                if (inmueble != null) nuevaReserva.InmuebleAsociado = inmueble; 
                nuevaReserva.InquilinoAsociado = repoInquilino.ObtenerPorId(nuevaReserva.IdInquilino);
                return View(nuevaReserva); 
            } 
            nuevaReserva.MontoPorDia = inmueble!.PrecioPorDia;
            nuevaReserva.IdUsuarioCreador = int.Parse(User.FindFirstValue("Id") ?? "0");
            int nuevoId = repoReserva.Alta(nuevaReserva); 
            TempData["Mensaje"] = $"Reserva extendida exitosamente. Se ha generado un nuevo alquiler (Reserva #{nuevoId})."; 
            return RedirectToAction(nameof(Index)); 
        }
    }
}



