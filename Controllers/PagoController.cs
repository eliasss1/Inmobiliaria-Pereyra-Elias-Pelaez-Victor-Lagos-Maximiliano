using Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers;

public class PagoController : Controller
{

    private readonly IRepositorioPago repo;
    public PagoController(IRepositorioPago _repo)
    {
        repo = _repo;
    }

}
