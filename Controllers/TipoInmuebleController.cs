using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;

namespace Inmobiliaria.Controllers;

public class TipoInmuebleController : Controller
{
    private readonly IRepositorioTipoInmueble _repositorio;

    public TipoInmuebleController(IRepositorioTipoInmueble repositorio)
    {
        _repositorio = repositorio;
    }

    public IActionResult Index(int pagina = 1)
    {
        int tamaño = 5;
        var lista = _repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño);
        var total = _repositorio.ObtenerCantidad();
        
        ViewBag.Pagina = pagina;
        ViewBag.TotalPaginas = (int)Math.Ceiling((double)total / tamaño);
        
        return View(lista);
    }
    [Authorize]
    public IActionResult Details(int id)
    {
        var entidad = _repositorio.ObtenerPorId(id);
        if (entidad == null) return NotFound();
        return View(entidad);
    }
    
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TipoInmueble entidad)
    {
        if (ModelState.IsValid)
        {
            _repositorio.Alta(entidad);
            return RedirectToAction(nameof(Index));
        }
        return View(entidad);
    }

    [Authorize]
    public IActionResult Edit(int id)
    {
        var entidad = _repositorio.ObtenerPorId(id);
        if (entidad == null) return NotFound();
        return View(entidad);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, TipoInmueble entidad)
    {
        if (ModelState.IsValid)
        {
            _repositorio.Modificacion(entidad);
            return RedirectToAction(nameof(Index));
        }
        return View(entidad);
    }

    [Authorize]
    public IActionResult Eliminar(int id)
    {
        var entidad = _repositorio.ObtenerPorId(id);
        if (entidad == null) return NotFound();
        return View(entidad);
    }
    [Authorize]
    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarConfirmado(int id)
    {
        try 
        {
            _repositorio.Baja(id);
            TempData["MensajeExito"] = "El tipo de inmueble se elimino correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (MySqlConnector.MySqlException ex)
        {
            
            if (ex.Number == 1451)
            {
                
                TempData["MensajeError"] = "No se puede eliminar este Tipo de Inmueble porque hay Inmuebles asociados a el. Primero debe eliminar o modificar esos inmuebles.";
            }
            else 
            {
                
                TempData["MensajeError"] = "Ocurrio un error en la base de datos al intentar eliminar.";
            }
            
            
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            
            TempData["MensajeError"] = "Ocurrio un error inesperado.";
            return RedirectToAction(nameof(Index));
        }
    }
}