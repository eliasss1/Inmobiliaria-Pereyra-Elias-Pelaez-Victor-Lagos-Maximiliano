using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;

namespace Inmobiliaria.Controllers;

public class TipoInmueblesController : Controller
{
    private readonly IRepositorioTipoInmueble _repositorio;

    public TipoInmueblesController(IRepositorioTipoInmueble repositorio)
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

    public IActionResult Details(int id)
    {
        var entidad = _repositorio.ObtenerPorId(id);
        if (entidad == null) return NotFound();
        return View(entidad);
    }

    public IActionResult Create()
    {
        return View();
    }

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

    public IActionResult Edit(int id)
    {
        var entidad = _repositorio.ObtenerPorId(id);
        if (entidad == null) return NotFound();
        return View(entidad);
    }

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

    
    public IActionResult Eliminar(int id)
    {
        var entidad = _repositorio.ObtenerPorId(id);
        if (entidad == null) return NotFound();
        return View(entidad);
    }

    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarConfirmado(int id)
    {
        _repositorio.Baja(id);
        return RedirectToAction(nameof(Index));
    }
}