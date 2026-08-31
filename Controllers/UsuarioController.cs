
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;

public class UsuarioController : Controller
{
    private readonly IRepositorioUsuario repo;

    public UsuarioController(IRepositorioUsuario _repo)
    {
        repo = _repo;
    }

    public IActionResult login()
    {
        
        return View();
        
    }

}


